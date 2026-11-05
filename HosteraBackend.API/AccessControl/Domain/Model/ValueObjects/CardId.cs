using System.Text.RegularExpressions;

namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the four-character hexadecimal identifier written to an RFID card.
/// </summary>
public sealed partial record CardId
{
    /// <summary>
    ///     Initializes a new <see cref="CardId"/> with the given raw string value.
    /// </summary>
    /// <param name="value">The raw card identifier. Must be four hexadecimal characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, whitespace, or not four hexadecimal characters.</exception>
    public CardId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CardId cannot be null, empty, or whitespace.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (!HexadecimalFormat().IsMatch(normalized))
            throw new ArgumentException("CardId must be four hexadecimal characters.", nameof(value));

        Value = normalized;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    [GeneratedRegex("^[0-9A-F]{4}$")]
    private static partial Regex HexadecimalFormat();
}
