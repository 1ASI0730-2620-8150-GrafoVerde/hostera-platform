using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Rooms.Application.CommandServices;

/// <summary>
///     Represents the rate plan command service of the Rooms bounded context.
/// </summary>
public interface IRatePlanCommandService
{
    /// <summary>
    ///     Handles the create rate plan command.
    /// </summary>
    /// <param name="command">The <see cref="CreateRatePlanCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="RatePlan" />, or the <see cref="RateError" /> that prevented it.</returns>
    Task<Result<RatePlan, RateError>> Handle(CreateRatePlanCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update rate plan command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateRatePlanCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="RatePlan" />, or the <see cref="RateError" /> that prevented it.</returns>
    Task<Result<RatePlan, RateError>> Handle(UpdateRatePlanCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the set daily rates command.
    /// </summary>
    /// <param name="command">The <see cref="SetDailyRatesCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The daily rates of the range, or the <see cref="RateError" /> that prevented it.</returns>
    Task<Result<IEnumerable<DailyRate>, RateError>> Handle(SetDailyRatesCommand command,
        CancellationToken cancellationToken);
}
