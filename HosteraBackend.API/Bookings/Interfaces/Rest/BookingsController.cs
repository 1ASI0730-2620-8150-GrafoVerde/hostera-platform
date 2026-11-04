using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Bookings controller.
/// </summary>
/// <param name="bookingCommandService">The booking command service</param>
/// <param name="bookingQueryService">The booking query service</param>
[ApiController]
[Route("api/v1/bookings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Booking Endpoints")]
public class BookingsController(
    IBookingCommandService bookingCommandService,
    IBookingQueryService bookingQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets a booking by its identifier.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the booking, or not found when it does not exist.</returns>
    [HttpGet("{bookingId:int}")]
    [EndpointSummary("Get a booking by ID")]
    [EndpointDescription("Get a booking by its unique identifier")]
    [ProducesResponseType(typeof(BookingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingById([FromRoute] int bookingId, CancellationToken cancellationToken)
    {
        var booking = await bookingQueryService.Handle(new GetBookingByIdQuery(bookingId), cancellationToken);

        return BookingsActionResultAssembler.ToActionResultFromQueryResult(this, booking,
            BookingError.BookingNotFound, found => Ok(BookingResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    ///     Updates the guest, stay, room and rate plan of a pending or confirmed booking.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="resource">The UpdateBookingResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the updated booking, or a problem details response when it does not exist, cannot be
    ///     edited, a value is not valid or the room is not available.
    /// </returns>
    [HttpPut("{bookingId:int}")]
    [EndpointSummary("Update a booking")]
    [EndpointDescription("Update a pending or confirmed booking. The total is recalculated only when the nights, " +
                         "room type or rate plan change")]
    [ProducesResponseType(typeof(BookingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBooking([FromRoute] int bookingId,
        [FromBody] UpdateBookingResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateBookingCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId);
            var result = await bookingCommandService.Handle(command, cancellationToken);

            return BookingsActionResultAssembler.ToActionResultFromResult(this, result,
                updated => Ok(BookingResourceFromEntityAssembler.ToResourceFromEntity(updated)));
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
