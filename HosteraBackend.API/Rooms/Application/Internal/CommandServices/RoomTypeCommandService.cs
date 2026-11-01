using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.CommandServices;

/// <summary>
///     Represents the room type command service of the Rooms bounded context.
/// </summary>
/// <param name="roomTypeRepository">
///     The <see cref="IRoomTypeRepository" /> to use.
/// </param>
/// <param name="propertyRepository">
///     The <see cref="IPropertyRepository" /> to use.
/// </param>
/// <param name="roomRepository">
///     The <see cref="IRoomRepository" /> to use.
/// </param>
/// <param name="ratePlanRepository">
///     The <see cref="IRatePlanRepository" /> to use.
/// </param>
/// <param name="dailyRateRepository">
///     The <see cref="IDailyRateRepository" /> to use.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class RoomTypeCommandService(
    IRoomTypeRepository roomTypeRepository,
    IPropertyRepository propertyRepository,
    IRoomRepository roomRepository,
    IRatePlanRepository ratePlanRepository,
    IDailyRateRepository dailyRateRepository,
    IUnitOfWork unitOfWork)
    : IRoomTypeCommandService
{
    /// <inheritdoc />
    public async Task<Result<RoomType, RoomError>> Handle(CreateRoomTypeCommand command,
        CancellationToken cancellationToken)
    {
        RoomType roomType;
        try
        {
            roomType = new RoomType(command);
        }
        catch (ArgumentException)
        {
            return Failure(RoomError.InvalidInput);
        }

        if (await propertyRepository.FindByIdAsync(roomType.PropertyId, cancellationToken) is null)
            return Failure(RoomError.PropertyNotFound);

        if (await roomTypeRepository.ExistsByPropertyIdAndNameAsync(roomType.PropertyId, roomType.Name, null,
                cancellationToken))
            return Failure(RoomError.DuplicateRoomTypeName);

        await roomTypeRepository.AddAsync(roomType, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<RoomType, RoomError>.Success(roomType);
    }

    /// <inheritdoc />
    public async Task<Result<RoomType, RoomError>> Handle(UpdateRoomTypeCommand command,
        CancellationToken cancellationToken)
    {
        var roomType = await roomTypeRepository.FindByIdAsync(command.RoomTypeId, cancellationToken);
        if (roomType is null)
            return Failure(RoomError.RoomTypeNotFound);

        if (await roomTypeRepository.ExistsByPropertyIdAndNameAsync(roomType.PropertyId, command.Name?.Trim() ?? "",
                roomType.Id, cancellationToken))
            return Failure(RoomError.DuplicateRoomTypeName);

        try
        {
            roomType.Update(command);
        }
        catch (ArgumentException)
        {
            return Failure(RoomError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<RoomType, RoomError>.Success(roomType);
    }

    /// <inheritdoc />
    public async Task<Result<RoomType, RoomError>> Handle(DeleteRoomTypeCommand command,
        CancellationToken cancellationToken)
    {
        var roomType = await roomTypeRepository.FindByIdAsync(command.RoomTypeId, cancellationToken);
        if (roomType is null)
            return Failure(RoomError.RoomTypeNotFound);

        if (await roomRepository.ExistsByRoomTypeIdAsync(roomType.Id, cancellationToken)
            || await ratePlanRepository.ExistsByRoomTypeIdAsync(roomType.Id, cancellationToken))
            return Failure(RoomError.RoomTypeInUse);

        // Plans that sell every room type may still price this one, and those prices go with it
        foreach (var dailyRate in await dailyRateRepository.FindByRoomTypeIdAsync(roomType.Id, cancellationToken))
            dailyRateRepository.Remove(dailyRate);

        roomTypeRepository.Remove(roomType);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<RoomType, RoomError>.Success(roomType);
    }

    private static Result<RoomType, RoomError> Failure(RoomError error) =>
        new Result<RoomType, RoomError>.Failure(error);
}
