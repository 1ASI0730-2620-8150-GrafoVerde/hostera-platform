namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Source of the information of the operational dashboard, which can be unavailable on its own.
/// </summary>
public enum EDashboardSection
{
    /// <summary>
    ///     The rooms and their status periods.
    /// </summary>
    Rooms,

    /// <summary>
    ///     The bookings of the property.
    /// </summary>
    Bookings,

    /// <summary>
    ///     The inventory items and their stock.
    /// </summary>
    Inventory,

    /// <summary>
    ///     The RFID access events.
    /// </summary>
    AccessControl
}
