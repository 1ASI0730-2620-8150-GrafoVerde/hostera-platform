using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Bookings of a property controller.
/// </summary>
/// <param name="bookingCommandService">The booking command service</param>
/// <param name="bookingQueryService">The booking query service</param>
/// <param name="paymentQueryService">The payment query service, which provides the balance of each booking</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/bookings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Booking Endpoints")]
public class PropertyBookingsController(
    IBookingCommandService bookingCommandService,
    IBookingQueryService bookingQueryService,
    IPaymentQueryService paymentQueryService)
    : ControllerBase
{
    private const int MaxPageSize = 200;

    /// <summary>
    ///     Gets the bookings of a property that satisfy every given criterion.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="status">An optional booking status.</param>
    /// <param name="roomId">An optional room.</param>
    /// <param name="from">Keep only the stays with a night on or after this day.</param>
    /// <param name="to">Keep only the stays with a night on or before this day.</param>
    /// <param name="search">Keep only the bookings whose guest name or code contains this text.</param>
    /// <param name="page">The page to return, starting at 1; omit it to return every matching booking.</param>
    /// <param name="pageSize">The number of bookings per page, from 1 to 200.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the matching bookings, with their total count in the X-Total-Count header, or bad
    ///     request when a criterion is not valid.
    /// </returns>
    [HttpGet]
    [EndpointSummary("Get the bookings of a property")]
    [EndpointDescription("Get the bookings of a property ordered by check-in day and code. Every given criterion " +
                         "must match. The total number of matches is returned in the X-Total-Count header")]
    [ProducesResponseType(typeof(IEnumerable<BookingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookingsByPropertyId(
        [FromRoute] int propertyId,
        [FromQuery] string? status,
        [FromQuery] int? roomId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] string? search,
        [FromQuery] int? page,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            if (page < 1)
                throw new ArgumentException("The page must be 1 or greater.", nameof(page));
            if (pageSize is < 1 or > MaxPageSize)
                throw new ArgumentException($"The page size must be between 1 and {MaxPageSize}.", nameof(pageSize));
            if (from is not null && to is not null && from > to)
                throw new ArgumentException("The start of the search must not be after its end.", nameof(from));

            var query = new GetAllBookingsByPropertyIdQuery(
                propertyId,
                string.IsNullOrWhiteSpace(status) ? null : BookingsEnumNames.Parse<EBookingStatus>(status, "status"),
                roomId,
                from,
                to,
                search,
                page,
                pageSize);
            var bookingPage = await bookingQueryService.Handle(query, cancellationToken);
            var bookings = bookingPage.Bookings.ToList();
            var paidAmounts = await paymentQueryService.Handle(
                new GetPaidAmountsByBookingIdsQuery(bookings.Select(booking => booking.Id).ToList()),
                cancellationToken);

            Response.Headers["X-Total-Count"] = bookingPage.TotalCount.ToString();
            return Ok(bookings.Select(booking =>
                BookingResourceFromEntityAssembler.ToResourceFromEntity(booking,
                    paidAmounts.GetValueOrDefault(booking.Id))));
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    /// <summary>
    ///     Creates a pending booking in a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="resource">The CreateBookingResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the created booking with its code and total, or a problem details response when a
    ///     value is not valid, the room, room type or rate plan do not fit, or the room is not available.
    /// </returns>
    [HttpPost]
    [EndpointSummary("Create a booking")]
    [EndpointDescription("Create a pending booking. The server assigns the next code of the property and prices " +
                         "each night with the daily rate of the room type under the rate plan, or its base rate")]
    [ProducesResponseType(typeof(BookingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBooking([FromRoute] int propertyId,
        [FromBody] CreateBookingResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateBookingCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await bookingCommandService.Handle(command, cancellationToken);

            return BookingsActionResultAssembler.ToActionResultFromResult(this, result,
                created => CreatedAtAction(
                    nameof(BookingsController.GetBookingById),
                    "Bookings",
                    new { bookingId = created.Id },
                    BookingResourceFromEntityAssembler.ToResourceFromEntity(created, 0)));
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
