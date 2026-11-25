using HosteraBackend.API.Iam.Application.Errors;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Iam.Application.CommandServices;

/// <summary>
///     Represents the user command service of the IAM bounded context.
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    ///     Handles the sign in command.
    /// </summary>
    /// <param name="command">The <see cref="SignInCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The signed-in <see cref="User" /> and its token, or the <see cref="IamError" /> that prevented it.</returns>
    Task<Result<(User User, string Token), IamError>> Handle(SignInCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the sign up command.
    /// </summary>
    /// <param name="command">The <see cref="SignUpCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="User" />, or the <see cref="IamError" /> that prevented it.</returns>
    Task<Result<User, IamError>> Handle(SignUpCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update user profile command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateUserProfileCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="User" />, or the <see cref="IamError" /> that prevented it.</returns>
    Task<Result<User, IamError>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken);
}
