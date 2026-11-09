using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Commands;

/// <summary>
///     Command to move a booking to another status: confirm, cancel or restore it, or mark it as a no-show
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="TargetStatus">
///     The EBookingStatus to reach: Confirmed, Cancelled, NoShow, or Pending to restore a cancelled booking
/// </param>
/// <param name="OperatorId">The identifier of the operator who makes the change</param>
/// <param name="CancellationReason">The ECancellationReason, required to cancel</param>
/// <param name="CancellationNote">An internal note about the cancellation, required for the Other reason</param>
/// <param name="ExpectedVersion">
///     The version of the booking the client read, or null to skip the check; a newer version rejects the change
/// </param>
public record TransitionBookingCommand(
    int BookingId,
    EBookingStatus TargetStatus,
    int OperatorId,
    ECancellationReason? CancellationReason = null,
    string? CancellationNote = null,
    int? ExpectedVersion = null);
