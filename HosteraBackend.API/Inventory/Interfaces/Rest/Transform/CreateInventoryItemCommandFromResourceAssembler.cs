using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateInventoryItemResource" /> into a
///     <see cref="CreateInventoryItemCommand" />.
/// </summary>
public static class CreateInventoryItemCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateInventoryItemResource" /> to a <see cref="CreateInventoryItemCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the item. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property that owns the item.</param>
    /// <returns>A new <see cref="CreateInventoryItemCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the code, the category or the unit is not valid.</exception>
    public static CreateInventoryItemCommand ToCommandFromResource(CreateInventoryItemResource resource,
        int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateInventoryItemCommand(
            propertyId,
            new InventoryCode(resource.Code),
            resource.Name,
            InventoryEnumNames.Parse<EInventoryCategory>(resource.Category, "inventory category"),
            InventoryEnumNames.Parse<EInventoryUnit>(resource.Unit, "inventory unit"),
            resource.PrimaryLocationId,
            resource.LowStockThreshold,
            resource.OpeningQuantity,
            resource.Operator);
    }
}
