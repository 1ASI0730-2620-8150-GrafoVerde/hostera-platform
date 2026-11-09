using HosteraBackend.API.Shared.Application.Internal.OutboundServices;

namespace HosteraBackend.API.Shared.Infrastructure.Time;

/// <summary>
///     Clock that reads the system time and converts it to the time zone of the properties.
/// </summary>
/// <param name="timeProvider">The <see cref="TimeProvider" /> that supplies the current UTC time.</param>
/// <param name="configuration">The configuration that holds the <c>Hostera:TimeZone</c> setting.</param>
public class PropertyClock(TimeProvider timeProvider, IConfiguration configuration) : IClock
{
    /// <summary>
    ///     The time zone used when the configuration does not set one.
    /// </summary>
    public const string DefaultTimeZone = "America/Lima";

    private readonly TimeZoneInfo _timeZone =
        TimeZoneInfo.FindSystemTimeZoneById(configuration["Hostera:TimeZone"] ?? DefaultTimeZone);

    /// <inheritdoc />
    public DateTimeOffset Now => timeProvider.GetUtcNow();

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(Now, _timeZone).DateTime);

    /// <inheritdoc />
    public DateTimeOffset AtLocalTime(DateOnly date, TimeOnly time)
    {
        var localTime = date.ToDateTime(time, DateTimeKind.Unspecified);
        return new DateTimeOffset(localTime, _timeZone.GetUtcOffset(localTime));
    }
}
