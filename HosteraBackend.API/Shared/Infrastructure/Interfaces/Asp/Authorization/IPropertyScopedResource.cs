namespace HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;

/// <summary>
///     Marks a request resource that names its property in the body instead of the route, so the request is limited
///     to the properties of the signed-in user like the routes with a <c>propertyId</c>.
/// </summary>
public interface IPropertyScopedResource
{
    /// <summary>
    ///     Gets the identifier of the property the request acts on.
    /// </summary>
    int PropertyId { get; }
}
