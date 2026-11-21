using HosteraBackend.API.Iam.Application.QueryServices;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Queries;
using HosteraBackend.API.Iam.Domain.Repositories;

namespace HosteraBackend.API.Iam.Application.Internal.QueryServices;

/// <summary>
///     Represents the user query service of the IAM bounded context.
/// </summary>
/// <param name="userRepository">
///     The <see cref="IUserRepository" /> to use.
/// </param>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.ListAsNoTrackingAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        return string.IsNullOrWhiteSpace(query.Username)
            ? null
            : await userRepository.FindByUsernameAsNoTrackingAsync(query.Username.Trim(), cancellationToken);
    }
}
