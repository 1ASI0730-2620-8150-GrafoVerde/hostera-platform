using HosteraBackend.API.AccessControl.Application.Errors;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.AccessControl.Application.CommandServices;

/// <summary>
///     Represents the access event command service of the Access Control bounded context.
/// </summary>
public interface IAccessEventCommandService
{
    /// <summary>
    ///     Handles the record access event command.
    /// </summary>
    /// <param name="command">The <see cref="RecordAccessEventCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The recorded <see cref="AccessEvent" />, or the <see cref="AccessEventError" /> that prevented it.</returns>
    Task<Result<AccessEvent, AccessEventError>> Handle(RecordAccessEventCommand command,
        CancellationToken cancellationToken);
}
