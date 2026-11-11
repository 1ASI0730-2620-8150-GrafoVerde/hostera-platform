using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Application.Errors;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Application.Internal.CommandServices;

/// <summary>
///     Represents the credential command service of the Access Control bounded context.
/// </summary>
/// <param name="credentialRepository">
///     The <see cref="ICredentialRepository" /> to use.
/// </param>
/// <param name="staffMemberRepository">
///     The <see cref="IStaffMemberRepository" /> used to find the holder of a staff credential.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class CredentialCommandService(
    ICredentialRepository credentialRepository,
    IStaffMemberRepository staffMemberRepository,
    IUnitOfWork unitOfWork)
    : ICredentialCommandService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Credential>, CredentialError>> Handle(IssueGuestKeyCardsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CardIds.Count == 0)
            return new Result<IEnumerable<Credential>, CredentialError>.Failure(CredentialError.InvalidInput);

        if (command.CardIds.Distinct().Count() != command.CardIds.Count)
            return new Result<IEnumerable<Credential>, CredentialError>.Failure(CredentialError.DuplicateCardId);

        foreach (var cardId in command.CardIds)
            if (await IsCardInUseAsync(command.PropertyId, cardId, cancellationToken))
                return new Result<IEnumerable<Credential>, CredentialError>.Failure(CredentialError.DuplicateCardId);

        List<Credential> credentials;
        try
        {
            credentials = command.CardIds.Select(cardId => new Credential(command, cardId)).ToList();
        }
        catch (ArgumentException)
        {
            return new Result<IEnumerable<Credential>, CredentialError>.Failure(CredentialError.InvalidInput);
        }

        foreach (var credential in credentials)
            await credentialRepository.AddAsync(credential, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<IEnumerable<Credential>, CredentialError>.Success(credentials);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Only the key cards that still open doors at that moment change; revoked or expired cards keep their history.
    ///     The changes are saved even when no key card changes, together with the other pending changes of the request,
    ///     such as the check-out of the booking.
    /// </remarks>
    public async Task<Result<IEnumerable<Credential>, CredentialError>> Handle(EndGuestKeyCardsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookingId <= 0)
            return new Result<IEnumerable<Credential>, CredentialError>.Failure(CredentialError.InvalidInput);

        var keyCards = await credentialRepository.FindByBookingIdAsync(command.BookingId, cancellationToken);
        var endedKeyCards = keyCards.Where(keyCard => keyCard.IsUsableAt(command.EndAt)).ToList();
        foreach (var keyCard in endedKeyCards)
            keyCard.EndAt(command.EndAt);

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<IEnumerable<Credential>, CredentialError>.Success(endedKeyCards);
    }

    /// <inheritdoc />
    public async Task<Result<Credential, CredentialError>> Handle(IssueStaffCredentialCommand command,
        CancellationToken cancellationToken)
    {
        var staffMember = await staffMemberRepository.FindByIdAsync(command.StaffMemberId, cancellationToken);
        if (staffMember is null || staffMember.PropertyId != command.PropertyId)
            return Failure(CredentialError.StaffMemberNotFound);

        var now = DateTimeOffset.UtcNow;
        var staffCredentials = await credentialRepository.FindByStaffMemberIdAsync(staffMember.Id, cancellationToken);
        if (staffCredentials.Any(credential => credential.IsUsableAt(now)))
            return Failure(CredentialError.StaffCredentialExists);

        if (await IsCardInUseAsync(command.PropertyId, command.CardId, cancellationToken))
            return Failure(CredentialError.DuplicateCardId);

        Credential staffCredential;
        try
        {
            staffCredential = new Credential(command, staffMember);
        }
        catch (ArgumentException)
        {
            return Failure(CredentialError.InvalidInput);
        }

        await credentialRepository.AddAsync(staffCredential, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Credential, CredentialError>.Success(staffCredential);
    }

    /// <inheritdoc />
    public async Task<Result<Credential, CredentialError>> Handle(ReplaceCredentialCommand command,
        CancellationToken cancellationToken)
    {
        var credential = await credentialRepository.FindByIdAsync(command.CredentialId, cancellationToken);
        if (credential is null)
            return Failure(CredentialError.CredentialNotFound);

        if (await IsCardInUseAsync(credential.PropertyId, command.CardId, cancellationToken))
            return Failure(CredentialError.DuplicateCardId);

        Credential replacement;
        try
        {
            replacement = credential.ReplaceWith(command.CardId, command.OperatorId);
        }
        catch (ArgumentException)
        {
            return Failure(CredentialError.InvalidInput);
        }
        catch (InvalidOperationException)
        {
            return Failure(CredentialError.CredentialNotUsable);
        }

        credentialRepository.Update(credential);
        await credentialRepository.AddAsync(replacement, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Credential, CredentialError>.Success(replacement);
    }

    /// <inheritdoc />
    public async Task<Result<Credential, CredentialError>> Handle(RevokeCredentialCommand command,
        CancellationToken cancellationToken)
    {
        var credential = await credentialRepository.FindByIdAsync(command.CredentialId, cancellationToken);
        if (credential is null)
            return Failure(CredentialError.CredentialNotFound);

        try
        {
            credential.Revoke(command.Reason, command.Note, command.OperatorId);
        }
        catch (ArgumentException)
        {
            return Failure(CredentialError.InvalidInput);
        }
        catch (InvalidOperationException)
        {
            return Failure(CredentialError.CredentialNotUsable);
        }

        credentialRepository.Update(credential);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Credential, CredentialError>.Success(credential);
    }

    private async Task<bool> IsCardInUseAsync(int propertyId, CardId cardId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var credentials =
            await credentialRepository.FindByPropertyIdAndCardIdAsync(propertyId, cardId, cancellationToken);
        return credentials.Any(credential => credential.IsUsableAt(now));
    }

    private static Result<Credential, CredentialError> Failure(CredentialError error) =>
        new Result<Credential, CredentialError>.Failure(error);
}
