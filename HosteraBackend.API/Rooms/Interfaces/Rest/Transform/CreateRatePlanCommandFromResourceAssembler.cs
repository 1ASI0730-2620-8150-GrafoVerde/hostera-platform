using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateRatePlanResource" /> into a
///     <see cref="CreateRatePlanCommand" />.
/// </summary>
public static class CreateRatePlanCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateRatePlanResource" /> to a <see cref="CreateRatePlanCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the plan. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property that owns the plan.</param>
    /// <returns>A new <see cref="CreateRatePlanCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when an included service is not valid.</exception>
    public static CreateRatePlanCommand ToCommandFromResource(CreateRatePlanResource resource, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateRatePlanCommand(
            propertyId,
            resource.Name,
            resource.Refundable,
            resource.CancellationPolicy,
            resource.IncludedServices
                .Select(service => RoomsEnumNames.Parse<EIncludedService>(service, "included service"))
                .ToList(),
            resource.RoomTypeIds);
    }
}
