using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Payments of a booking controller.
/// </summary>
/// <param name="paymentCommandService">The payment command service</param>
/// <param name="paymentQueryService">The payment query service</param>
[ApiController]
[Route("api/v1/bookings/{bookingId:int}/payments")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Payment Endpoints")]
public class BookingPaymentsController(
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService)
    : ControllerBase
{
    /// <summary>
    ///     The header a client sends so that repeating a payment request does not record it twice.
    /// </summary>
    public const string IdempotencyKeyHeader = "Idempotency-Key";

    /// <summary>
    ///     Gets the payments of a booking.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the payments of the booking, ordered by when they were received.</returns>
    [HttpGet]
    [EndpointSummary("Get the payments of a booking")]
    [EndpointDescription("Get the payments of a booking, ordered by when they were received")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByBookingId([FromRoute] int bookingId,
        CancellationToken cancellationToken)
    {
        var payments = await paymentQueryService.Handle(new GetAllPaymentsByBookingIdQuery(bookingId),
            cancellationToken);
        return Ok(payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Records a payment received for a booking.
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="resource">The RecordPaymentResource resource.</param>
    /// <param name="idempotencyKey">An optional key that makes repeating the request safe.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A created response with the payment and the new balance of the booking; an OK response with the original
    ///     result when the idempotency key was already used for the same payment; or a problem details response when
    ///     the booking does not accept payments or the amount exceeds its balance.
    /// </returns>
    [HttpPost]
    [EndpointSummary("Record a payment")]
    [EndpointDescription("Record an amount received for a pending, confirmed or checked-in booking, up to its " +
                         "balance due. Send an Idempotency-Key header to make retries safe: a repeated request " +
                         "returns the original payment instead of recording another one")]
    [ProducesResponseType(typeof(RecordedPaymentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RecordedPaymentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordPayment([FromRoute] int bookingId,
        [FromBody] RecordPaymentResource resource,
        [FromHeader(Name = IdempotencyKeyHeader)] string? idempotencyKey,
        [FromHeader(Name = BookingETag.IfMatchHeader)] string? ifMatch,
        CancellationToken cancellationToken)
    {
        try
        {
            var command =
                RecordPaymentCommandFromResourceAssembler.ToCommandFromResource(resource, bookingId, idempotencyKey)
                    with { ExpectedVersion = BookingETag.ParseIfMatch(ifMatch) };
            var result = await paymentCommandService.Handle(command, cancellationToken);

            return BookingsActionResultAssembler.ToActionResultFromResult(this, result, recorded =>
            {
                var recordedResource =
                    RecordedPaymentResourceFromRecordedPaymentAssembler.ToResourceFromRecordedPayment(recorded);
                Response.Headers.ETag = BookingETag.ToETag(recorded.BookingVersion);
                if (recorded.IsReplay)
                {
                    Response.Headers["Idempotent-Replayed"] = "true";
                    return Ok(recordedResource);
                }

                return Created($"/api/v1/bookings/{bookingId}/payments", recordedResource);
            });
        }
        catch (ArgumentException exception)
        {
            return BookingsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
