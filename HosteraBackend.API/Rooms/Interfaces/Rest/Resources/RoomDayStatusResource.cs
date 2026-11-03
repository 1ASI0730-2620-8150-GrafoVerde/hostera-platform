namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Room day status resource for REST API
/// </summary>
/// <param name="Date">The day</param>
/// <param name="Status">
///     The status of the room on the day: available, booked, occupied, needs-cleaning, blocked or out-of-service
/// </param>
public record RoomDayStatusResource(DateOnly Date, string Status);
