namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Kind of physical area where inventory items are stored.
/// </summary>
public enum EStorageLocationType
{
    /// <summary>
    ///     Main storage room of the property.
    /// </summary>
    Warehouse,

    /// <summary>
    ///     Small closet on a floor, usually used by housekeeping.
    /// </summary>
    Closet,

    /// <summary>
    ///     Stock kept at the front desk.
    /// </summary>
    FrontDesk,

    /// <summary>
    ///     Service area such as the laundry or the kitchen.
    /// </summary>
    ServiceArea
}
