namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the unique identifier printed on an RFID key card.
/// </summary>
public sealed record CardNumber
{
    private const int MaxLength = 32;

    /// <summary>
    ///     Initializes a new <see cref="CardNumber"/> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw card number. Must be non-null, non-whitespace, and at most 32 characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, whitespace, or exceeds the maximum length.</exception>
    public CardNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CardNumber cannot be null, empty, or whitespace.", nameof(value));

        if (value.Trim().Length > MaxLength)
            throw new ArgumentException($"CardNumber cannot be longer than {MaxLength} characters.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
