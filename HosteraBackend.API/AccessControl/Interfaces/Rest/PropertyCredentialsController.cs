using System.Net.Mime;
using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/rfid-credentials")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("RFID Credential Endpoints")]
public class PropertyCredentialsController(
    ICredentialCommandService credentialCommandService,
    ICredentialQueryService credentialQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the credentials of a property")]
    [EndpointDescription("Get all RFID credentials of a property, starting with the most recent")]
    [ProducesResponseType(typeof(IEnumerable<CredentialResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCredentialsByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllCredentialsByPropertyIdQuery(propertyId);
        var credentials = await credentialQueryService.Handle(query, cancellationToken);
        return Ok(credentials.Select(CredentialResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost("encode/guest-key-cards")]
    [EndpointSummary("Issue the key cards of a guest")]
    [EndpointDescription("Issue one credential for each encoded card of a booking")]
    [ProducesResponseType(typeof(IEnumerable<CredentialResource>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IssueGuestKeyCards([FromRoute] int propertyId,
        [FromBody] IssueGuestKeyCardsResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CredentialCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await credentialCommandService.Handle(command, cancellationToken);

            return CredentialActionResultAssembler.ToActionResultFromResult(this, result,
                issued => StatusCode(StatusCodes.Status201Created,
                    issued.Select(CredentialResourceFromEntityAssembler.ToResourceFromEntity)));
        }
        catch (ArgumentException exception)
        {
            return CredentialActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    [HttpPost("encode/staff-credential")]
    [EndpointSummary("Issue the credential of a staff member")]
    [EndpointDescription("Issue a credential for a staff member who holds no other usable credential")]
    [ProducesResponseType(typeof(CredentialResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IssueStaffCredential([FromRoute] int propertyId,
        [FromBody] IssueStaffCredentialResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CredentialCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await credentialCommandService.Handle(command, cancellationToken);

            return CredentialActionResultAssembler.ToActionResultFromResult(this, result,
                issued => CreatedAtAction(
                    nameof(CredentialsController.GetCredentialById),
                    "Credentials",
                    new { credentialId = issued.Id },
                    CredentialResourceFromEntityAssembler.ToResourceFromEntity(issued)));
        }
        catch (ArgumentException exception)
        {
            return CredentialActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
