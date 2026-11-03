using System.Text.RegularExpressions;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Aggregates;

/// <summary>
///     Represents an arrangement for a planned guest stay at a property.
/// </summary>
/// <remarks>
///     A booking covers each night from its check-in day up to the day before its check-out day. Pending, Confirmed
///     and Checked in bookings hold their room for those nights. The total is the price of all nights, saved when the
///     booking is created and recalculated only when its nights, room type or rate plan change. Availability,
///     capacity and pricing depend on the Rooms bounded context and are checked by the application layer.
/// </remarks>
public partial class Booking
{
    /// <summary>
    ///     The prefix of every booking code, such as BKG-1048.
    /// </summary>
    public const string CodePrefix = "BKG-";

    /// <summary>
    ///     The longest stay accepted, in nights.
    /// </summary>
    public const int MaxNights = 365;

    /// <summary>
    ///     The largest number of guests accepted by any booking.
    /// </summary>
    public const int MaxGuests = 12;

    private static readonly EBookingStatus[] RoomHoldingStatuses =
        [EBookingStatus.Pending, EBookingStatus.Confirmed, EBookingStatus.CheckedIn];

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Booking()
    {
        Code = null!;
        GuestName = null!;
        GuestEmail = null!;
    }

    /// <summary>
    ///     Creates a pending booking from a create command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateBookingCommand.
    /// </remarks>
    /// <param name="command">The CreateBookingCommand command</param>
    /// <param name="code">The booking code assigned by the property, such as BKG-1048.</param>
    /// <param name="totalAmount">The price of all nights of the stay.</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a value is missing, out of range or not valid.</exception>
    public Booking(CreateBookingCommand command, string code, decimal totalAmount)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        PropertyId = command.PropertyId;
        Code = code;
        Status = EBookingStatus.Pending;
        GuestName = string.Empty;
        GuestEmail = string.Empty;
        ApplyDetails(command.GuestName, command.GuestEmail, command.GuestPhone, command.PreferredLanguage,
            command.CheckInDate, command.CheckOutDate, command.Guests, command.RoomTypeId, command.RoomId,
            command.RatePlanId, command.GuestRequest, totalAmount);
    }

    /// <summary>
    ///     Gets the server-generated identifier for this booking.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the code guests and staff use for the booking, such as BKG-1048.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    ///     Gets where the booking stands in its lifecycle.
    /// </summary>
    public EBookingStatus Status { get; private set; }

    /// <summary>
    ///     Gets the full name of the guest.
    /// </summary>
    public string GuestName { get; private set; }

    /// <summary>
    ///     Gets the email address of the guest.
    /// </summary>
    public string GuestEmail { get; private set; }

    /// <summary>
    ///     Gets the phone number of the guest, or null when it was not given.
    /// </summary>
    public string? GuestPhone { get; private set; }

    /// <summary>
    ///     Gets the language in which the guest prefers to be contacted.
    /// </summary>
    public EPreferredLanguage PreferredLanguage { get; private set; }

    /// <summary>
    ///     Gets the day the first night begins.
    /// </summary>
    public DateOnly CheckInDate { get; private set; }

    /// <summary>
    ///     Gets the day the guest leaves after the last night.
    /// </summary>
    public DateOnly CheckOutDate { get; private set; }

    /// <summary>
    ///     Gets the number of people staying.
    /// </summary>
    public int Guests { get; private set; }

    /// <summary>
    ///     Gets the identifier of the booked room type.
    /// </summary>
    public int RoomTypeId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the booked room.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the rate plan that prices the nights.
    /// </summary>
    public int RatePlanId { get; private set; }

    /// <summary>
    ///     Gets the price of all nights, saved with the booking.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    ///     Gets the note in which the guest asks for something for the stay, or null.
    /// </summary>
    public string? GuestRequest { get; private set; }

    /// <summary>
    ///     Gets the number of nights of the stay.
    /// </summary>
    public int Nights => CheckOutDate.DayNumber - CheckInDate.DayNumber;

    /// <summary>
    ///     Gets the last night of the stay, the day before check-out.
    /// </summary>
    public DateOnly LastNight => CheckOutDate.AddDays(-1);

    /// <summary>
    ///     Gets whether the booking holds its room for its nights.
    /// </summary>
    public bool HoldsRoom => RoomHoldingStatuses.Contains(Status);

    /// <summary>
    ///     Gets whether the guest details, stay, guests, room and rate plan can still change.
    /// </summary>
    public bool IsEditable => Status is EBookingStatus.Pending or EBookingStatus.Confirmed;

    /// <summary>
    ///     Builds the code that follows the latest code of a property.
    /// </summary>
    /// <remarks>
    ///     Each property numbers its bookings in its own thousand: property 1 starts at BKG-1001.
    /// </remarks>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="latestNumber">The number of the latest code of the property, or 0 when it has none.</param>
    /// <returns>The next booking code.</returns>
    public static string NextCode(int propertyId, int latestNumber) =>
        $"{CodePrefix}{Math.Max(latestNumber, propertyId * 1000) + 1}";

    /// <summary>
    ///     Reads the number of a booking code.
    /// </summary>
    /// <param name="code">The code, such as BKG-1048.</param>
    /// <returns>The number of the code, or 0 when the code does not follow the BKG-number format.</returns>
    public static int NumberOf(string? code) =>
        code is not null && code.StartsWith(CodePrefix, StringComparison.Ordinal)
                         && int.TryParse(code.AsSpan(CodePrefix.Length), out var number)
            ? number
            : 0;

    /// <summary>
    ///     Indicates whether the stay shares at least one night with another stay.
    /// </summary>
    /// <param name="checkInDate">The check-in day of the other stay.</param>
    /// <param name="checkOutDate">The check-out day of the other stay.</param>
    /// <returns>True when both stays include a common night.</returns>
    public bool Overlaps(DateOnly checkInDate, DateOnly checkOutDate) =>
        CheckInDate < checkOutDate && checkInDate < CheckOutDate;

    /// <summary>
    ///     Indicates whether new values keep the nights, room type and rate plan that priced the booking.
    /// </summary>
    /// <param name="roomTypeId">The new room type.</param>
    /// <param name="ratePlanId">The new rate plan.</param>
    /// <param name="checkInDate">The new check-in day.</param>
    /// <param name="checkOutDate">The new check-out day.</param>
    /// <returns>True when the saved total still applies.</returns>
    public bool HasSamePricingAs(int roomTypeId, int ratePlanId, DateOnly checkInDate, DateOnly checkOutDate) =>
        RoomTypeId == roomTypeId && RatePlanId == ratePlanId && CheckInDate == checkInDate &&
        CheckOutDate == checkOutDate;

    /// <summary>
    ///     Updates the guest, stay, room and rate plan of an editable booking.
    /// </summary>
    /// <param name="command">The UpdateBookingCommand command</param>
    /// <param name="totalAmount">The price of all nights, kept or recalculated by the caller.</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a value is missing, out of range or not valid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the booking is not Pending or Confirmed.</exception>
    public void Update(UpdateBookingCommand command, decimal totalAmount)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!IsEditable)
            throw new InvalidOperationException($"A {Status} booking cannot be edited.");

        ApplyDetails(command.GuestName, command.GuestEmail, command.GuestPhone, command.PreferredLanguage,
            command.CheckInDate, command.CheckOutDate, command.Guests, command.RoomTypeId, command.RoomId,
            command.RatePlanId, command.GuestRequest, totalAmount);
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailRegex();

    private void ApplyDetails(string guestName, string guestEmail, string? guestPhone,
        EPreferredLanguage preferredLanguage, DateOnly checkInDate, DateOnly checkOutDate, int guests,
        int roomTypeId, int roomId, int ratePlanId, string? guestRequest, decimal totalAmount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(guestName);
        ArgumentException.ThrowIfNullOrWhiteSpace(guestEmail);
        if (!EmailRegex().IsMatch(guestEmail.Trim()))
            throw new ArgumentException("The guest email is not a valid email address.", nameof(guestEmail));
        if (!Enum.IsDefined(preferredLanguage))
            throw new ArgumentException("The preferred language is not valid.", nameof(preferredLanguage));

        var nights = checkOutDate.DayNumber - checkInDate.DayNumber;
        if (nights < 1 || nights > MaxNights)
            throw new ArgumentException($"A stay covers from 1 to {MaxNights} nights.", nameof(checkOutDate));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(guests);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(guests, MaxGuests);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ratePlanId);
        ArgumentOutOfRangeException.ThrowIfNegative(totalAmount);

        GuestName = guestName.Trim();
        GuestEmail = guestEmail.Trim();
        GuestPhone = string.IsNullOrWhiteSpace(guestPhone) ? null : guestPhone.Trim();
        PreferredLanguage = preferredLanguage;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Guests = guests;
        RoomTypeId = roomTypeId;
        RoomId = roomId;
        RatePlanId = ratePlanId;
        GuestRequest = string.IsNullOrWhiteSpace(guestRequest) ? null : guestRequest.Trim();
        TotalAmount = totalAmount;
    }
}
