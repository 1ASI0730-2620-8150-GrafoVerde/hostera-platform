using HosteraBackend.API.Iam.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Iam.Domain.Model.Commands;

/// <summary>
///     Command to change the full name and the role of a user
/// </summary>
/// <param name="UserId">The identifier of the user</param>
/// <param name="FullName">The new full name of the person who uses the account</param>
/// <param name="Role">The new role of the user in the organization</param>
public record UpdateUserProfileCommand(int UserId, string FullName, EUserRole Role);
