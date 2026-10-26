using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Entities;

/// <summary>
///     Represents a service included in a rate plan.
/// </summary>
/// <remarks>
///     This entity belongs to the RatePlan aggregate and only changes through it.
/// </remarks>
public class RatePlanService
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected RatePlanService()
    {
    }

    /// <summary>
    ///     Creates the link between a rate plan and a service.
    /// </summary>
    /// <param name="service">The included service.</param>
    public RatePlanService(EIncludedService service)
    {
        Service = service;
    }

    /// <summary>
    ///     Gets the identifier of the rate plan.
    /// </summary>
    public int RatePlanId { get; private set; }

    /// <summary>
    ///     Gets the included service.
    /// </summary>
    public EIncludedService Service { get; private set; }
}
