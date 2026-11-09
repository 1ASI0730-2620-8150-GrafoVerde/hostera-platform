using HosteraBackend.API.AccessControl.Application.Errors;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.AccessControl.Application.CommandServices;

/// <summary>
///     Represents the credential command service of the Access Control bounded context.
/// </summary>
public interface ICredentialCommandService
{
    /// <summary>
    ///     Handles the issue guest key cards command.
    /// </summary>
    /// <param name="command">The <see cref="IssueGuestKeyCardsCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued credentials, or the <see cref="CredentialError" /> that prevented it.</returns>
    Task<Result<IEnumerable<Credential>, CredentialError>> Handle(IssueGuestKeyCardsCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the issue staff credential command.
    /// </summary>
    /// <param name="command">The <see cref="IssueStaffCredentialCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued <see cref="Credential" />, or the <see cref="CredentialError" /> that prevented it.</returns>
    Task<Result<Credential, CredentialError>> Handle(IssueStaffCredentialCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the replace credential command.
    /// </summary>
    /// <param name="command">The <see cref="ReplaceCredentialCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The replacement <see cref="Credential" />, or the <see cref="CredentialError" /> that prevented it.</returns>
    Task<Result<Credential, CredentialError>> Handle(ReplaceCredentialCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the revoke credential command.
    /// </summary>
    /// <param name="command">The <see cref="RevokeCredentialCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The revoked <see cref="Credential" />, or the <see cref="CredentialError" /> that prevented it.</returns>
    Task<Result<Credential, CredentialError>> Handle(RevokeCredentialCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the end guest key cards command.
    /// </summary>
    /// <param name="command">The <see cref="EndGuestKeyCardsCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key cards that stopped opening doors, or the <see cref="CredentialError" /> that prevented it.</returns>
    Task<Result<IEnumerable<Credential>, CredentialError>> Handle(EndGuestKeyCardsCommand command,
        CancellationToken cancellationToken);
}
