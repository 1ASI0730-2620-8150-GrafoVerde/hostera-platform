namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Reason an RFID reader denied a credential.
/// </summary>
public enum EDenialReason
{
    /// <summary>
    ///     The credential does not open that access point or that room.
    /// </summary>
    OutsideScope,

    /// <summary>
    ///     The access of the credential has not started yet.
    /// </summary>
    NotActiveYet,

    /// <summary>
    ///     The access of the credential already ended.
    /// </summary>
    Expired,

    /// <summary>
    ///     The credential was revoked.
    /// </summary>
    Revoked
}
