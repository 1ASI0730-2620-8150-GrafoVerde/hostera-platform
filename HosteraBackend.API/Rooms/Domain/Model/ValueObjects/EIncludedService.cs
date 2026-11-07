namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     Service a rate plan includes besides the room.
/// </summary>
public enum EIncludedService
{
    /// <summary>
    ///     Only the room.
    /// </summary>
    RoomOnly,

    /// <summary>
    ///     The room with breakfast.
    /// </summary>
    BreakfastIncluded,

    /// <summary>
    ///     The room with breakfast and one more meal.
    /// </summary>
    HalfBoard,

    /// <summary>
    ///     The room with all meals.
    /// </summary>
    FullBoard
}
