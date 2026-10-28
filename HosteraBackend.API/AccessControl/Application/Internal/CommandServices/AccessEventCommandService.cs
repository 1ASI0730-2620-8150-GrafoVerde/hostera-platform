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
///     Represents the access event command service of the Access Control bounded context.
/// </summary>
/// <param name="accessEventRepository">
///     The <see cref="IAccessEventRepository" /> to use.
/// </param>
/// <param name="credentialRepository">
///     The <see cref="ICredentialRepository" /> used to find the credential written to the presented card.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class AccessEventCommandService(
    IAccessEventRepository accessEventRepository,
    ICredentialRepository credentialRepository,
    IUnitOfWork unitOfWork)
    : IAccessEventCommandService
{
    /// <summary>
    ///     Margin accepted between the clock of a reader and the clock of the server.
    /// </summary>
    private static readonly TimeSpan ReaderClockTolerance = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<Result<AccessEvent, AccessEventError>> Handle(RecordAccessEventCommand command,
        CancellationToken cancellationToken)
    {
        if (command.OccurredAt > DateTimeOffset.UtcNow.Add(ReaderClockTolerance))
            return Failure(AccessEventError.InvalidInput);

        var credentials = await credentialRepository.FindByPropertyIdAndCardIdAsync(command.PropertyId,
            command.CardId, cancellationToken);
        var credential = CredentialPresentedAt(credentials, command.OccurredAt);
        if (credential is null)
            return Failure(AccessEventError.CredentialNotFound);

        AccessEvent accessEvent;
        try
        {
            accessEvent = new AccessEvent(command, credential);
        }
        catch (ArgumentException)
        {
            return Failure(AccessEventError.InvalidInput);
        }

        await accessEventRepository.AddAsync(accessEvent, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<AccessEvent, AccessEventError>.Success(accessEvent);
    }

    /// <summary>
    ///     Chooses the credential a card held at a moment.
    /// </summary>
    /// <remarks>
    ///     A card can be reused after its credential expires or is revoked, so the same card identifier can belong
    ///     to several credentials. The active one is chosen first; otherwise, the most recently issued.
    /// </remarks>
    private static Credential? CredentialPresentedAt(IEnumerable<Credential> credentials, DateTimeOffset moment)
    {
        var candidates = credentials.OrderByDescending(credential => credential.IssuedAt).ToList();
        return candidates.FirstOrDefault(credential => credential.StatusAt(moment) == ECredentialStatus.Active)
               ?? candidates.FirstOrDefault();
    }

    private static Result<AccessEvent, AccessEventError> Failure(AccessEventError error) =>
        new Result<AccessEvent, AccessEventError>.Failure(error);
}
