namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Category that groups the supplies a property keeps in stock.
/// </summary>
public enum EInventoryCategory
{
    /// <summary>
    ///     Bed sheets, towels and other textiles.
    /// </summary>
    Linen,

    /// <summary>
    ///     Toiletries and other items left in the rooms for guests.
    /// </summary>
    GuestAmenities,

    /// <summary>
    ///     Products used by housekeeping to clean rooms and common areas.
    /// </summary>
    CleaningSupplies,

    /// <summary>
    ///     Blank RFID key cards and other access supplies.
    /// </summary>
    AccessSupplies
}
