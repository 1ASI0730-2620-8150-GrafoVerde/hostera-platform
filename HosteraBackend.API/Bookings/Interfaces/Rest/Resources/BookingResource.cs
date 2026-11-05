namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Booking resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the booking</param>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="Code">The code of the booking, such as BKG-1048</param>
/// <param name="Status">
///     The status of the booking: pending, confirmed, checked-in, checked-out, cancelled or no-show
/// </param>
/// <param name="GuestName">The full name of the guest</param>
/// <param name="GuestEmail">The email address of the guest</param>
/// <param name="GuestPhone">The phone number of the guest, or null</param>
/// <param name="PreferredLanguage">The language of the guest: es or en</param>
/// <param name="CheckInDate">The day the first night begins</param>
/// <param name="CheckOutDate">The day the guest leaves after the last night</param>
/// <param name="Guests">The number of people staying</param>
/// <param name="RoomTypeId">The identifier of the booked room type</param>
/// <param name="RoomId">The identifier of the booked room</param>
/// <param name="RatePlanId">The identifier of the rate plan that prices the nights</param>
/// <param name="TotalAmount">The price of all nights, saved with the booking</param>
/// <param name="PaidAmount">The sum of the payments of the booking</param>
/// <param name="BalanceDue">The total minus the payments, never below zero</param>
/// <param name="PaymentStatus">How much of the total is paid: unpaid, partially-paid or paid</param>
/// <param name="GuestRequest">The note in which the guest asks for something for the stay, or null</param>
/// <param name="CreatedAt">When the booking was created</param>
/// <param name="ConfirmedAt">When the booking was confirmed, or null</param>
/// <param name="ConfirmedBy">The operator who confirmed the booking, or null</param>
/// <param name="CancelledAt">When the booking was cancelled, or null</param>
/// <param name="CancelledBy">The operator who cancelled the booking, or null</param>
/// <param name="CancellationReason">Why the booking was cancelled, or null</param>
/// <param name="CancellationNote">The internal note about the cancellation, or null</param>
/// <param name="NoShowAt">When the booking was marked as a no-show, or null</param>
/// <param name="NoShowBy">The operator who marked the booking as a no-show, or null</param>
public record BookingResource(
    int Id,
    int PropertyId,
    string Code,
    string Status,
    string GuestName,
    string GuestEmail,
    string? GuestPhone,
    string PreferredLanguage,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int Guests,
    int RoomTypeId,
    int RoomId,
    int RatePlanId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceDue,
    string PaymentStatus,
    string? GuestRequest,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ConfirmedAt,
    int? ConfirmedBy,
    DateTimeOffset? CancelledAt,
    int? CancelledBy,
    string? CancellationReason,
    string? CancellationNote,
    DateTimeOffset? NoShowAt,
    int? NoShowBy);
