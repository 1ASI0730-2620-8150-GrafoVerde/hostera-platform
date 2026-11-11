using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The room type repository contract.
/// </summary>
public interface IRoomTypeRepository : IBaseRepository<RoomType>
{
    /// <summary>
    ///     Find the room types of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the room types of the property, or empty otherwise.
    /// </returns>
    Task<IEnumerable<RoomType>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether another room type of the property already uses the name
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="name">The name to search.</param>
    /// <param name="excludedRoomTypeId">The identifier of a room type to ignore, or null to check them all.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when the name is already used.</returns>
    Task<bool> ExistsByPropertyIdAndNameAsync(int propertyId, string name, int? excludedRoomTypeId = null,
        CancellationToken cancellationToken = default);
}
