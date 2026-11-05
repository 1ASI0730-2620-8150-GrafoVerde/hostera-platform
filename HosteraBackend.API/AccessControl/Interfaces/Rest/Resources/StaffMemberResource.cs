namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Staff member resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the staff member</param>
/// <param name="PropertyId">The identifier of the property where the staff member works</param>
/// <param name="Name">The name of the staff member</param>
/// <param name="Role">The role of the staff member</param>
public record StaffMemberResource(int Id, int PropertyId, string Name, string Role);
