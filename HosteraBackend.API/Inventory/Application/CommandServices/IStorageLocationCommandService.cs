using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Inventory.Application.CommandServices;

/// <summary>
///     Represents the storage location command service of the Inventory bounded context.
/// </summary>
public interface IStorageLocationCommandService
{
    /// <summary>
    ///     Handles the create storage location command.
    /// </summary>
    /// <param name="command">The <see cref="CreateStorageLocationCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="StorageLocation" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<StorageLocation, InventoryError>> Handle(CreateStorageLocationCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update storage location command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateStorageLocationCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="StorageLocation" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<StorageLocation, InventoryError>> Handle(UpdateStorageLocationCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the delete storage location command.
    /// </summary>
    /// <param name="command">The <see cref="DeleteStorageLocationCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deleted <see cref="StorageLocation" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<StorageLocation, InventoryError>> Handle(DeleteStorageLocationCommand command,
        CancellationToken cancellationToken);
}
