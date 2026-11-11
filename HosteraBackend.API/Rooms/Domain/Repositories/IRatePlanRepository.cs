using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The rate plan repository contract.
/// </summary>
public interface IRatePlanRepository : IBaseRepository<RatePlan>
{
    /// <summary>
    ///     Find the rate plans of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the rate plans of the property, or empty otherwise.
    /// </returns>
    Task<IEnumerable<RatePlan>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether another rate plan of the property already uses the name
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="name">The name to search.</param>
    /// <param name="excludedRatePlanId">The identifier of a plan to ignore, or null to check them all.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when the name is already used.</returns>
    Task<bool> ExistsByPropertyIdAndNameAsync(int propertyId, string name, int? excludedRatePlanId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether any rate plan lists the room type
    /// </summary>
    /// <remarks>
    ///     Plans that apply to every room type do not list them, so they do not count.
    /// </remarks>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when at least one rate plan lists the room type.</returns>
    Task<bool> ExistsByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
}
