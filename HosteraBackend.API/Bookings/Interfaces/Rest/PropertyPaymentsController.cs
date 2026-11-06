using System.Net.Mime;
using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;
using HosteraBackend.API.Bookings.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest;

/// <summary>
///     Payments of a property controller.
/// </summary>
/// <param name="paymentQueryService">The payment query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/payments")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Payment Endpoints")]
public class PropertyPaymentsController(IPaymentQueryService paymentQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets the payments received for the bookings of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the payments, ordered by when they were received.</returns>
    [HttpGet]
    [EndpointSummary("Get the payments of a property")]
    [EndpointDescription("Get the payments received for the bookings of a property, ordered by when they were " +
                         "received")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var payments = await paymentQueryService.Handle(new GetAllPaymentsByPropertyIdQuery(propertyId),
            cancellationToken);
        return Ok(payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
