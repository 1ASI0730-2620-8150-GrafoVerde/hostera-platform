using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Authorization;

/// <summary>
///     Finds the property of the properties, room types, rooms and rate plans addressed by route parameter.
/// </summary>
/// <param name="propertyQueryService">The property query service</param>
/// <param name="roomTypeQueryService">The room type query service</param>
/// <param name="roomQueryService">The room query service</param>
/// <param name="ratePlanQueryService">The rate plan query service</param>
public class RoomsPropertyScopeResolver(
    IPropertyQueryService propertyQueryService,
    IRoomTypeQueryService roomTypeQueryService,
    IRoomQueryService roomQueryService,
    IRatePlanQueryService ratePlanQueryService)
    : IPropertyScopeResolver
{
    private const string PropertyIdKey = "propertyId";
    private const string RoomTypeIdKey = "roomTypeId";
    private const string RoomIdKey = "roomId";
    private const string RatePlanIdKey = "ratePlanId";

    /// <inheritdoc />
    public IReadOnlyCollection<string> RouteKeys { get; } = [PropertyIdKey, RoomTypeIdKey, RoomIdKey, RatePlanIdKey];

    /// <inheritdoc />
    public async Task<int?> FindPropertyIdAsync(string routeKey, int id, CancellationToken cancellationToken)
    {
        return routeKey switch
        {
            PropertyIdKey => (await propertyQueryService.Handle(new GetPropertyByIdQuery(id), cancellationToken))?.Id,
            RoomTypeIdKey => (await roomTypeQueryService.Handle(new GetRoomTypeByIdQuery(id), cancellationToken))
                ?.PropertyId,
            RoomIdKey => (await roomQueryService.Handle(new GetRoomByIdQuery(id), cancellationToken))?.PropertyId,
            RatePlanIdKey => (await ratePlanQueryService.Handle(new GetRatePlanByIdQuery(id), cancellationToken))
                ?.PropertyId,
            _ => null
        };
    }

    /// <inheritdoc />
    public IActionResult ToNotFoundResult(string routeKey, ControllerBase controller)
    {
        return routeKey switch
        {
            RatePlanIdKey => RoomsActionResultAssembler.ToActionResultFromRateError(controller,
                RateError.RatePlanNotFound),
            RoomTypeIdKey => NotFound(controller, RoomError.RoomTypeNotFound),
            RoomIdKey => NotFound(controller, RoomError.RoomNotFound),
            _ => NotFound(controller, RoomError.PropertyNotFound)
        };
    }

    private static IActionResult NotFound(ControllerBase controller, RoomError error) =>
        RoomsActionResultAssembler.ToActionResultFromQueryResult<object>(controller, null, error, controller.Ok);
}
