namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get all RFID credentials issued for a room
/// </summary>
/// <param name="RoomNumber">The room number to search</param>
public record GetAllRfidCredentialsByRoomNumberQuery(string RoomNumber);
