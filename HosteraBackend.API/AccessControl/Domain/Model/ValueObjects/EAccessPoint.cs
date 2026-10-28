namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Place of the property where an RFID reader receives a credential.
/// </summary>
public enum EAccessPoint
{
    /// <summary>
    ///     Main entrance of the property.
    /// </summary>
    MainEntrance,

    /// <summary>
    ///     Door of a room.
    /// </summary>
    RoomDoor,

    /// <summary>
    ///     Elevator of the property.
    /// </summary>
    Elevator,

    /// <summary>
    ///     Corridor reserved for the staff.
    /// </summary>
    ServiceCorridor,

    /// <summary>
    ///     Storage room of the property.
    /// </summary>
    StorageRoom
}
