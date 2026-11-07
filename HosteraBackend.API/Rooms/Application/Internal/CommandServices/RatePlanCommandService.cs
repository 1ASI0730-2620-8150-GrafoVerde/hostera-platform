using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.CommandServices;

/// <summary>
///     Represents the rate plan command service of the Rooms bounded context.
/// </summary>
/// <param name="ratePlanRepository">
///     The <see cref="IRatePlanRepository" /> to use.
/// </param>
/// <param name="dailyRateRepository">
///     The <see cref="IDailyRateRepository" /> to use.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class RatePlanCommandService(
    IRatePlanRepository ratePlanRepository,
    IDailyRateRepository dailyRateRepository,
    IUnitOfWork unitOfWork)
    : IRatePlanCommandService
{
    private const int MaxNightsPerRange = 366;

    /// <inheritdoc />
    public async Task<Result<RatePlan, RateError>> Handle(CreateRatePlanCommand command,
        CancellationToken cancellationToken)
    {
        RatePlan ratePlan;
        try
        {
            ratePlan = new RatePlan(command);
        }
        catch (ArgumentException)
        {
            return Failure(RateError.InvalidInput);
        }

        if (await ratePlanRepository.ExistsByPropertyIdAndNameAsync(ratePlan.PropertyId, ratePlan.Name, null,
                cancellationToken))
            return Failure(RateError.DuplicateRatePlanName);

        await ratePlanRepository.AddAsync(ratePlan, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<RatePlan, RateError>.Success(ratePlan);
    }

    /// <inheritdoc />
    public async Task<Result<RatePlan, RateError>> Handle(UpdateRatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var ratePlan = await ratePlanRepository.FindByIdAsync(command.RatePlanId, cancellationToken);
        if (ratePlan is null)
            return Failure(RateError.RatePlanNotFound);

        if (await ratePlanRepository.ExistsByPropertyIdAndNameAsync(ratePlan.PropertyId, command.Name?.Trim() ?? "",
                ratePlan.Id, cancellationToken))
            return Failure(RateError.DuplicateRatePlanName);

        try
        {
            ratePlan.Update(command);
        }
        catch (ArgumentException)
        {
            return Failure(RateError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<RatePlan, RateError>.Success(ratePlan);
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<DailyRate>, RateError>> Handle(SetDailyRatesCommand command,
        CancellationToken cancellationToken)
    {
        var nights = command.EndDate.DayNumber - command.StartDate.DayNumber + 1;
        if (nights < 1 || nights > MaxNightsPerRange)
            return new Result<IEnumerable<DailyRate>, RateError>.Failure(RateError.InvalidDateRange);

        if (command.Amount <= 0)
            return new Result<IEnumerable<DailyRate>, RateError>.Failure(RateError.InvalidInput);

        var ratePlan = await ratePlanRepository.FindByIdAsync(command.RatePlanId, cancellationToken);
        if (ratePlan is null)
            return new Result<IEnumerable<DailyRate>, RateError>.Failure(RateError.RatePlanNotFound);

        if (!ratePlan.AppliesTo(command.RoomTypeId))
            return new Result<IEnumerable<DailyRate>, RateError>.Failure(RateError.RoomTypeNotInRatePlan);

        var existing = (await dailyRateRepository.FindByRatePlanAndRoomTypeAsync(ratePlan.Id, command.RoomTypeId,
            command.StartDate, command.EndDate, cancellationToken)).ToDictionary(rate => rate.Date);

        var dailyRates = new List<DailyRate>();
        try
        {
            for (var date = command.StartDate; date <= command.EndDate; date = date.AddDays(1))
            {
                existing.TryGetValue(date, out var dailyRate);

                if (command.Amount is not { } amount)
                {
                    if (dailyRate is not null) dailyRateRepository.Remove(dailyRate);
                    continue;
                }

                if (dailyRate is null)
                {
                    dailyRate = new DailyRate(ratePlan.PropertyId, ratePlan.Id, command.RoomTypeId, date, amount);
                    await dailyRateRepository.AddAsync(dailyRate, cancellationToken);
                }
                else
                {
                    dailyRate.ChangeAmount(amount);
                }

                dailyRates.Add(dailyRate);
            }
        }
        catch (ArgumentException)
        {
            return new Result<IEnumerable<DailyRate>, RateError>.Failure(RateError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<IEnumerable<DailyRate>, RateError>.Success(dailyRates);
    }

    private static Result<RatePlan, RateError> Failure(RateError error) =>
        new Result<RatePlan, RateError>.Failure(error);
}
