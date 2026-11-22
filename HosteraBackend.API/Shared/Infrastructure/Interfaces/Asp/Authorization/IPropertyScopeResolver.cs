using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;

/// <summary>
///     Finds the property that owns the resources a bounded context addresses by route parameter, such as
///     <c>roomId</c> or <c>bookingId</c>, so requests can be limited to the properties of the signed-in user.
/// </summary>
/// <remarks>
///     Each bounded context registers one resolver for the route parameters of its resources.
/// </remarks>
public interface IPropertyScopeResolver
{
    /// <summary>
    ///     Gets the names of the route parameters whose resources this resolver finds, such as <c>roomId</c>.
    /// </summary>
    IReadOnlyCollection<string> RouteKeys { get; }

    /// <summary>
    ///     Finds the property of a resource.
    /// </summary>
    /// <param name="routeKey">The name of the route parameter, one of <see cref="RouteKeys" />.</param>
    /// <param name="id">The identifier in the route parameter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the property of the resource, or null when the resource does not exist.</returns>
    Task<int?> FindPropertyIdAsync(string routeKey, int id, CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the not found problem details response the bounded context gives for a missing resource, so a
    ///     resource of another organization cannot be told apart from one that does not exist.
    /// </summary>
    /// <param name="routeKey">The name of the route parameter, one of <see cref="RouteKeys" />.</param>
    /// <param name="controller">The controller that handles the request.</param>
    /// <returns>The HTTP response.</returns>
    IActionResult ToNotFoundResult(string routeKey, ControllerBase controller);
}
