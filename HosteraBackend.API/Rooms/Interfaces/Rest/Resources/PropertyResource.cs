namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Property resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the property</param>
/// <param name="OrganizationId">The identifier of the organization that owns the property</param>
/// <param name="Name">The name of the property</param>
/// <param name="City">The city of the property</param>
/// <param name="Currency">The three-letter code of the currency of the property</param>
public record PropertyResource(int Id, int OrganizationId, string Name, string City, string Currency);
