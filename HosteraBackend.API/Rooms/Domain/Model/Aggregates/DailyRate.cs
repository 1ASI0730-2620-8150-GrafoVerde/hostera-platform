namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents the price of one night of a room type under a rate plan.
/// </summary>
/// <remarks>
///     A daily rate replaces the base nightly rate of the room type on that date.
///     There is at most one daily rate for each rate plan, room type and date.
/// </remarks>
public partial class DailyRate
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected DailyRate()
    {
    }

    /// <summary>
    ///     Creates the price of one night.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="date">The night the price applies to.</param>
    /// <param name="amount">The price of the night. Must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an identifier or the amount is not positive.</exception>
    public DailyRate(int propertyId, int ratePlanId, int roomTypeId, DateOnly date, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ratePlanId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        PropertyId = propertyId;
        RatePlanId = ratePlanId;
        RoomTypeId = roomTypeId;
        Date = date;
        Amount = amount;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this daily rate.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the rate plan.
    /// </summary>
    public int RatePlanId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room type.
    /// </summary>
    public int RoomTypeId { get; private set; }

    /// <summary>
    ///     Gets the night the price applies to.
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    ///     Gets the price of the night.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    ///     Changes the price of the night.
    /// </summary>
    /// <param name="amount">The new price. Must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the amount is not positive.</exception>
    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Amount = amount;
    }
}
