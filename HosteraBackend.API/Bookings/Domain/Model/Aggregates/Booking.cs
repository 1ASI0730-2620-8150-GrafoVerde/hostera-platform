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
        Version = 1;
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
    ///     Gets the version of the booking, which grows with every change so that a client can tell whether the
    ///     booking changed after it read it.
    /// </summary>
    public int Version { get; private set; }

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
    ///     Gets when the booking was confirmed, or null.
    /// </summary>
    public DateTimeOffset? ConfirmedAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who confirmed the booking, or null.
    /// </summary>
    public int? ConfirmedBy { get; private set; }

    /// <summary>
    ///     Gets when the booking was cancelled, or null.
    /// </summary>
    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who cancelled the booking, or null.
    /// </summary>
    public int? CancelledBy { get; private set; }

    /// <summary>
    ///     Gets why the booking was cancelled, or null.
    /// </summary>
    public ECancellationReason? CancellationReason { get; private set; }

    /// <summary>
    ///     Gets the internal note about the cancellation, or null.
    /// </summary>
    public string? CancellationNote { get; private set; }

    /// <summary>
    ///     Gets when the booking was marked as a no-show, or null.
    /// </summary>
    public DateTimeOffset? NoShowAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who marked the booking as a no-show, or null.
    /// </summary>
    public int? NoShowBy { get; private set; }

    /// <summary>
    ///     Gets when the stay started, or null.
    /// </summary>
    public DateTimeOffset? CheckedInAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who completed the check-in, or null.
    /// </summary>
    public int? CheckedInBy { get; private set; }

    /// <summary>
    ///     Gets the type of the identity document verified at check-in, or null.
    /// </summary>
    public EDocumentType? GuestDocumentType { get; private set; }

    /// <summary>
    ///     Gets the number of the identity document verified at check-in, or null.
    /// </summary>
    public string? GuestDocumentNumber { get; private set; }

    /// <summary>
    ///     Gets when the stay ended, or null.
    /// </summary>
    public DateTimeOffset? CheckedOutAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who completed the check-out, or null.
    /// </summary>
    public int? CheckedOutBy { get; private set; }

    /// <summary>
    ///     Gets the condition in which the guest left the room, or null.
    /// </summary>
    public ERoomCondition? RoomCondition { get; private set; }

    /// <summary>
    ///     Gets the note about the departure, or null.
    /// </summary>
    public string? DepartureNote { get; private set; }

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
    ///     Gets whether payments can be recorded for the booking: it is Pending, Confirmed or Checked in.
    /// </summary>
    public bool AcceptsPayments => HoldsRoom;

    /// <summary>
    ///     Records that a payment was received for the booking, which changes its balance and so its version.
    /// </summary>
    /// <remarks>
    ///     Two payments recorded at the same time cannot both pass the balance check: the second one finds a newer
    ///     version of the booking and is rejected.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the booking does not accept payments.</exception>
    public void AcceptPayment()
    {
        if (!AcceptsPayments)
            throw new InvalidOperationException($"A {Status} booking does not accept payments.");

        Version++;
    }

    /// <summary>
    ///     Calculates what is still owed: the total minus the payments, never below zero.
    /// </summary>
    /// <param name="paidAmount">The sum of the payments of the booking.</param>
    /// <returns>The balance due.</returns>
    public decimal BalanceDue(decimal paidAmount) => Math.Max(TotalAmount - paidAmount, 0);

    /// <summary>
    ///     Describes how much of the total has been paid.
    /// </summary>
    /// <param name="paidAmount">The sum of the payments of the booking.</param>
    /// <returns>Paid when nothing is owed, PartiallyPaid when some payments exist, and Unpaid otherwise.</returns>
    public EPaymentStatus PaymentStatus(decimal paidAmount) =>
        BalanceDue(paidAmount) == 0 ? EPaymentStatus.Paid
        : paidAmount > 0 ? EPaymentStatus.PartiallyPaid
        : EPaymentStatus.Unpaid;

    /// <summary>
    ///     Gets whether the booking can be confirmed: it is Pending.
    /// </summary>
    public bool CanBeConfirmed => Status == EBookingStatus.Pending;

    /// <summary>
    ///     Gets whether the booking can be cancelled: it is Pending or Confirmed.
    /// </summary>
    public bool CanBeCancelled => IsEditable;

    /// <summary>
    ///     Indicates whether the booking can be marked as a no-show: it is Confirmed and its check-in day has come.
    /// </summary>
    /// <param name="today">The current day of the property.</param>
    /// <returns>True when the guest can be recorded as not arrived.</returns>
    public bool CanBeMarkedNoShow(DateOnly today) => Status == EBookingStatus.Confirmed && CheckInDate <= today;

    /// <summary>
    ///     Indicates whether a cancelled booking can return to Pending: its stay has not started.
    /// </summary>
    /// <param name="today">The current day of the property.</param>
    /// <returns>True when the booking can be restored, if its room is still available.</returns>
    public bool CanBeRestored(DateOnly today) => Status == EBookingStatus.Cancelled && today <= CheckInDate;

    /// <summary>
    ///     Indicates whether the stay can start: the booking is Confirmed and today is one of its nights.
    /// </summary>
    /// <param name="today">The current day of the property.</param>
    /// <returns>True when the guest can check in today.</returns>
    public bool CanBeCheckedIn(DateOnly today) =>
        Status == EBookingStatus.Confirmed && CheckInDate <= today && today < CheckOutDate;

    /// <summary>
    ///     Gets whether the stay can end: the booking is Checked in.
    /// </summary>
    public bool CanBeCheckedOut => Status == EBookingStatus.CheckedIn;

    /// <summary>
    ///     Lists the statuses the booking can move to through a status change.
    /// </summary>
    /// <remarks>
    ///     Check-in and check-out are separate operations with their own requirements, so they are not listed.
    /// </remarks>
    /// <param name="today">The current day of the property.</param>
    /// <returns>The reachable statuses: Confirmed, Cancelled, NoShow or Pending.</returns>
    public IReadOnlyList<EBookingStatus> AllowedTransitions(DateOnly today)
    {
        var transitions = new List<EBookingStatus>();
        if (CanBeConfirmed) transitions.Add(EBookingStatus.Confirmed);
        if (CanBeCancelled) transitions.Add(EBookingStatus.Cancelled);
        if (CanBeMarkedNoShow(today)) transitions.Add(EBookingStatus.NoShow);
        if (CanBeRestored(today)) transitions.Add(EBookingStatus.Pending);
        return transitions;
    }

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
        Version++;
    }

    /// <summary>
    ///     Confirms a pending booking. It keeps its room and can still be edited or cancelled.
    /// </summary>
    /// <param name="at">When the booking is confirmed.</param>
    /// <param name="by">The identifier of the operator who confirms it.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operator is not valid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the booking is not Pending.</exception>
    public void Confirm(DateTimeOffset at, int by)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(by);
        if (!CanBeConfirmed)
            throw new InvalidOperationException($"A {Status} booking cannot be confirmed.");

        Status = EBookingStatus.Confirmed;
        Version++;
        ConfirmedAt = at;
        ConfirmedBy = by;
    }

    /// <summary>
    ///     Ends a pending or confirmed booking without a stay, which releases its room.
    /// </summary>
    /// <param name="reason">Why the booking is cancelled.</param>
    /// <param name="note">An internal note, required when the reason is Other.</param>
    /// <param name="at">When the booking is cancelled.</param>
    /// <param name="by">The identifier of the operator who cancels it.</param>
    /// <exception cref="ArgumentException">Thrown when the operator is not valid or the Other reason has no note.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the booking is not Pending or Confirmed.</exception>
    public void Cancel(ECancellationReason reason, string? note, DateTimeOffset at, int by)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(by);
        if (!Enum.IsDefined(reason))
            throw new ArgumentException("The cancellation reason is not valid.", nameof(reason));
        if (reason == ECancellationReason.Other && string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("The Other cancellation reason requires a note.", nameof(note));
        if (!CanBeCancelled)
            throw new InvalidOperationException($"A {Status} booking cannot be cancelled.");

        Status = EBookingStatus.Cancelled;
        Version++;
        CancelledAt = at;
        CancelledBy = by;
        CancellationReason = reason;
        CancellationNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }

    /// <summary>
    ///     Records that the guest of a confirmed booking did not arrive, which releases its room.
    /// </summary>
    /// <param name="today">The current day of the property.</param>
    /// <param name="at">When the no-show is recorded.</param>
    /// <param name="by">The identifier of the operator who records it.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operator is not valid.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the booking is not Confirmed or its check-in day has not come.
    /// </exception>
    public void MarkNoShow(DateOnly today, DateTimeOffset at, int by)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(by);
        if (!CanBeMarkedNoShow(today))
            throw new InvalidOperationException($"A {Status} booking cannot be marked as a no-show on {today}.");

        Status = EBookingStatus.NoShow;
        Version++;
        NoShowAt = at;
        NoShowBy = by;
    }

    /// <summary>
    ///     Returns a cancelled booking to Pending with its saved total, so it must be confirmed again.
    /// </summary>
    /// <remarks>
    ///     The caller checks that the room is still available for the stay.
    /// </remarks>
    /// <param name="today">The current day of the property.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the booking is not Cancelled or its stay has started.
    /// </exception>
    public void Restore(DateOnly today)
    {
        if (!CanBeRestored(today))
            throw new InvalidOperationException($"A {Status} booking cannot be restored on {today}.");

        Status = EBookingStatus.Pending;
        Version++;
        ConfirmedAt = null;
        ConfirmedBy = null;
        CancelledAt = null;
        CancelledBy = null;
        CancellationReason = null;
        CancellationNote = null;
    }

    /// <summary>
    ///     Starts the stay of a confirmed booking once the identity of the guest is verified. A balance due does not
    ///     prevent it.
    /// </summary>
    /// <param name="today">The current day of the property.</param>
    /// <param name="documentType">The type of the identity document presented by the guest.</param>
    /// <param name="documentNumber">The number of the identity document.</param>
    /// <param name="documentVerified">Whether reception checked that the document belongs to the guest present.</param>
    /// <param name="at">When the stay starts.</param>
    /// <param name="by">The identifier of the operator who completes the check-in.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the operator, the document type or the document number is not valid, or the identity is not
    ///     verified.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the booking is not Confirmed or today is not one of its nights.
    /// </exception>
    public void CheckIn(DateOnly today, EDocumentType documentType, string documentNumber, bool documentVerified,
        DateTimeOffset at, int by)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(by);
        if (!Enum.IsDefined(documentType))
            throw new ArgumentException("The document type is not valid.", nameof(documentType));
        ArgumentException.ThrowIfNullOrWhiteSpace(documentNumber);
        if (!documentVerified)
            throw new ArgumentException("The identity of the guest must be verified.", nameof(documentVerified));
        if (!CanBeCheckedIn(today))
            throw new InvalidOperationException($"A {Status} booking cannot be checked in on {today}.");

        Status = EBookingStatus.CheckedIn;
        Version++;
        CheckedInAt = at;
        CheckedInBy = by;
        GuestDocumentType = documentType;
        GuestDocumentNumber = documentNumber.Trim();
    }

    /// <summary>
    ///     Ends the stay of a checked-in booking once nothing is owed. Leaving before the check-out day releases the
    ///     remaining nights without changing the total.
    /// </summary>
    /// <param name="balanceDue">What the booking still owes.</param>
    /// <param name="roomCondition">The condition in which the guest left the room.</param>
    /// <param name="note">An optional note about the departure.</param>
    /// <param name="at">When the stay ends.</param>
    /// <param name="by">The identifier of the operator who completes the check-out.</param>
    /// <exception cref="ArgumentException">Thrown when the operator or the room condition is not valid.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the booking is not Checked in or something is still owed.
    /// </exception>
    public void CheckOut(decimal balanceDue, ERoomCondition roomCondition, string? note, DateTimeOffset at, int by)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(by);
        if (!Enum.IsDefined(roomCondition))
            throw new ArgumentException("The room condition is not valid.", nameof(roomCondition));
        if (!CanBeCheckedOut)
            throw new InvalidOperationException($"A {Status} booking cannot be checked out.");
        if (balanceDue > 0)
            throw new InvalidOperationException("A booking with a balance due cannot be checked out.");

        Status = EBookingStatus.CheckedOut;
        Version++;
        CheckedOutAt = at;
        CheckedOutBy = by;
        RoomCondition = roomCondition;
        DepartureNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
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
