namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Unit in which the quantity of an inventory item is counted.
/// </summary>
public enum EInventoryUnit
{
    /// <summary>
    ///     Whole pieces, such as towels or key cards. Quantities must be whole numbers.
    /// </summary>
    Units,

    /// <summary>
    ///     Liquid volume, such as cleaning products.
    /// </summary>
    Liters,

    /// <summary>
    ///     Weight, such as detergent powder.
    /// </summary>
    Kilograms
}
