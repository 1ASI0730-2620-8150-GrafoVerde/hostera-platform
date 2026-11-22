namespace HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks a controller action that answers requests without a bearer token, such as signing in.
/// </summary>
/// <remarks>
///     The request authorization middleware rejects every other controller action when the request has no valid
///     token.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute;
