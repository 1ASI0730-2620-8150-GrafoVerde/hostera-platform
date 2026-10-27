namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Kind of person who presented a credential.
/// </summary>
public enum EHolderType
{
    /// <summary>
    ///     A guest with a key card.
    /// </summary>
    Guest,

    /// <summary>
    ///     A staff member with a staff credential.
    /// </summary>
    Staff
}
