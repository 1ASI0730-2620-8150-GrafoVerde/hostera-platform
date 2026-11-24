using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Sign in resource for REST API
/// </summary>
/// <param name="Username">The username of the user</param>
/// <param name="Password">The password of the user</param>
public record SignInResource([Required] string Username, [Required] string Password);
