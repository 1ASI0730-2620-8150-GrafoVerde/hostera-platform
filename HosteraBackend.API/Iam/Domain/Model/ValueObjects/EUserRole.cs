namespace HosteraBackend.API.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Role of a user in the organization whose properties the user operates.
/// </summary>
/// <remarks>
///     The role describes the user, for example in the account menu of the frontend. It does not grant or restrict any
///     action yet: every user reaches all the properties of the organization.
/// </remarks>
public enum EUserRole
{
    /// <summary>
    ///     The user manages the operation of the properties of the organization. A user who signs up and starts a new
    ///     organization gets this role.
    /// </summary>
    OperationsManager,

    /// <summary>
    ///     The user carries out the daily operation of the properties of the organization.
    /// </summary>
    Operator
}
