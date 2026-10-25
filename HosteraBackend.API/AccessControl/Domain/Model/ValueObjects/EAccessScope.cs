namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Areas a staff credential opens.
/// </summary>
public enum EAccessScope
{
    /// <summary>
    ///     Service corridors and storage rooms.
    /// </summary>
    ServiceAreas,

    /// <summary>
    ///     Every room of the property.
    /// </summary>
    AllRooms,

    /// <summary>
    ///     Every room and area of the property.
    /// </summary>
    AllAreas
}
