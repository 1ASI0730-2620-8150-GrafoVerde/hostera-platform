namespace HosteraBackend.API.Rooms.Interfaces.ACL;

/// <summary>
///     The data of a rate plan shared with other bounded contexts.
/// </summary>
/// <param name="Id">The identifier of the rate plan.</param>
/// <param name="PropertyId">The identifier of the property that owns the rate plan.</param>
/// <param name="IsActive">Whether new bookings can use the rate plan.</param>
/// <param name="AppliesToAllRoomTypes">Whether the plan sells every room type of the property.</param>
/// <param name="RoomTypeIds">The room types the plan sells when it does not sell them all.</param>
public record RatePlanSummary(
    int Id,
    int PropertyId,
    bool IsActive,
    bool AppliesToAllRoomTypes,
    IReadOnlyList<int> RoomTypeIds)
{
    /// <summary>
    ///     Indicates whether a room type is sold under the plan.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <returns>True when the plan sells every room type or lists this one.</returns>
    public bool AppliesTo(int roomTypeId) => AppliesToAllRoomTypes || RoomTypeIds.Contains(roomTypeId);
}
