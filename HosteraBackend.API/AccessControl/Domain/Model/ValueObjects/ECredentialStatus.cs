namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Status of a credential at a given moment, derived from its validity period and its revocation.
/// </summary>
public enum ECredentialStatus
{
    /// <summary>
    ///     The access has not started yet.
    /// </summary>
    Scheduled,

    /// <summary>
    ///     The credential grants access.
    /// </summary>
    Active,

    /// <summary>
    ///     The access already ended.
    /// </summary>
    Expired,

    /// <summary>
    ///     The credential was revoked.
    /// </summary>
    Revoked
}
