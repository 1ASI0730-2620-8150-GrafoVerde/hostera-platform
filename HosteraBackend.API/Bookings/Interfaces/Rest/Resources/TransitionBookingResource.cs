using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Booking transition resource for REST API
/// </summary>
/// <param name="Status">
///     The status to reach: confirmed, cancelled, no-show, or pending to restore a cancelled booking
/// </param>
/// <param name="OperatorId">The identifier of the operator who makes the change</param>
/// <param name="CancellationReason">
///     Required to cancel: guest-request, change-of-plans, duplicate-booking, property-unable or other
/// </param>
/// <param name="CancellationNote">An internal note about the cancellation, required for the other reason</param>
public record TransitionBookingResource(
    [Required] string Status,
    [Range(1, int.MaxValue)] int OperatorId,
    string? CancellationReason,
    [MaxLength(500)] string? CancellationNote);
