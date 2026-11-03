using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The room repository contract.
/// </summary>
public interface IRoomRepository : IBaseRepository<Room>
{
    /// <summary>
    ///     Find the rooms of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the rooms of the property ordered by number, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Room>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether another room of the property already uses the number
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="number">The number to search.</param>
    /// <param name="excludedRoomId">The identifier of a room to ignore, or null to check them all.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when the number is already used.</returns>
    Task<bool> ExistsByPropertyIdAndNumberAsync(int propertyId, string number, int? excludedRoomId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether any room belongs to the room type
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when at least one room uses the room type.</returns>
    Task<bool> ExistsByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
}
