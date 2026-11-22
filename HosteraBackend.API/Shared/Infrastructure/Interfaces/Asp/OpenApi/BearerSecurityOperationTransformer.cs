using HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.OpenApi;

/// <summary>
///     Marks every operation that requires a bearer token with the security requirement of
///     <see cref="BearerSecuritySchemeTransformer" />. Operations marked with <see cref="AllowAnonymousAttribute" />
///     stay public.
/// </summary>
public sealed class BearerSecurityOperationTransformer : IOpenApiOperationTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BearerSecuritySchemeTransformer.SchemeName, context.Document)] = []
        });
        return Task.CompletedTask;
    }
}
