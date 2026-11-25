using HosteraBackend.API.Iam.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Iam.Interfaces.ACL;

/// <summary>
///     Facade that other bounded contexts use to create and read the users of the IAM bounded context.
/// </summary>
public interface IIamContextFacade
{
    /// <summary>
    ///     Creates a user of an organization.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user, between 8 and 64 characters.</param>
    /// <param name="fullName">The full name of the person who uses the account, between 2 and 100 characters.</param>
    /// <param name="role">The role of the user in the organization.</param>
    /// <param name="organizationId">The identifier of the organization of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the created user, or zero when it could not be created.</returns>
    Task<int> CreateUserAsync(string username, string password, string fullName, EUserRole role, int organizationId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Changes the full name and the role of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="fullName">The new full name, between 2 and 100 characters.</param>
    /// <param name="role">The new role of the user in the organization.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the user was updated; false when it does not exist or the values are not valid.</returns>
    Task<bool> UpdateUserProfileAsync(int userId, string fullName, EUserRole role,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Fetches the identifier of a user by username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the user, or zero when it does not exist.</returns>
    Task<int> FetchUserIdByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    ///     Fetches the username of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The username, or an empty string when the user does not exist.</returns>
    Task<string> FetchUsernameByUserIdAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    ///     Fetches the organization of a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the organization, or null when the user does not exist.</returns>
    Task<int?> FetchOrganizationIdByUserIdAsync(int userId, CancellationToken cancellationToken);
}
