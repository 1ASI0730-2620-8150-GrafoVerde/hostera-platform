using HosteraBackend.API.Iam.Application.Errors;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices;
using HosteraBackend.API.Iam.Application.QueryServices;
using HosteraBackend.API.Iam.Domain.Model.Queries;
using HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using HosteraBackend.API.Iam.Resources;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;

namespace HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Authorizes the requests to the controller endpoints with the bearer token of the Authorization header.
/// </summary>
/// <remarks>
///     When the token is valid, the signed-in user is stored in <see cref="HttpContext.Items" /> under
///     <see cref="UserItemKey" />. Requests without a valid token are answered with a <c>401 Unauthorized</c> problem
///     details response, except for actions marked with <see cref="AllowAnonymousAttribute" />. Endpoints that are not
///     controller actions, such as the OpenAPI document, the Scalar reference and the health check, and CORS
///     preflight requests are not checked.
/// </remarks>
/// <param name="next">The next middleware of the pipeline.</param>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     The key of the signed-in user in <see cref="HttpContext.Items" />.
    /// </summary>
    public const string UserItemKey = "User";

    private const string BearerScheme = "Bearer";

    /// <summary>
    ///     Authorizes the request and calls the next middleware when it is allowed.
    /// </summary>
    /// <param name="context">The context of the request.</param>
    /// <param name="userQueryService">The user query service that finds the signed-in user.</param>
    /// <param name="tokenService">The token service that validates the bearer token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context, IUserQueryService userQueryService, ITokenService tokenService)
    {
        if (!RequiresToken(context))
        {
            await next(context);
            return;
        }

        var token = ReadBearerToken(context.Request);
        var userId = string.IsNullOrEmpty(token) ? null : await tokenService.ValidateToken(token);
        var user = userId is null
            ? null
            : await userQueryService.Handle(new GetUserByIdQuery(userId.Value), context.RequestAborted);

        if (user is null)
        {
            await WriteAuthenticationRequiredAsync(context);
            return;
        }

        context.Items[UserItemKey] = user;
        await next(context);
    }

    private static bool RequiresToken(HttpContext context)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
            return false;

        var metadata = context.GetEndpoint()?.Metadata;
        return metadata?.GetMetadata<ControllerActionDescriptor>() is not null &&
               metadata.GetMetadata<AllowAnonymousAttribute>() is null;
    }

    private static string? ReadBearerToken(HttpRequest request)
    {
        var authorization = request.Headers.Authorization.ToString();
        return authorization.StartsWith($"{BearerScheme} ", StringComparison.OrdinalIgnoreCase)
            ? authorization[(BearerScheme.Length + 1)..].Trim()
            : null;
    }

    private static async Task WriteAuthenticationRequiredAsync(HttpContext context)
    {
        var services = context.RequestServices;
        var localizer = services.GetRequiredService<IStringLocalizer<IamMessages>>();
        const IamError error = IamError.AuthenticationRequired;
        var problem = services.GetRequiredService<ProblemDetailsFactory>().CreateProblemDetails(
            context,
            StatusCodes.Status401Unauthorized,
            error.ToString(),
            detail: localizer[$"{nameof(IamError)}.{error}"]);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers[HeaderNames.WWWAuthenticate] = BearerScheme;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }
}
