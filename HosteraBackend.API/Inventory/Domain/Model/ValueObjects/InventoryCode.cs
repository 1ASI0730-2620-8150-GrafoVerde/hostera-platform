namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the code that identifies an inventory item or a storage location within a property.
/// </summary>
public sealed record InventoryCode
{
    private const int MaxLength = 20;

    /// <summary>
    ///     Initializes a new <see cref="InventoryCode" /> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw code. Must be non-null, non-whitespace, and at most 20 characters.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="value" /> is null, empty, whitespace, or exceeds the maximum length.
    /// </exception>
    public InventoryCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("InventoryCode cannot be null, empty, or whitespace.", nameof(value));

        if (value.Trim().Length > MaxLength)
            throw new ArgumentException($"InventoryCode cannot be longer than {MaxLength} characters.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
