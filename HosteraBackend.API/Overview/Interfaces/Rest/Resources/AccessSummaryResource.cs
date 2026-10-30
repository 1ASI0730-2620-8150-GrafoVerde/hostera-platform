namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Access summary resource for REST API
/// </summary>
/// <param name="GrantedEvents">The access attempts of the day that opened the door</param>
/// <param name="DeniedEvents">The access attempts of the day that left the door closed</param>
/// <param name="TotalEvents">Every access attempt of the day</param>
public record AccessSummaryResource(int GrantedEvents, int DeniedEvents, int TotalEvents);
