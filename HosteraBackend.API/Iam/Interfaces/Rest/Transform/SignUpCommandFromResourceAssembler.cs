using HosteraBackend.API.Iam.Domain.Model.Commands;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="SignUpResource" /> into a <see cref="SignUpCommand" />.
/// </summary>
public static class SignUpCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="SignUpResource" /> to a <see cref="SignUpCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the sign-up data. Must not be null.</param>
    /// <returns>A new <see cref="SignUpCommand" /> instance.</returns>
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SignUpCommand(resource.Username, resource.Password);
    }
}
