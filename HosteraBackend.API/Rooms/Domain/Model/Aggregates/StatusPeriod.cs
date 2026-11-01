using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents a date range during which a room has one operational status, with the reason it was set.
/// </summary>
/// <remarks>
///     Both dates are included. The status periods of a room never overlap: setting a status over days that already
///     have one first releases those days, which trims, splits or removes the existing periods.
/// </remarks>
public partial class StatusPeriod
{
    /// <summary>
    ///     The longest reason accepted.
    /// </summary>
    public const int MaxReasonLength = 240;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected StatusPeriod()
    {
        Reason = null!;
    }

    /// <summary>
    ///     Creates the status period of a room.
    /// </summary>
    /// <param name="propertyId">The identifier of the property that owns the room.</param>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="status">The operational status of the room during the period.</param>
    /// <param name="startDate">The first day of the period.</param>
    /// <param name="endDate">The last day of the period.</param>
    /// <param name="reason">Why the status was set. Required.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when an identifier is not positive, the period ends before it starts or the reason is empty.
    /// </exception>
    public StatusPeriod(int propertyId, int roomId, EOperationalStatus status, DateOnly startDate, DateOnly endDate,
        string reason)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (endDate < startDate)
            throw new ArgumentException("A status period cannot end before it starts.", nameof(endDate));

        var trimmedReason = reason.Trim();
        if (trimmedReason.Length > MaxReasonLength)
            throw new ArgumentException($"A reason has at most {MaxReasonLength} characters.", nameof(reason));

        PropertyId = propertyId;
        RoomId = roomId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        Reason = trimmedReason;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this status period.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the room.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    ///     Gets the operational status of the room during the period.
    /// </summary>
    public EOperationalStatus Status { get; private set; }

    /// <summary>
    ///     Gets the first day of the period.
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    ///     Gets the last day of the period.
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    ///     Gets why the status was set.
    /// </summary>
    public string Reason { get; private set; }

    /// <summary>
    ///     Gets whether the status prevents booking the room. Only Needs cleaning allows it.
    /// </summary>
    public bool PreventsBooking => Status != EOperationalStatus.NeedsCleaning;

    /// <summary>
    ///     Indicates whether the period includes a day.
    /// </summary>
    /// <param name="date">The day to evaluate.</param>
    /// <returns>True when the day is between the first and the last day of the period.</returns>
    public bool Covers(DateOnly date) => StartDate <= date && date <= EndDate;

    /// <summary>
    ///     Indicates whether the period shares at least one day with a range.
    /// </summary>
    /// <param name="startDate">The first day of the range.</param>
    /// <param name="endDate">The last day of the range.</param>
    /// <returns>True when the period and the range overlap.</returns>
    public bool Overlaps(DateOnly startDate, DateOnly endDate) => StartDate <= endDate && startDate <= EndDate;

    /// <summary>
    ///     Indicates whether a range includes every day of the period.
    /// </summary>
    /// <param name="startDate">The first day of the range.</param>
    /// <param name="endDate">The last day of the range.</param>
    /// <returns>True when releasing the range leaves no day of the period.</returns>
    public bool IsWithin(DateOnly startDate, DateOnly endDate) => startDate <= StartDate && EndDate <= endDate;

    /// <summary>
    ///     Removes the days of a range from the period, keeping the days before the range in this period.
    /// </summary>
    /// <remarks>
    ///     When the range falls inside the period, the days after it become a new period with the same status and
    ///     reason. The caller must remove the period instead when <see cref="IsWithin" /> is true.
    /// </remarks>
    /// <param name="startDate">The first day of the range to release.</param>
    /// <param name="endDate">The last day of the range to release.</param>
    /// <returns>The new period with the days after the range, or null when the period does not continue after it.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the range covers the whole period.</exception>
    public StatusPeriod? ReleaseDays(DateOnly startDate, DateOnly endDate)
    {
        if (!Overlaps(startDate, endDate))
            return null;

        if (IsWithin(startDate, endDate))
            throw new InvalidOperationException("Releasing the whole period removes it instead of trimming it.");

        StatusPeriod? remainder = null;
        if (EndDate > endDate && StartDate < startDate)
            remainder = new StatusPeriod(PropertyId, RoomId, Status, endDate.AddDays(1), EndDate, Reason);

        if (StartDate < startDate)
            EndDate = startDate.AddDays(-1);
        else
            StartDate = endDate.AddDays(1);

        return remainder;
    }
}
