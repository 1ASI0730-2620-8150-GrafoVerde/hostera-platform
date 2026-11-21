using HosteraBackend.API.Iam.Application.CommandServices;
using HosteraBackend.API.Iam.Application.Errors;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Commands;
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
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
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
    public async Task<Result<User, IamError>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (command.Password is not { Length: >= SignUpCommand.PasswordMinLength and <= SignUpCommand.PasswordMaxLength })
            return new Result<User, IamError>.Failure(IamError.InvalidInput);

        User user;
        try
        {
            user = new User(command.Username, hashingService.HashPassword(command.Password));
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
}
