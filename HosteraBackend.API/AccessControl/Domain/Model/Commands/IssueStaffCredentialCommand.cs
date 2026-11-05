using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to issue the credential of a staff member
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="StaffMemberId">The identifier of the staff member</param>
/// <param name="HolderName">The name of the staff member</param>
/// <param name="Scope">The EAccessScope the credential opens</param>
/// <param name="ValidityPeriod">The ValidityPeriod of the access, with no end for permanent access</param>
/// <param name="CardId">The CardId of the encoded card</param>
/// <param name="IssuedBy">The identifier of the operator who issues the credential</param>
public record IssueStaffCredentialCommand(
    int PropertyId,
    int StaffMemberId,
    string HolderName,
    EAccessScope Scope,
    ValidityPeriod ValidityPeriod,
    CardId CardId,
    int IssuedBy);
