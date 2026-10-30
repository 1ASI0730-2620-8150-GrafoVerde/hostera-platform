using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Represents the inventory item command service of the Inventory bounded context.
/// </summary>
/// <param name="inventoryItemRepository">
///     The <see cref="IInventoryItemRepository" /> to use.
/// </param>
/// <param name="storageLocationRepository">
///     The <see cref="IStorageLocationRepository" /> used to validate the locations of the item.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class InventoryItemCommandService(
    IInventoryItemRepository inventoryItemRepository,
    IStorageLocationRepository storageLocationRepository,
    IUnitOfWork unitOfWork)
    : IInventoryItemCommandService
{
    /// <inheritdoc />
    public async Task<Result<InventoryItem, InventoryError>> Handle(CreateInventoryItemCommand command,
        CancellationToken cancellationToken)
    {
        var locationError =
            await ValidateLocationAsync(command.PrimaryLocationId, command.PropertyId, cancellationToken);
        if (locationError is { } error)
            return Failure(error);

        if (await inventoryItemRepository.ExistsByPropertyIdAndCodeAsync(command.PropertyId, command.Code,
                cancellationToken))
            return Failure(InventoryError.DuplicateInventoryItemCode);

        InventoryItem inventoryItem;
        try
        {
            inventoryItem = new InventoryItem(command);
        }
        catch (ArgumentException)
        {
            return Failure(InventoryError.InvalidInput);
        }

        await inventoryItemRepository.AddAsync(inventoryItem, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InventoryItem, InventoryError>.Success(inventoryItem);
    }

    /// <inheritdoc />
    public async Task<Result<InventoryItem, InventoryError>> Handle(UpdateInventoryItemCommand command,
        CancellationToken cancellationToken)
    {
        var inventoryItem = await inventoryItemRepository.FindByIdAsync(command.InventoryItemId, cancellationToken);
        if (inventoryItem is null)
            return Failure(InventoryError.InventoryItemNotFound);

        var locationError =
            await ValidateLocationAsync(command.PrimaryLocationId, inventoryItem.PropertyId, cancellationToken);
        if (locationError is { } error)
            return Failure(error);

        try
        {
            inventoryItem.Update(command);
        }
        catch (InvalidOperationException)
        {
            return Failure(InventoryError.UnitChangeNotAllowed);
        }
        catch (ArgumentException)
        {
            return Failure(InventoryError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InventoryItem, InventoryError>.Success(inventoryItem);
    }

    /// <inheritdoc />
    public async Task<Result<InventoryItem, InventoryError>> Handle(AdjustStockCommand command,
        CancellationToken cancellationToken)
    {
        var inventoryItem = await inventoryItemRepository.FindByIdAsync(command.InventoryItemId, cancellationToken);
        if (inventoryItem is null)
            return Failure(InventoryError.InventoryItemNotFound);

        var sourceError =
            await ValidateLocationAsync(command.StorageLocationId, inventoryItem.PropertyId, cancellationToken);
        if (sourceError is { } source)
            return Failure(source);

        if (command.Operation == EStockOperation.Transfer && command.DestinationStorageLocationId is { } destinationId)
        {
            var destinationError =
                await ValidateLocationAsync(destinationId, inventoryItem.PropertyId, cancellationToken);
            if (destinationError is { } destination)
                return Failure(destination);
        }

        if (!command.Reason.IsValidFor(command.Operation) ||
            (command.Reason == EStockAdjustmentReason.Other && string.IsNullOrWhiteSpace(command.Note)))
            return Failure(InventoryError.InvalidAdjustmentReason);

        if (command.Operation is EStockOperation.Out or EStockOperation.Transfer &&
            command.Quantity > inventoryItem.QuantityAt(command.StorageLocationId))
            return Failure(InventoryError.InsufficientStock);

        try
        {
            inventoryItem.AdjustStock(command, DateTimeOffset.UtcNow);
        }
        catch (InvalidOperationException)
        {
            return Failure(InventoryError.InsufficientStock);
        }
        catch (ArgumentException)
        {
            return Failure(InventoryError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InventoryItem, InventoryError>.Success(inventoryItem);
    }

    /// <inheritdoc />
    public async Task<Result<InventoryItem, InventoryError>> Handle(UnassignStorageLocationCommand command,
        CancellationToken cancellationToken)
    {
        var inventoryItem = await inventoryItemRepository.FindByIdAsync(command.InventoryItemId, cancellationToken);
        if (inventoryItem is null)
            return Failure(InventoryError.InventoryItemNotFound);

        if (!inventoryItem.IsAssignedTo(command.StorageLocationId))
            return Failure(InventoryError.StorageLocationNotAssigned);

        try
        {
            inventoryItem.UnassignStorageLocation(command);
        }
        catch (InvalidOperationException)
        {
            return Failure(InventoryError.StorageLocationAssignmentInUse);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<InventoryItem, InventoryError>.Success(inventoryItem);
    }

    private async Task<InventoryError?> ValidateLocationAsync(int storageLocationId, int propertyId,
        CancellationToken cancellationToken)
    {
        var storageLocation = await storageLocationRepository.FindByIdAsync(storageLocationId, cancellationToken);
        if (storageLocation is null)
            return InventoryError.StorageLocationNotFound;

        return storageLocation.PropertyId != propertyId
            ? InventoryError.StorageLocationFromAnotherProperty
            : null;
    }

    private static Result<InventoryItem, InventoryError> Failure(InventoryError error) =>
        new Result<InventoryItem, InventoryError>.Failure(error);
}
