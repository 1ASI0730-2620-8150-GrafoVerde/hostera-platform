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
}
