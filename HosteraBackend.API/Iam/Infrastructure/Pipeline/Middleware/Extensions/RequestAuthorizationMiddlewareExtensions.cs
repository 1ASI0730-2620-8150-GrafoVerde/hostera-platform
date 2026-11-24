using HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Registers the <see cref="RequestAuthorizationMiddleware" /> in the ASP.NET Core pipeline.
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    /// <summary>
    ///     Adds the <see cref="RequestAuthorizationMiddleware" /> to the pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder, to chain other calls.</returns>
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}
