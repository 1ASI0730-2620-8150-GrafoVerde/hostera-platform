namespace HosteraBackend.API.Bookings.Domain.Model.ReadModels;

/// <summary>
///     A rate plan that can price a booking, as the Bookings context reads it from Rooms.
/// </summary>
/// <param name="Id">The identifier of the rate plan.</param>
/// <param name="PropertyId">The identifier of the property that owns the rate plan.</param>
/// <param name="IsActive">Whether new bookings can use the rate plan.</param>
/// <param name="SoldRoomTypeIds">The room types sold under the plan, or null when it sells every room type.</param>
public record BookableRatePlan(int Id, int PropertyId, bool IsActive, IReadOnlyList<int>? SoldRoomTypeIds)
{
    /// <summary>
    ///     Indicates whether the plan sells a room type.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <returns>True when the plan sells every room type or lists this one.</returns>
    public bool Sells(int roomTypeId) => SoldRoomTypeIds is null || SoldRoomTypeIds.Contains(roomTypeId);
}
