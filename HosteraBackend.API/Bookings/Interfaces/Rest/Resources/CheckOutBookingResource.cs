using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Check-out resource for REST API
/// </summary>
/// <param name="RoomCondition">The condition in which the guest left the room: no-issues or needs-attention</param>
/// <param name="DepartureNote">An optional note about the departure or the room</param>
/// <param name="OperatorId">The identifier of the operator who completes the check-out</param>
public record CheckOutBookingResource(
    [Required] string RoomCondition,
    [MaxLength(500)] string? DepartureNote,
    [Range(1, int.MaxValue)] int OperatorId);
