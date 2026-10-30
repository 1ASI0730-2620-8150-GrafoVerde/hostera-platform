namespace HosteraBackend.API.Rooms.Domain.Model.Entities;

/// <summary>
///     Represents a room type sold under a rate plan.
/// </summary>
/// <remarks>
///     This entity belongs to the RatePlan aggregate and only changes through it.
/// </remarks>
public class RatePlanRoomType
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected RatePlanRoomType()
    {
    }

    /// <summary>
    ///     Creates the link between a rate plan and a room type.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the identifier is not positive.</exception>
    public RatePlanRoomType(int roomTypeId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        RoomTypeId = roomTypeId;
    }

    /// <summary>
    ///     Gets the identifier of the rate plan.
    /// </summary>
    public int RatePlanId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room type.
    /// </summary>
    public int RoomTypeId { get; private set; }
}
