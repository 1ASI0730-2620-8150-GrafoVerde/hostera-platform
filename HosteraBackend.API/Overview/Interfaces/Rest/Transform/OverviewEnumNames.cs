using System.Text.RegularExpressions;

namespace HosteraBackend.API.Overview.Interfaces.Rest.Transform;

/// <summary>
///     Converts the Overview enums to the kebab-case names used by the REST API, such as "needs-cleaning".
/// </summary>
public static partial class OverviewEnumNames
{
    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex WordStartRegex();

    /// <summary>
    ///     Gets the kebab-case name of an enum value.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>The kebab-case name, for example "out-of-service".</returns>
    public static string ToName<TEnum>(TEnum value) where TEnum : struct, Enum =>
        WordStartRegex().Replace(value.ToString(), "-$1").ToLowerInvariant();
}
