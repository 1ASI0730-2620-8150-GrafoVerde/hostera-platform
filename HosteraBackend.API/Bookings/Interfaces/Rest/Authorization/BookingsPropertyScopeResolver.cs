using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Authorization;

/// <summary>
///     Finds the property of the bookings addressed by route parameter, including their payments and stays.
/// </summary>
/// <param name="bookingQueryService">The booking query service</param>
public class BookingsPropertyScopeResolver(IBookingQueryService bookingQueryService) : IPropertyScopeResolver
{
    private const string BookingIdKey = "bookingId";

    /// <inheritdoc />
    public IReadOnlyCollection<string> RouteKeys { get; } = [BookingIdKey];

    /// <inheritdoc />
    public async Task<int?> FindPropertyIdAsync(string routeKey, int id, CancellationToken cancellationToken)
    {
        var booking = await bookingQueryService.Handle(new GetBookingByIdQuery(id), cancellationToken);
        return booking?.PropertyId;
    }

    /// <inheritdoc />
    public IActionResult ToNotFoundResult(string routeKey, ControllerBase controller)
    {
        return BookingsActionResultAssembler.ToActionResultFromQueryResult<object>(controller, null,
            BookingError.BookingNotFound, controller.Ok);
    }
}
