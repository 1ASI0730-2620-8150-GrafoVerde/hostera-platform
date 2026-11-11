using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Replace credential resource for REST API
/// </summary>
/// <param name="CardId">The identifier of the new card</param>
/// <param name="OperatorId">The identifier of the operator who replaces the credential</param>
public record ReplaceCredentialResource(
    [Required] string CardId,
    [Range(1, int.MaxValue)] int OperatorId);
