namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Reason given when a credential is revoked.
/// </summary>
public enum ERevocationReason
{
    /// <summary>
    ///     The card was lost.
    /// </summary>
    LostCard,

    /// <summary>
    ///     The card was damaged.
    /// </summary>
    DamagedCard,

    /// <summary>
    ///     The card represents a security risk.
    /// </summary>
    SecurityRisk,

    /// <summary>
    ///     The staff member left the property.
    /// </summary>
    StaffLeft,

    /// <summary>
    ///     The card was replaced by a new one.
    /// </summary>
    Replaced,

    /// <summary>
    ///     Another reason, described in the revocation note.
    /// </summary>
    Other
}
