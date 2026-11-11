using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Bookings controller.
/// </summary>
/// <param name="bookingCommandService">The booking command service</param>
/// <param name="bookingQueryService">The booking query service</param>
/// <param name="paymentQueryService">The payment query service, which provides the balance of each booking</param>
/// <param name="clock">The clock that provides the current day of the property</param>
[ApiController]
[Route("api/v1/bookings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Booking Endpoints")]
public class BookingsController(
    IBookingCommandService bookingCommandService,
    IBookingQueryService bookingQueryService,
    IPaymentQueryService paymentQueryService,
    IClock clock)
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
        if (booking is null)
            return BookingsActionResultAssembler.ToActionResultFromQueryResult(this, booking,
                BookingError.BookingNotFound, Ok);

        Response.Headers.ETag = BookingETag.ToETag(booking.Version);
        return Ok(await ToResourceAsync(booking, cancellationToken));
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
        [FromBody] UpdateBookingResource resource,
        [FromHeader(Name = BookingETag.IfMatchHeader)] string? ifMatch,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateBookingCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId) with
            {
                ExpectedVersion = BookingETag.ParseIfMatch(ifMatch)
            };
            var result = await bookingCommandService.Handle(command, cancellationToken);

            if (result is Result<Booking, BookingError>.Success { Value: var updated })
            {
                Response.Headers.ETag = BookingETag.ToETag(updated.Version);
                return Ok(await ToResourceAsync(updated, cancellationToken));
            }
            return BookingsActionResultAssembler.ToActionResultFromResult(this, result, Ok);
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    /// <summary>
    ///     Moves a booking to another status: confirms, cancels or restores it, or marks it as a no-show.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="resource">The TransitionBookingResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the booking in its new status, or a problem details response when it does not exist,
    ///     a value is missing, the room is no longer available, or the transition is not allowed, in which case the
    ///     response includes the current status and the allowed transitions.
    /// </returns>
    [HttpPost("{bookingId:int}/transitions")]
    [EndpointSummary("Change the status of a booking")]
    [EndpointDescription("Confirm a pending booking, cancel a pending or confirmed one with a reason, mark a " +
                         "confirmed one as a no-show from its check-in day, or restore a cancelled one to pending " +
                         "before its stay starts while its room is available")]
    [ProducesResponseType(typeof(BookingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> TransitionBooking([FromRoute] int bookingId,
        [FromBody] TransitionBookingResource resource,
        [FromHeader(Name = BookingETag.IfMatchHeader)] string? ifMatch,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = TransitionBookingCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId) with
            {
                ExpectedVersion = BookingETag.ParseIfMatch(ifMatch)
            };
            var result = await bookingCommandService.Handle(command, cancellationToken);

            if (result is Result<Booking, BookingError>.Failure { Error: BookingError.InvalidStatusChange })
            {
                var booking = await bookingQueryService.Handle(new GetBookingByIdQuery(bookingId), cancellationToken);
                if (booking is not null)
                    return BookingsActionResultAssembler.ToActionResultFromInvalidTransition(this, booking,
                        booking.AllowedTransitions(clock.Today));
            }

            if (result is Result<Booking, BookingError>.Success { Value: var changed })
            {
                Response.Headers.ETag = BookingETag.ToETag(changed.Version);
                return Ok(await ToResourceAsync(changed, cancellationToken));
            }
            return BookingsActionResultAssembler.ToActionResultFromResult(this, result, Ok);
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    private async Task<BookingResource> ToResourceAsync(Booking booking, CancellationToken cancellationToken)
    {
        var paidAmounts =
            await paymentQueryService.Handle(new GetPaidAmountsByBookingIdsQuery([booking.Id]), cancellationToken);
        return BookingResourceFromEntityAssembler.ToResourceFromEntity(booking,
            paidAmounts.GetValueOrDefault(booking.Id));
    }
}
