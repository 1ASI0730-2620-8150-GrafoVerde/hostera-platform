using System.Net.Mime;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/staff-members")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Staff Member Endpoints")]
public class PropertyStaffMembersController(IStaffMemberQueryService staffMemberQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the staff members of a property")]
    [EndpointDescription("Get all staff members of a property, ordered by name")]
    [ProducesResponseType(typeof(IEnumerable<StaffMemberResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaffMembersByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllStaffMembersByPropertyIdQuery(propertyId);
        var staffMembers = await staffMemberQueryService.Handle(query, cancellationToken);
        return Ok(staffMembers.Select(StaffMemberResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
