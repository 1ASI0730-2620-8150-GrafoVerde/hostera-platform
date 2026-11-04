using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Rooms.Domain.Model.Errors;

public static class RoomErrors
{
    public static readonly Error RoomNotFound = new(
        "Monitoring.RoomNotFound",
        "Room not found.");

    public static readonly Error RoomUpdateFailed = new(
        "Monitoring.RoomUpdateFailed",
        "An error occurred while updating the room data.");

    public static readonly Error RoomCreationFailed = new(
        "Monitoring.RoomCreationFailed",
        "An error occurred while creating the room.");

    public static readonly Error InternalServerError = new(
        "Monitoring.InternalServerError",
        "An unexpected error occurred.");

}