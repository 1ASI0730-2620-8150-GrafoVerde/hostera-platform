using System.Net.Mime;
using HosteraBackend.API.Iam.Application.CommandServices;
using HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;
using HosteraBackend.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication Endpoints")]
public class AuthenticationController(IUserCommandService userCommandService) : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [EndpointSummary("Sign in")]
    [EndpointDescription("Check the username and password of a user and issue the bearer token for the next requests")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result,
            signedIn => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(signedIn.User,
                signedIn.Token)));
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [EndpointSummary("Sign up")]
    [EndpointDescription("Register a new user in a new organization. The password needs 8 to 64 characters")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromResult(this, result,
            user => CreatedAtAction(
                nameof(UsersController.GetUserById),
                "Users",
                new { userId = user.Id },
                UserResourceFromEntityAssembler.ToResourceFromEntity(user)));
    }
}
