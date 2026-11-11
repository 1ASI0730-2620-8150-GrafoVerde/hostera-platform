using System.Data.Common;
using HosteraBackend.API.Overview.Domain.Model.ReadModels;
using HosteraBackend.API.Overview.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Overview.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IOverviewReadRepository" />.
/// </summary>
/// <remarks>
///     The Overview bounded context does not own the rooms, status periods and bookings, so it reads their tables
///     with read-only SQL queries instead of mapping their aggregates. The tables and columns are the ones of the
///     database design of the Rooms and Bookings bounded contexts. A table that cannot be read, for example because
///     its bounded context is not deployed yet, makes its method return null.
/// </remarks>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
/// <param name="logger">
///     The logger used to report a source that cannot provide its data.
/// </param>
public class OverviewReadRepository(AppDbContext context, ILogger<OverviewReadRepository> logger)
    : IOverviewReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomReadModel>?> FindRoomsByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await ReadAsync("rooms", async () =>
        {
            var rows = await context.Database
                .SqlQuery<RoomRow>($"""
                                    SELECT id AS Id, number AS Number
                                    FROM rooms
                                    WHERE property_id = {propertyId}
                                    """)
                .ToListAsync(cancellationToken);

            return rows
                .Select(row => new RoomReadModel(row.Id, row.Number))
                .OrderBy(room => room.Number)
                .ToList();
        });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatusPeriodReadModel>?> FindStatusPeriodsByPropertyIdAsync(int propertyId,
        DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        var firstDay = from.ToDateTime(TimeOnly.MinValue);
        var lastDay = to.ToDateTime(TimeOnly.MinValue);

        return await ReadAsync("status periods", async () =>
        {
            var rows = await context.Database
                .SqlQuery<StatusPeriodRow>($"""
                                            SELECT room_id AS RoomId, status AS Status,
                                                   start_date AS StartDate, end_date AS EndDate
                                            FROM status_periods
                                            WHERE property_id = {propertyId}
                                              AND start_date <= {lastDay}
                                              AND end_date >= {firstDay}
                                            """)
                .ToListAsync(cancellationToken);

            return rows
                .Select(row => ParseStatus<ERoomStatus>(row.Status) is { } status
                    ? new StatusPeriodReadModel(row.RoomId, status, DateOnly.FromDateTime(row.StartDate),
                        DateOnly.FromDateTime(row.EndDate))
                    : null)
                .OfType<StatusPeriodReadModel>()
                .ToList();
        });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BookingReadModel>?> FindBookingsByPropertyIdAsync(int propertyId, DateOnly from,
        DateOnly to, CancellationToken cancellationToken = default)
    {
        var firstNight = from.ToDateTime(TimeOnly.MinValue);
        var lastNight = to.ToDateTime(TimeOnly.MinValue);

        return await ReadAsync("bookings", async () =>
        {
            var rows = await context.Database
                .SqlQuery<BookingRow>($"""
                                       SELECT id AS Id, code AS Code, status AS Status, guest_name AS GuestName,
                                              room_id AS RoomId, check_in_date AS CheckInDate,
                                              check_out_date AS CheckOutDate, total_amount AS TotalAmount,
                                              (SELECT COALESCE(SUM(payments.amount), 0)
                                               FROM payments
                                               WHERE payments.booking_id = bookings.id) AS PaidAmount
                                       FROM bookings
                                       WHERE property_id = {propertyId}
                                         AND check_in_date <= {lastNight}
                                         AND check_out_date > {firstNight}
                                       """)
                .ToListAsync(cancellationToken);

            return rows
                .Select(row => ParseStatus<EBookingStatus>(row.Status) is { } status
                    ? new BookingReadModel(row.Id, row.Code, status, row.GuestName, row.RoomId,
                        DateOnly.FromDateTime(row.CheckInDate), DateOnly.FromDateTime(row.CheckOutDate),
                        row.TotalAmount, row.PaidAmount)
                    : null)
                .OfType<BookingReadModel>()
                .ToList();
        });
    }

    /// <summary>
    ///     Runs a read and returns null when its table cannot be read.
    /// </summary>
    private async Task<IReadOnlyList<T>?> ReadAsync<T>(string source, Func<Task<List<T>>> read)
    {
        try
        {
            return await read();
        }
        catch (Exception exception) when (exception is DbException or InvalidOperationException)
        {
            logger.LogWarning(exception, "The {Source} could not be read for the operational overview.", source);
            return null;
        }
    }

    /// <summary>
    ///     Parses a stored status, such as "CheckedIn", "checked-in" or "checked_in", into an enum value.
    /// </summary>
    /// <returns>The enum value, or null for an unknown status, which the overview ignores.</returns>
    private static TEnum? ParseStatus<TEnum>(string? status) where TEnum : struct, Enum
    {
        var normalized = status?.Replace("-", string.Empty).Replace("_", string.Empty);
        return Enum.TryParse<TEnum>(normalized, true, out var value) && Enum.IsDefined(value) ? value : null;
    }

    /// <summary>
    ///     Row of the rooms table read by the overview.
    /// </summary>
    public sealed class RoomRow
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Row of the status periods table read by the overview.
    /// </summary>
    public sealed class StatusPeriodRow
    {
        public int RoomId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    ///     Row of the bookings table read by the overview.
    /// </summary>
    public sealed class BookingRow
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public int? RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
