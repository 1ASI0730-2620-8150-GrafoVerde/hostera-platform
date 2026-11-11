namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Recorded payment resource for REST API: the payment with the balance of its booking after it
/// </summary>
/// <param name="Payment">The recorded payment</param>
/// <param name="BalanceDue">What the booking still owes</param>
/// <param name="PaymentStatus">How much of the booking total is paid: unpaid, partially-paid or paid</param>
public record RecordedPaymentResource(PaymentResource Payment, decimal BalanceDue, string PaymentStatus);
