namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     RFID access attempts of a property on a day
/// </summary>
/// <param name="GrantedEvents">The attempts that opened the door</param>
/// <param name="DeniedEvents">The attempts that left the door closed</param>
public record AccessSummary(int GrantedEvents, int DeniedEvents)
{
    /// <summary>
    ///     Gets every access attempt of the day.
    /// </summary>
    public int TotalEvents => GrantedEvents + DeniedEvents;
}
