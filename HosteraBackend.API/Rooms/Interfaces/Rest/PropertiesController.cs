using System.Net.Mime;
using HosteraBackend.API.Iam.Interfaces.ACL;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Property Endpoints")]
public class PropertiesController(
    IPropertyQueryService propertyQueryService,
    IAuthenticatedUserAccessor authenticatedUserAccessor)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all properties")]
    [EndpointDescription("Get the properties of the organization of the signed-in user, ordered by name")]
    [ProducesResponseType(typeof(IEnumerable<PropertyResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProperties(CancellationToken cancellationToken)
    {
        var organizationId = authenticatedUserAccessor.AuthenticatedUser?.OrganizationId ?? 0;
        var query = new GetAllPropertiesByOrganizationIdQuery(organizationId);
        var properties = await propertyQueryService.Handle(query, cancellationToken);
        return Ok(properties.Select(PropertyResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{propertyId:int}")]
    [EndpointSummary("Get a property by ID")]
    [EndpointDescription("Get a property by its unique identifier")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertyById([FromRoute] int propertyId, CancellationToken cancellationToken)
    {
        var property = await propertyQueryService.Handle(new GetPropertyByIdQuery(propertyId), cancellationToken);

        return property is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "PropertyNotFound",
                detail: "The specified property was not found.")
            : Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
    }
}
