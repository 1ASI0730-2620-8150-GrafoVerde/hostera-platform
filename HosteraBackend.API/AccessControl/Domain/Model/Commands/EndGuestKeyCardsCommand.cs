namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to end the key cards of a booking when its guest checks out
/// </summary>
/// <param name="BookingId">The identifier of the booking whose key cards end</param>
/// <param name="EndAt">The moment the key cards stop opening doors</param>
public record EndGuestKeyCardsCommand(int BookingId, DateTimeOffset EndAt);
