using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.CommandServices;

/// <summary>
///     Represents the room command service of the Rooms bounded context.
/// </summary>
/// <param name="roomRepository">
///     The <see cref="IRoomRepository" /> to use.
/// </param>
/// <param name="roomTypeRepository">
///     The <see cref="IRoomTypeRepository" /> to use.
/// </param>
/// <param name="propertyRepository">
///     The <see cref="IPropertyRepository" /> to use.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class RoomCommandService(
    IRoomRepository roomRepository,
    IRoomTypeRepository roomTypeRepository,
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork)
    : IRoomCommandService
{
    /// <inheritdoc />
    public async Task<Result<Room, RoomError>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        Room room;
        try
        {
            room = new Room(command);
        }
        catch (ArgumentException)
        {
            return Failure(RoomError.InvalidInput);
        }

        if (await propertyRepository.FindByIdAsync(room.PropertyId, cancellationToken) is null)
            return Failure(RoomError.PropertyNotFound);

        var roomTypeError = await ValidateRoomTypeAsync(room.PropertyId, room.RoomTypeId, null, cancellationToken);
        if (roomTypeError is { } error)
            return Failure(error);

        if (await roomRepository.ExistsByPropertyIdAndNumberAsync(room.PropertyId, room.Number, null,
                cancellationToken))
            return Failure(RoomError.DuplicateRoomNumber);

        await roomRepository.AddAsync(room, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Room, RoomError>.Success(room);
    }

    /// <inheritdoc />
    public async Task<Result<Room, RoomError>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = await roomRepository.FindByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Failure(RoomError.RoomNotFound);

        var roomTypeError =
            await ValidateRoomTypeAsync(room.PropertyId, command.RoomTypeId, room.RoomTypeId, cancellationToken);
        if (roomTypeError is { } error)
            return Failure(error);

        if (await roomRepository.ExistsByPropertyIdAndNumberAsync(room.PropertyId, command.Number?.Trim() ?? "",
                room.Id, cancellationToken))
            return Failure(RoomError.DuplicateRoomNumber);

        try
        {
            room.Update(command);
        }
        catch (ArgumentException)
        {
            return Failure(RoomError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Room, RoomError>.Success(room);
    }

    /// <summary>
    ///     Checks that a room of the property can belong to the room type.
    /// </summary>
    /// <remarks>
    ///     An inactive room type only stays valid for a room that already belongs to it.
    /// </remarks>
    private async Task<RoomError?> ValidateRoomTypeAsync(int propertyId, int roomTypeId, int? currentRoomTypeId,
        CancellationToken cancellationToken)
    {
        var roomType = await roomTypeRepository.FindByIdAsync(roomTypeId, cancellationToken);
        if (roomType is null)
            return RoomError.RoomTypeNotFound;

        if (roomType.PropertyId != propertyId)
            return RoomError.RoomTypeFromAnotherProperty;

        if (!roomType.IsActive && roomType.Id != currentRoomTypeId)
            return RoomError.InactiveRoomType;

        return null;
    }

    private static Result<Room, RoomError> Failure(RoomError error) =>
        new Result<Room, RoomError>.Failure(error);
}
