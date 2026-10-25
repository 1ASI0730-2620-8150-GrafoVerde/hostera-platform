namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a non-negative quantity of an inventory item expressed in its unit.
/// </summary>
public sealed record StockQuantity
{
    /// <summary>
    ///     Initializes a new <see cref="StockQuantity" />.
    /// </summary>
    /// <param name="value">The quantity. Must be zero or greater.</param>
    /// <param name="unit">The unit of the item. Quantities counted in units must be whole numbers.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="value" /> is negative, or is not a whole number for <see cref="EInventoryUnit.Units" />.
    /// </exception>
    public StockQuantity(decimal value, EInventoryUnit unit)
    {
        if (value < 0)
            throw new ArgumentException("StockQuantity cannot be negative.", nameof(value));

        if (unit == EInventoryUnit.Units && value != decimal.Truncate(value))
            throw new ArgumentException("StockQuantity counted in units must be a whole number.", nameof(value));

        Value = value;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public decimal Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
