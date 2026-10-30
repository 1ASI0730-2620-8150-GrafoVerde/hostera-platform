namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Status of a status period of a room, as the Overview bounded context reads it from the Rooms bounded context.
/// </summary>
public enum ERoomStatus
{
    /// <summary>
    ///     The room cannot be sold, for example for a private event.
    /// </summary>
    Blocked,

    /// <summary>
    ///     The room is under repair.
    /// </summary>
    OutOfService,

    /// <summary>
    ///     The room must be cleaned before the next guest.
    /// </summary>
    NeedsCleaning
}
