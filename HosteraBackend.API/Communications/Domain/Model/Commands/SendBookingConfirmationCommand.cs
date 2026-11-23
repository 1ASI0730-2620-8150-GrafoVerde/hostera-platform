using HosteraBackend.API.Communications.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Communications.Domain.Model.Commands;

/// <summary>
///     Command to email the guest of a new booking a confirmation with the details of the stay
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="BookingCode">The code of the booking, such as BKG-1048</param>
/// <param name="GuestName">The full name of the guest</param>
/// <param name="GuestEmail">The email address that receives the confirmation</param>
/// <param name="Language">The EEmailLanguage in which the confirmation is written</param>
/// <param name="PropertyName">The name of the property</param>
/// <param name="RoomTypeName">The name of the booked room type</param>
/// <param name="CheckInDate">The day the first night begins</param>
/// <param name="CheckOutDate">The day the guest leaves after the last night</param>
/// <param name="Guests">The number of people staying</param>
/// <param name="TotalAmount">The price of all nights of the stay</param>
/// <param name="Currency">The three-letter code of the currency of the property</param>
public record SendBookingConfirmationCommand(
    int PropertyId,
    int BookingId,
    string BookingCode,
    string GuestName,
    string GuestEmail,
    EEmailLanguage Language,
    string PropertyName,
    string RoomTypeName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int Guests,
    decimal TotalAmount,
    string Currency);
