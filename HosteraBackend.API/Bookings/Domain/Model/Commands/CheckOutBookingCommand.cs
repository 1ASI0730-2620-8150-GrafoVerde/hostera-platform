using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Commands;

/// <summary>
///     Command to end the stay of a checked-in booking
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="RoomCondition">The ERoomCondition in which the guest left the room</param>
/// <param name="DepartureNote">An optional note about the departure or the room</param>
/// <param name="OperatorId">The identifier of the operator who completes the check-out</param>
/// <param name="ExpectedVersion">
///     The version of the booking the client read, or null to skip the check; a newer version rejects the change
/// </param>
public record CheckOutBookingCommand(
    int BookingId,
    ERoomCondition RoomCondition,
    string? DepartureNote,
    int OperatorId,
    int? ExpectedVersion = null);
