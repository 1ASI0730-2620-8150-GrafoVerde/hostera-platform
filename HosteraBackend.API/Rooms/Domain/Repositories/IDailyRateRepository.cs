using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The daily rate repository contract.
/// </summary>
public interface IDailyRateRepository : IBaseRepository<DailyRate>
{
    /// <summary>
    ///     Find the daily rates of a property, optionally limited to a period
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="startDate">The first night of the period, or null for no lower limit.</param>
    /// <param name="endDate">The last night of the period, or null for no upper limit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the daily rates ordered by date, or empty otherwise.
    /// </returns>
    Task<IEnumerable<DailyRate>> FindByPropertyIdAndPeriodAsync(int propertyId, DateOnly? startDate, DateOnly? endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the daily rates of a room type under a rate plan in a period
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="startDate">The first night of the period.</param>
    /// <param name="endDate">The last night of the period.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the daily rates of the period, or empty otherwise.
    /// </returns>
    Task<IEnumerable<DailyRate>> FindByRatePlanAndRoomTypeAsync(int ratePlanId, int roomTypeId, DateOnly startDate,
        DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find every daily rate of a room type, under any rate plan
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the daily rates of the room type, or empty otherwise.
    /// </returns>
    Task<IEnumerable<DailyRate>> FindByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
}
