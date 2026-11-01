using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="SetRoomStatusResource" /> into a
///     <see cref="SetRoomStatusCommand" />.
/// </summary>
public static class SetRoomStatusCommandFromResourceAssembler
{
    private const string AvailableStatus = "available";

    /// <summary>
    ///     Converts a <see cref="SetRoomStatusResource" /> to a <see cref="SetRoomStatusCommand" />.
    /// </summary>
    /// <remarks>
    ///     The "available" status becomes a null status, which releases the days of the range.
    /// </remarks>
    /// <param name="resource">The resource with the status and its range. Must not be null.</param>
    /// <param name="roomId">The identifier of the room.</param>
    /// <returns>A new <see cref="SetRoomStatusCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the status is not valid.</exception>
    public static SetRoomStatusCommand ToCommandFromResource(SetRoomStatusResource resource, int roomId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        EOperationalStatus? status = string.Equals(resource.Status?.Trim(), AvailableStatus,
            StringComparison.OrdinalIgnoreCase)
            ? null
            : RoomsEnumNames.Parse<EOperationalStatus>(resource.Status, "room status");

        return new SetRoomStatusCommand(roomId, status, resource.StartDate, resource.EndDate, resource.Reason);
    }
}
