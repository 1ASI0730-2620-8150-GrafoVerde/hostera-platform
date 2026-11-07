using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Stays of a booking controller: check-in and check-out.
/// </summary>
/// <param name="stayCommandService">The stay command service</param>
/// <param name="paymentQueryService">The payment query service, which provides the balance of the booking</param>
[ApiController]
[Route("api/v1/bookings/{bookingId:int}")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Stay Endpoints")]
public class BookingStaysController(
    IStayCommandService stayCommandService,
    IPaymentQueryService paymentQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Starts the stay of a confirmed booking and registers the key cards encoded for its guest.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="resource">The CheckInBookingResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the checked-in booking and its key cards, or a problem details response when the
    ///     booking cannot be checked in today, the identity is not verified, or the key cards cannot be issued.
    /// </returns>
    [HttpPost("check-in")]
    [EndpointSummary("Check in a booking")]
    [EndpointDescription("Start the stay of a confirmed booking on one of its nights. The guest's identity document " +
                         "must be verified and at least one key card encoded; the key cards open the room until " +
                         "11:00 on the check-out day. A balance due does not prevent the check-in")]
    [ProducesResponseType(typeof(CheckedInStayResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CheckInBooking([FromRoute] int bookingId,
        [FromBody] CheckInBookingResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CheckInBookingCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId);
            var result = await stayCommandService.Handle(command, cancellationToken);

            if (result is Result<CheckedInStay, BookingError>.Success { Value: var stay })
                return Ok(new CheckedInStayResource(await ToResourceAsync(stay.Booking, cancellationToken),
                    stay.KeyCardIds));
            return BookingsActionResultAssembler.ToActionResultFromResult(this, result, Ok);
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    /// <summary>
    ///     Ends the stay of a checked-in booking, ends its key cards and marks the room for cleaning.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="resource">The CheckOutBookingResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the checked-out booking, the number of ended key cards and whether the room was marked
    ///     for cleaning, or a problem details response when the booking is not checked in or still owes money.
    /// </returns>
    [HttpPost("check-out")]
    [EndpointSummary("Check out a booking")]
    [EndpointDescription("End the stay of a checked-in booking once nothing is owed. Its key cards stop opening " +
                         "the room, and the room is marked as needs-cleaning on the departure day unless that day " +
                         "already has a status or another guest arrives")]
    [ProducesResponseType(typeof(CheckedOutStayResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CheckOutBooking([FromRoute] int bookingId,
        [FromBody] CheckOutBookingResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CheckOutBookingCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId);
            var result = await stayCommandService.Handle(command, cancellationToken);

            if (result is Result<CheckedOutStay, BookingError>.Success { Value: var stay })
                return Ok(new CheckedOutStayResource(await ToResourceAsync(stay.Booking, cancellationToken),
                    stay.EndedKeyCards, stay.RoomMarkedForCleaning));
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
