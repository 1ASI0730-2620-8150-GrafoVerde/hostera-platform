using System.Globalization;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Converts the version of a booking to and from the ETag and If-Match headers of the REST API.
/// </summary>
/// <remarks>
///     A client sends back the ETag it read in If-Match so that a change made by someone else in between is rejected
///     instead of overwritten. Requests without If-Match, or with "*", skip the check.
/// </remarks>
public static class BookingETag
{
    /// <summary>
    ///     The header a client sends with the ETag of the booking it read.
    /// </summary>
    public const string IfMatchHeader = "If-Match";

    /// <summary>
    ///     Builds the ETag of a booking version.
    /// </summary>
    /// <param name="version">The version of the booking.</param>
    /// <returns>The quoted version, such as "3".</returns>
    public static string ToETag(int version) => $"\"{version.ToString(CultureInfo.InvariantCulture)}\"";

    /// <summary>
    ///     Reads the booking version that an If-Match header expects.
    /// </summary>
    /// <param name="ifMatch">The value of the If-Match header, or null.</param>
    /// <returns>The expected version, or null when the header is missing or is "*".</returns>
    /// <exception cref="ArgumentException">Thrown when the header is not the ETag of a booking.</exception>
    public static int? ParseIfMatch(string? ifMatch)
    {
        var value = ifMatch?.Trim();
        if (string.IsNullOrEmpty(value) || value == "*")
            return null;

        if (value.StartsWith("W/", StringComparison.Ordinal))
            value = value[2..];

        if (int.TryParse(value.Trim('"'), NumberStyles.None, CultureInfo.InvariantCulture, out var version))
            return version;

        throw new ArgumentException("If-Match must be the ETag of the booking, such as \"3\".", nameof(ifMatch));
    }
}
