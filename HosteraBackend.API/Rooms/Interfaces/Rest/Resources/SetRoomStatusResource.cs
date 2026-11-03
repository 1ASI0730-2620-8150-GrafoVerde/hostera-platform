using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Set room status resource for REST API
/// </summary>
/// <param name="Status">
///     The status to set: blocked, out-of-service or needs-cleaning; or available to release the days
/// </param>
/// <param name="StartDate">The first day of the range</param>
/// <param name="EndDate">The last day of the range</param>
/// <param name="Reason">Why the status is set; required unless the status is available</param>
public record SetRoomStatusResource(
    [Required] string Status,
    DateOnly StartDate,
    DateOnly EndDate,
    [MaxLength(240)] string? Reason);
