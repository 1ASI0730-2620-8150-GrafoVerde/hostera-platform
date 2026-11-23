using System.Globalization;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Iam.Interfaces.ACL;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HosteraBackend.API.Iam.Infrastructure.Pipeline.Filters;

/// <summary>
///     Limits every controller action to the properties of the organization of the signed-in user.
/// </summary>
/// <remarks>
///     The filter reads each route parameter that a <see cref="IPropertyScopeResolver" /> knows, such as
///     <c>propertyId</c>, <c>roomId</c> or <c>bookingId</c>, and the property of each request body that implements
///     <see cref="IPropertyScopedResource" />. When the resource does not exist, or its property belongs to another
///     organization, the action does not run and the request gets the not found response of the bounded context, so
///     the resources of other organizations cannot be told apart from missing ones. Actions without a signed-in user,
///     such as signing in, are not checked.
/// </remarks>
/// <param name="authenticatedUserAccessor">The accessor of the signed-in user.</param>
/// <param name="externalPropertyService">The service that reads the organization of a property.</param>
/// <param name="resolvers">The property scope resolvers of the bounded contexts.</param>
public class PropertyAccessFilter(
    IAuthenticatedUserAccessor authenticatedUserAccessor,
    ExternalPropertyService externalPropertyService,
    IEnumerable<IPropertyScopeResolver> resolvers)
    : IAsyncActionFilter
{
    /// <summary>
    ///     The name of the route parameter of a property.
    /// </summary>
    public const string PropertyIdRouteKey = "propertyId";

    private readonly Dictionary<string, IPropertyScopeResolver> _resolvers = resolvers
        .SelectMany(resolver => resolver.RouteKeys.Select(routeKey => (routeKey, resolver)))
        .ToDictionary(entry => entry.routeKey, entry => entry.resolver, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = authenticatedUserAccessor.AuthenticatedUser;
        if (user is null || context.Controller is not ControllerBase controller)
        {
            await next();
            return;
        }

        foreach (var (routeKey, id) in ScopedIdentifiersOf(context))
        {
            var resolver = _resolvers[routeKey];
            var cancellationToken = context.HttpContext.RequestAborted;
            var propertyId = await resolver.FindPropertyIdAsync(routeKey, id, cancellationToken);
            var organizationId = propertyId is null
                ? null
                : await externalPropertyService.FetchOrganizationIdByPropertyIdAsync(propertyId.Value,
                    cancellationToken);

            if (organizationId != user.OrganizationId)
            {
                context.Result = resolver.ToNotFoundResult(routeKey, controller);
                return;
            }
        }

        await next();
    }

    private IEnumerable<(string RouteKey, int Id)> ScopedIdentifiersOf(ActionExecutingContext context)
    {
        foreach (var (routeKey, value) in context.RouteData.Values)
            if (_resolvers.ContainsKey(routeKey) &&
                int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.None,
                    CultureInfo.InvariantCulture, out var id))
                yield return (routeKey, id);

        if (!_resolvers.ContainsKey(PropertyIdRouteKey))
            yield break;

        foreach (var resource in context.ActionArguments.Values.OfType<IPropertyScopedResource>())
            yield return (PropertyIdRouteKey, resource.PropertyId);
    }
}
