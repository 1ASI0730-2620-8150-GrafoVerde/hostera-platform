using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Queries;

namespace HosteraBackend.API.Iam.Application.QueryServices;

/// <summary>
///     Represents the user query service of the IAM bounded context.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    ///     Handles the get user by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetUserByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="User" /> if found, or null otherwise.</returns>
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get all users query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllUsersQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The users, ordered by username.</returns>
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get all users by organization id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllUsersByOrganizationIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The users of the organization, ordered by username.</returns>
    Task<IEnumerable<User>> Handle(GetAllUsersByOrganizationIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get user by username query.
    /// </summary>
    /// <param name="query">The <see cref="GetUserByUsernameQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="User" /> if found, or null otherwise.</returns>
    Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken);
}
