namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     Status of a configuration record of the Rooms bounded context.
/// </summary>
public enum ERecordStatus
{
    /// <summary>
    ///     The record is offered for sale.
    /// </summary>
    Active,

    /// <summary>
    ///     The record is kept but not offered for sale.
    /// </summary>
    Inactive
}
