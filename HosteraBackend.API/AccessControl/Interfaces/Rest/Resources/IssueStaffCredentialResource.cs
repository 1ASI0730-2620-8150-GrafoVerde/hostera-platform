using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Issue staff credential resource for REST API
/// </summary>
/// <param name="StaffMemberId">The identifier of the staff member</param>
/// <param name="Scope">The areas the credential opens: service-areas, all-rooms or all-areas</param>
/// <param name="ValidFrom">The moment the access starts</param>
/// <param name="ValidUntil">The moment the access ends, or null for permanent access</param>
/// <param name="CardId">The identifier of the encoded card</param>
/// <param name="IssuedBy">The identifier of the operator who issues the credential</param>
public record IssueStaffCredentialResource(
    [Range(1, int.MaxValue)] int StaffMemberId,
    [Required] string Scope,
    [Required] DateTimeOffset? ValidFrom,
    DateTimeOffset? ValidUntil,
    [Required] string CardId,
    [Range(1, int.MaxValue)] int IssuedBy);
