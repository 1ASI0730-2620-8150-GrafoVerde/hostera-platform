using System.Text.RegularExpressions;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Converts the Access Control enums to and from the kebab-case names used by the REST API, such as "lost-card".
/// </summary>
public static partial class AccessControlEnumNames
{
    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex WordStartRegex();

    /// <summary>
    ///     Gets the kebab-case name of an enum value.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>The kebab-case name, for example "guest-key-card".</returns>
    public static string ToName<TEnum>(TEnum value) where TEnum : struct, Enum =>
        WordStartRegex().Replace(value.ToString(), "-$1").ToLowerInvariant();

    /// <summary>
    ///     Parses a kebab-case name into an enum value.
    /// </summary>
    /// <param name="name">The name to parse, such as "lost-card".</param>
    /// <param name="field">The name of the field, used in the error message.</param>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>The matching enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when the name does not match any value of the enum.</exception>
    public static TEnum Parse<TEnum>(string? name, string field) where TEnum : struct, Enum
    {
        var normalized = name?.Replace("-", string.Empty);
        if (string.IsNullOrWhiteSpace(normalized)
            || normalized.Any(char.IsDigit)
            || !Enum.TryParse<TEnum>(normalized, true, out var value))
            throw new ArgumentException($"'{name}' is not a valid {field}.", field);

        return value;
    }
}
