using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateRatePlanResource" /> into an
///     <see cref="UpdateRatePlanCommand" />.
/// </summary>
public static class UpdateRatePlanCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateRatePlanResource" /> to an <see cref="UpdateRatePlanCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the plan. Must not be null.</param>
    /// <param name="ratePlanId">The identifier of the plan to update.</param>
    /// <returns>A new <see cref="UpdateRatePlanCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when an included service or the status is not valid.</exception>
    public static UpdateRatePlanCommand ToCommandFromResource(UpdateRatePlanResource resource, int ratePlanId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateRatePlanCommand(
            ratePlanId,
            resource.Name,
            resource.Refundable,
            resource.CancellationPolicy,
            resource.IncludedServices
                .Select(service => RoomsEnumNames.Parse<EIncludedService>(service, "included service"))
                .ToList(),
            resource.RoomTypeIds,
            RoomsEnumNames.Parse<ERecordStatus>(resource.Status, "rate plan status"));
    }
}
