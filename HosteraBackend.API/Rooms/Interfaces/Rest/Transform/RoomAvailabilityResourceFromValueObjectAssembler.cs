using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="RoomAvailability" /> value object into a
///     <see cref="RoomAvailabilityResource" /> for REST representation.
/// </summary>
public static class RoomAvailabilityResourceFromValueObjectAssembler
{
    /// <summary>
    ///     Converts a <see cref="RoomAvailability" /> to its <see cref="RoomAvailabilityResource" />.
    /// </summary>
    /// <param name="valueObject">The <see cref="RoomAvailability" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="RoomAvailabilityResource" /> with the day statuses of the room.</returns>
    public static RoomAvailabilityResource ToResourceFromValueObject(RoomAvailability valueObject)
    {
        ArgumentNullException.ThrowIfNull(valueObject);
        var room = valueObject.Room;
        return new RoomAvailabilityResource(
            room.Id,
            room.Number,
            room.RoomTypeId,
            room.Floor,
            valueObject.Days.Select(day => new RoomDayStatusResource(day.Date, RoomsEnumNames.ToName(day.Status))));
    }
}
