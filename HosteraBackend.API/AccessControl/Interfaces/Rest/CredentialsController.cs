using System.Net.Mime;
using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest;

[ApiController]
[Route("api/v1/rfid-credentials")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("RFID Credential Endpoints")]
public class CredentialsController(
    ICredentialCommandService credentialCommandService,
    ICredentialQueryService credentialQueryService)
    : ControllerBase
{
    [HttpGet("{credentialId:int}")]
    [EndpointSummary("Get a credential by ID")]
    [EndpointDescription("Get an RFID credential by its unique identifier")]
    [ProducesResponseType(typeof(CredentialResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCredentialById([FromRoute] int credentialId,
        CancellationToken cancellationToken)
    {
        var query = new GetCredentialByIdQuery(credentialId);
        var credential = await credentialQueryService.Handle(query, cancellationToken);

        return credential is null
            ? CredentialActionResultAssembler.ToActionResultFromMissingCredential(this)
            : Ok(CredentialResourceFromEntityAssembler.ToResourceFromEntity(credential));
    }

    [HttpPost("{credentialId:int}/replace")]
    [EndpointSummary("Replace a credential")]
    [EndpointDescription("Revoke a credential as replaced and issue a new card with the same end of validity")]
    [ProducesResponseType(typeof(CredentialResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReplaceCredential([FromRoute] int credentialId,
        [FromBody] ReplaceCredentialResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CredentialCommandFromResourceAssembler.ToCommandFromResource(resource, credentialId);
            var result = await credentialCommandService.Handle(command, cancellationToken);

            return CredentialActionResultAssembler.ToActionResultFromResult(this, result,
                replacement => CreatedAtAction(
                    nameof(GetCredentialById),
                    new { credentialId = replacement.Id },
                    CredentialResourceFromEntityAssembler.ToResourceFromEntity(replacement)));
        }
        catch (ArgumentException exception)
        {
            return CredentialActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    [HttpPost("{credentialId:int}/revoke")]
    [EndpointSummary("Revoke a credential")]
    [EndpointDescription("Revoke an active or scheduled credential. The other reason requires a note")]
    [ProducesResponseType(typeof(CredentialResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RevokeCredential([FromRoute] int credentialId,
        [FromBody] RevokeCredentialResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CredentialCommandFromResourceAssembler.ToCommandFromResource(resource, credentialId);
            var result = await credentialCommandService.Handle(command, cancellationToken);

            return CredentialActionResultAssembler.ToActionResultFromResult(this, result,
                revoked => Ok(CredentialResourceFromEntityAssembler.ToResourceFromEntity(revoked)));
        }
        catch (ArgumentException exception)
        {
            return CredentialActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
