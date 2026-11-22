using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Iam.Domain.Repositories;

/// <summary>
///     The user repository contract.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Find a user by username
    /// </summary>
    /// <param name="username">The username to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found, or null otherwise.</returns>
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find a user by username without change tracking
    /// </summary>
    /// <param name="username">The username to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found, or null otherwise.</returns>
    Task<User?> FindByUsernameAsNoTrackingAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find a user by id without change tracking
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user if found, or null otherwise.</returns>
    Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     List the users ordered by username without change tracking
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An Enumerable containing the users, or empty otherwise.</returns>
    Task<IEnumerable<User>> ListAsNoTrackingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Check if a user exists by username
    /// </summary>
    /// <param name="username">The username to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True if a user uses the username, false otherwise.</returns>
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the highest organization identifier of the users
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The highest organization identifier, or zero when there are no users.</returns>
    Task<int> FindLatestOrganizationIdAsync(CancellationToken cancellationToken = default);
}
