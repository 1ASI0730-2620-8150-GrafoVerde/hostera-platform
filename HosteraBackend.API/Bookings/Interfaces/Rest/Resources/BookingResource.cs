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
/// <param name="GuestRequest">The note in which the guest asks for something for the stay, or null</param>
/// <param name="CreatedAt">When the booking was created</param>
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
    string? GuestRequest,
    DateTimeOffset? CreatedAt);
