using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     A recorded payment with the balance of its booking after it.
/// </summary>
/// <param name="Payment">The payment.</param>
/// <param name="BalanceDue">What the booking still owes after the payment.</param>
/// <param name="PaymentStatus">How much of the booking total is paid after the payment.</param>
/// <param name="IsReplay">
///     True when a previous request with the same idempotency key already recorded the payment.
/// </param>
public record RecordedPayment(Payment Payment, decimal BalanceDue, EPaymentStatus PaymentStatus, bool IsReplay);
