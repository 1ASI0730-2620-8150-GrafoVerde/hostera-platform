namespace HosteraBackend.API.Shared.Application.Internal.OutboundServices;

/// <summary>
///     Provides the current moment and the current calendar day of the hotel operation.
/// </summary>
/// <remarks>
///     Business rules that depend on "today", such as when a booking can be checked in, use the day in the time zone
///     of the properties instead of the day in UTC.
/// </remarks>
public interface IClock
{
    /// <summary>
    ///     Gets the current moment.
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    ///     Gets the current calendar day in the time zone of the properties.
    /// </summary>
    DateOnly Today { get; }

    /// <summary>
    ///     Converts a calendar day and a time of day of the properties into a moment.
    /// </summary>
    /// <param name="date">The day in the time zone of the properties.</param>
    /// <param name="time">The time of day in the time zone of the properties.</param>
    /// <returns>The moment, with the offset of the time zone on that day.</returns>
    DateTimeOffset AtLocalTime(DateOnly date, TimeOnly time);
}
