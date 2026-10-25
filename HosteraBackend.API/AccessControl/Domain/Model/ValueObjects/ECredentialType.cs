namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Kind of credential.
/// </summary>
public enum ECredentialType
{
    /// <summary>
    ///     Card that opens the room of a guest during a stay.
    /// </summary>
    GuestKeyCard,

    /// <summary>
    ///     Card that opens the areas assigned to a staff member.
    /// </summary>
    StaffCredential
}
