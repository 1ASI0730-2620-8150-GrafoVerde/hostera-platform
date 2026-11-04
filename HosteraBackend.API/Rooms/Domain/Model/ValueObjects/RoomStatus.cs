namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

public enum RoomStatus
{
    Available,
    Occupied,
    Dirty,
    Clean,
    Inspected,
    Blocked,
    OutOfService
}