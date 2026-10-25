using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Inventory.Application.CommandServices;

/// <summary>
///     Represents the inventory item command service of the Inventory bounded context.
/// </summary>
public interface IInventoryItemCommandService
{
    /// <summary>
    ///     Handles the create inventory item command.
    /// </summary>
    /// <param name="command">The <see cref="CreateInventoryItemCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="InventoryItem" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<InventoryItem, InventoryError>> Handle(CreateInventoryItemCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update inventory item command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateInventoryItemCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="InventoryItem" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<InventoryItem, InventoryError>> Handle(UpdateInventoryItemCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the adjust stock command.
    /// </summary>
    /// <param name="command">The <see cref="AdjustStockCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The adjusted <see cref="InventoryItem" />, or the <see cref="InventoryError" /> that prevented it.</returns>
    Task<Result<InventoryItem, InventoryError>> Handle(AdjustStockCommand command,
        CancellationToken cancellationToken);
}
