using HosteraBackend.API.Iam.Application.CommandServices;
using HosteraBackend.API.Iam.Application.QueryServices;
using HosteraBackend.API.Iam.Domain.Model.Commands;
using HosteraBackend.API.Iam.Domain.Model.Queries;
using HosteraBackend.API.Iam.Interfaces.ACL;

namespace HosteraBackend.API.Iam.Application.ACL;

/// <summary>
///     Facade that other bounded contexts use to create and read the users of the IAM bounded context.
/// </summary>
/// <param name="userCommandService">The user command service</param>
/// <param name="userQueryService">The user query service</param>
public class IamContextFacade(IUserCommandService userCommandService, IUserQueryService userQueryService)
    : IIamContextFacade
{
    /// <inheritdoc />
    public async Task<int> CreateUserAsync(string username, string password, int organizationId,
        CancellationToken cancellationToken)
    {
        var result = await userCommandService.Handle(new SignUpCommand(username, password, organizationId),
            cancellationToken);
        return result.Fold(user => user.Id, _ => 0);
    }

    /// <inheritdoc />
    public async Task<int> FetchUserIdByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByUsernameQuery(username), cancellationToken);
        return user?.Id ?? 0;
    }

    /// <inheritdoc />
    public async Task<string> FetchUsernameByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId), cancellationToken);
        return user?.Username ?? string.Empty;
    }

    /// <inheritdoc />
    public async Task<int?> FetchOrganizationIdByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId), cancellationToken);
        return user?.OrganizationId;
    }
}
