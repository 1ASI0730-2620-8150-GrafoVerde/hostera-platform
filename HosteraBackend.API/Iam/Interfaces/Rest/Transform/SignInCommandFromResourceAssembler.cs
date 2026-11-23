using HosteraBackend.API.Iam.Domain.Model.Commands;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="SignInResource" /> into a <see cref="SignInCommand" />.
/// </summary>
public static class SignInCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="SignInResource" /> to a <see cref="SignInCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the sign-in data. Must not be null.</param>
    /// <returns>A new <see cref="SignInCommand" /> instance.</returns>
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SignInCommand(resource.Username, resource.Password);
    }
}
