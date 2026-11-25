using HosteraBackend.API.Iam.Application.CommandServices;
using HosteraBackend.API.Iam.Application.Errors;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Commands;
using HosteraBackend.API.Iam.Domain.Model.ValueObjects;
using HosteraBackend.API.Iam.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Iam.Application.Internal.CommandServices;

/// <summary>
///     Represents the user command service of the IAM bounded context.
/// </summary>
/// <param name="userRepository">
///     The <see cref="IUserRepository" /> to use.
/// </param>
/// <param name="tokenService">
///     The <see cref="ITokenService" /> that issues the token of a signed-in user.
/// </param>
/// <param name="hashingService">
///     The <see cref="IHashingService" /> that hashes and verifies passwords.
/// </param>
/// <param name="externalPropertyService">
///     The <see cref="ExternalPropertyService" /> that reads the organizations that own properties.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    ExternalPropertyService externalPropertyService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    /// <inheritdoc />
    /// <remarks>
    ///     An unknown username and a wrong password return the same error, so the response does not reveal which
    ///     usernames exist.
    /// </remarks>
    public async Task<Result<(User User, string Token), IamError>> Handle(SignInCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrEmpty(command.Password))
            return new Result<(User User, string Token), IamError>.Failure(IamError.InvalidInput);

        var user = await userRepository.FindByUsernameAsync(command.Username.Trim(), cancellationToken);
        if (user is null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return new Result<(User User, string Token), IamError>.Failure(IamError.InvalidCredentials);

        var token = tokenService.GenerateToken(user);
        return new Result<(User User, string Token), IamError>.Success((user, token));
    }

    /// <inheritdoc />
    /// <remarks>
    ///     A user signed up without an organization starts a new one, whose identifier follows every organization
    ///     that already has users or properties. It owns no property until one is registered for it, and the user is its
    ///     operations manager. A user added to an existing organization gets the role of the command.
    /// </remarks>
    public async Task<Result<User, IamError>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (command.Password is not { Length: >= SignUpCommand.PasswordMinLength and <= SignUpCommand.PasswordMaxLength })
            return new Result<User, IamError>.Failure(IamError.InvalidInput);

        var organizationId = command.OrganizationId ?? await NextOrganizationIdAsync(cancellationToken);
        var role = command.OrganizationId is null ? EUserRole.OperationsManager : command.Role;

        User user;
        try
        {
            user = new User(command.Username, hashingService.HashPassword(command.Password), command.FullName, role,
                organizationId);
        }
        catch (ArgumentException)
        {
            return new Result<User, IamError>.Failure(IamError.InvalidInput);
        }

        if (await userRepository.ExistsByUsernameAsync(user.Username, cancellationToken))
            return new Result<User, IamError>.Failure(IamError.UsernameAlreadyTaken);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<User, IamError>.Success(user);
    }

    private async Task<int> NextOrganizationIdAsync(CancellationToken cancellationToken)
    {
        var latestUserOrganizationId = await userRepository.FindLatestOrganizationIdAsync(cancellationToken);
        var latestPropertyOrganizationId = await externalPropertyService.FetchLatestOrganizationIdAsync(cancellationToken);
        return Math.Max(latestUserOrganizationId, latestPropertyOrganizationId) + 1;
    }
}
