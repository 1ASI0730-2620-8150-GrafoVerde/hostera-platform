using System.Net.Mime;
using HosteraBackend.API.Iam.Application.QueryServices;
using HosteraBackend.API.Iam.Domain.Model.Queries;
using HosteraBackend.API.Iam.Interfaces.ACL;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;
using HosteraBackend.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("User Endpoints")]
public class UsersController(
    IUserQueryService userQueryService,
    IAuthenticatedUserAccessor authenticatedUserAccessor)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all users")]
    [EndpointDescription("Get the users of the organization of the signed-in user, ordered by username")]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var organizationId = authenticatedUserAccessor.AuthenticatedUser?.OrganizationId ?? 0;
        var query = new GetAllUsersByOrganizationIdQuery(organizationId);
        var users = await userQueryService.Handle(query, cancellationToken);
        return Ok(users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{userId:int}")]
    [EndpointSummary("Get a user by ID")]
    [EndpointDescription("Get a user of the organization of the signed-in user by its unique identifier")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] int userId, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId), cancellationToken);

        // A user of another organization is reported as missing, so its existence is not revealed
        return user is null || user.OrganizationId != authenticatedUserAccessor.AuthenticatedUser?.OrganizationId
            ? IamActionResultAssembler.ToActionResultFromMissingUser(this)
            : Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }
}
