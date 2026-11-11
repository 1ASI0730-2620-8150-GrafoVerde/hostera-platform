using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Commands;

/// <summary>
///     Command to update the guest, stay, room and rate plan of a pending or confirmed booking
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="GuestName">The full name of the guest</param>
/// <param name="GuestEmail">The email address of the guest</param>
/// <param name="GuestPhone">The optional phone number of the guest</param>
/// <param name="PreferredLanguage">The EPreferredLanguage of the guest</param>
/// <param name="CheckInDate">The day the first night begins</param>
/// <param name="CheckOutDate">The day the guest leaves after the last night</param>
/// <param name="Guests">The number of people staying</param>
/// <param name="RoomTypeId">The identifier of the booked room type</param>
/// <param name="RoomId">The identifier of the booked room, which belongs to the room type</param>
/// <param name="RatePlanId">The identifier of the rate plan that prices the nights</param>
/// <param name="GuestRequest">An optional note in which the guest asks for something for the stay</param>
/// <param name="ExpectedVersion">
///     The version of the booking the client read, or null to skip the check; a newer version rejects the change
/// </param>
public record UpdateBookingCommand(
    int BookingId,
    string GuestName,
    string GuestEmail,
    string? GuestPhone,
    EPreferredLanguage PreferredLanguage,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int Guests,
    int RoomTypeId,
    int RoomId,
    int RatePlanId,
    string? GuestRequest,
    int? ExpectedVersion = null);
