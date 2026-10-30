namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Result of an access attempt at an RFID reader.
/// </summary>
public enum EAccessResult
{
    /// <summary>
    ///     The door opened.
    /// </summary>
    Granted,

    /// <summary>
    ///     The door stayed closed.
    /// </summary>
    Denied
}
