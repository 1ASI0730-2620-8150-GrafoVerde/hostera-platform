using System.ComponentModel.DataAnnotations;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Model.Commands;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Sign up resource for REST API
/// </summary>
/// <param name="Username">The username of the new user</param>
/// <param name="Password">The password of the new user, between 8 and 64 characters</param>
public record SignUpResource(
    [Required] [StringLength(User.UsernameMaxLength)] string Username,
    [Required]
    [StringLength(SignUpCommand.PasswordMaxLength, MinimumLength = SignUpCommand.PasswordMinLength)]
    string Password);
