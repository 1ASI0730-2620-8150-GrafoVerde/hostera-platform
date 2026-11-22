namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get all properties of an organization
/// </summary>
/// <param name="OrganizationId">The identifier of the organization that owns the properties</param>
public record GetAllPropertiesByOrganizationIdQuery(int OrganizationId);
