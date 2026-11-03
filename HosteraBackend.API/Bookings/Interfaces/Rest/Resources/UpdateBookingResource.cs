using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Update booking resource for REST API
/// </summary>
/// <param name="GuestName">The full name of the guest</param>
/// <param name="GuestEmail">The email address of the guest</param>
/// <param name="GuestPhone">The optional phone number of the guest</param>
/// <param name="PreferredLanguage">The language of the guest: es or en</param>
/// <param name="CheckInDate">The day the first night begins</param>
/// <param name="CheckOutDate">The day the guest leaves after the last night</param>
/// <param name="Guests">The number of people staying, up to the capacity of the room type</param>
/// <param name="RoomTypeId">
///     The identifier of a room type of the property; it must be active unless the booking already uses it
/// </param>
/// <param name="RoomId">The identifier of a room of that room type</param>
/// <param name="RatePlanId">
///     The identifier of a rate plan that sells the room type; it must be active unless the booking already uses it
/// </param>
/// <param name="GuestRequest">An optional note in which the guest asks for something for the stay</param>
public record UpdateBookingResource(
    [Required] [MaxLength(120)] string GuestName,
    [Required] [MaxLength(120)] string GuestEmail,
    [MaxLength(30)] string? GuestPhone,
    [Required] string PreferredLanguage,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    [Range(1, 12)] int Guests,
    [Range(1, int.MaxValue)] int RoomTypeId,
    [Range(1, int.MaxValue)] int RoomId,
    [Range(1, int.MaxValue)] int RatePlanId,
    [MaxLength(500)] string? GuestRequest);
