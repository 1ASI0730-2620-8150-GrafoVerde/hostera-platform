using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.OpenApi;

/// <summary>
///     Declares the bearer token security scheme in the OpenAPI document, so the Scalar reference can send the token
///     returned by the sign-in endpoint.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <summary>
    ///     The name of the security scheme in the OpenAPI document.
    /// </summary>
    public const string SchemeName = "Bearer";

    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "The token returned by POST /api/v1/authentication/sign-in."
        };
        return Task.CompletedTask;
    }
}
