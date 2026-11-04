using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Represents the storage location command service of the Inventory bounded context.
/// </summary>
/// <param name="storageLocationRepository">
///     The <see cref="IStorageLocationRepository" /> to use.
/// </param>
/// <param name="inventoryItemRepository">
///     The <see cref="IInventoryItemRepository" /> used to check whether a location is in use.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class StorageLocationCommandService(
    IStorageLocationRepository storageLocationRepository,
    IInventoryItemRepository inventoryItemRepository,
    IUnitOfWork unitOfWork)
    : IStorageLocationCommandService
{
    /// <inheritdoc />
    public async Task<Result<StorageLocation, InventoryError>> Handle(CreateStorageLocationCommand command,
        CancellationToken cancellationToken)
    {
        if (await storageLocationRepository.ExistsByPropertyIdAndCodeAsync(command.PropertyId, command.Code,
                cancellationToken))
            return Failure(InventoryError.DuplicateStorageLocationCode);

        StorageLocation storageLocation;
        try
        {
            storageLocation = new StorageLocation(command);
        }
        catch (ArgumentException)
        {
            return Failure(InventoryError.InvalidInput);
        }

        await storageLocationRepository.AddAsync(storageLocation, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<StorageLocation, InventoryError>.Success(storageLocation);
    }

    /// <inheritdoc />
    public async Task<Result<StorageLocation, InventoryError>> Handle(UpdateStorageLocationCommand command,
        CancellationToken cancellationToken)
    {
        var storageLocation =
            await storageLocationRepository.FindByIdAsync(command.StorageLocationId, cancellationToken);
        if (storageLocation is null)
            return Failure(InventoryError.StorageLocationNotFound);

        try
        {
            storageLocation.Update(command);
        }
        catch (ArgumentException)
        {
            return Failure(InventoryError.InvalidInput);
        }

        storageLocationRepository.Update(storageLocation);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<StorageLocation, InventoryError>.Success(storageLocation);
    }

    /// <inheritdoc />
    public async Task<Result<StorageLocation, InventoryError>> Handle(DeleteStorageLocationCommand command,
        CancellationToken cancellationToken)
    {
        var storageLocation =
            await storageLocationRepository.FindByIdAsync(command.StorageLocationId, cancellationToken);
        if (storageLocation is null)
            return Failure(InventoryError.StorageLocationNotFound);

        if (await inventoryItemRepository.ExistsByStorageLocationIdAsync(storageLocation.Id, cancellationToken))
            return Failure(InventoryError.StorageLocationInUse);

        storageLocationRepository.Remove(storageLocation);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<StorageLocation, InventoryError>.Success(storageLocation);
    }

    private static Result<StorageLocation, InventoryError> Failure(InventoryError error) =>
        new Result<StorageLocation, InventoryError>.Failure(error);
}
