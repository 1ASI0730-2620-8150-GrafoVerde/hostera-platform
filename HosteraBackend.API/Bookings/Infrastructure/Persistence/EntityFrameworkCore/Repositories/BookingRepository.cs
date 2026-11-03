using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IBookingRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class BookingRepository(AppDbContext context)
    : BaseRepository<Booking>(context), IBookingRepository
{
    private static readonly EBookingStatus[] RoomHoldingStatuses =
        [EBookingStatus.Pending, EBookingStatus.Confirmed, EBookingStatus.CheckedIn];

    /// <inheritdoc />
    public async Task<IEnumerable<Booking>> FindByPropertyIdAsync(GetAllBookingsByPropertyIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var bookings = Filter(query)
            .OrderBy(booking => booking.CheckInDate)
            .ThenBy(booking => booking.Code)
            .AsQueryable();

        if (query.Page is { } page)
            bookings = bookings.Skip((page - 1) * query.PageSize).Take(query.PageSize);

        return await bookings.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountByPropertyIdAsync(GetAllBookingsByPropertyIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await Filter(query).CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> FindLatestCodeNumberByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        var codes = await Context.Set<Booking>()
            .Where(booking => booking.PropertyId == propertyId)
            .Select(booking => booking.Code)
            .ToListAsync(cancellationToken);
        return codes.Select(Booking.NumberOf).DefaultIfEmpty(0).Max();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsRoomHoldAsync(int roomId, DateOnly checkInDate, DateOnly checkOutDate,
        int? excludedBookingId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Booking>()
            .AnyAsync(booking => booking.RoomId == roomId
                                 && booking.Id != excludedBookingId
                                 && RoomHoldingStatuses.Contains(booking.Status)
                                 && booking.CheckInDate < checkOutDate
                                 && checkInDate < booking.CheckOutDate, cancellationToken);
    }

    private IQueryable<Booking> Filter(GetAllBookingsByPropertyIdQuery query)
    {
        var bookings = Context.Set<Booking>().Where(booking => booking.PropertyId == query.PropertyId);

        if (query.Status is { } status)
            bookings = bookings.Where(booking => booking.Status == status);
        if (query.RoomId is { } roomId)
            bookings = bookings.Where(booking => booking.RoomId == roomId);
        // A stay has a night on or after "from" when it leaves after "from"
        if (query.From is { } from)
            bookings = bookings.Where(booking => booking.CheckOutDate > from);
        if (query.To is { } to)
            bookings = bookings.Where(booking => booking.CheckInDate <= to);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            bookings = bookings.Where(booking => booking.GuestName.Contains(search) || booking.Code.Contains(search));
        }

        return bookings;
    }
}
