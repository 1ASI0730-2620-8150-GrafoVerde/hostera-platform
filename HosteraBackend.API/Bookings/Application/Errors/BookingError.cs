namespace HosteraBackend.API.Bookings.Application.Errors;

/// <summary>
/// Represents errors that can occur in the booking operations.
/// </summary>
public enum BookingError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty guest name or a malformed email.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The check-out day is not after the check-in day, or the stay is longer than the supported period.
	/// </summary>
	InvalidStayDates,

	/// <summary>
	/// The requested booking does not exist.
	/// </summary>
	BookingNotFound,

	/// <summary>
	/// The room does not exist in the property of the booking.
	/// </summary>
	RoomNotFound,

	/// <summary>
	/// The room type does not exist in the property of the booking.
	/// </summary>
	RoomTypeNotFound,

	/// <summary>
	/// The rate plan does not exist in the property of the booking.
	/// </summary>
	RatePlanNotFound,

	/// <summary>
	/// The room does not belong to the booked room type.
	/// </summary>
	RoomNotInRoomType,

	/// <summary>
	/// The room type is inactive, so new bookings cannot use it.
	/// </summary>
	InactiveRoomType,

	/// <summary>
	/// The rate plan is inactive, so new bookings cannot use it.
	/// </summary>
	InactiveRatePlan,

	/// <summary>
	/// The rate plan does not sell the booked room type.
	/// </summary>
	RatePlanNotForRoomType,

	/// <summary>
	/// The number of guests exceeds the capacity of the room type.
	/// </summary>
	OverCapacity,

	/// <summary>
	/// Another booking holds the room, or a Blocked or Out of service status covers it, on a night of the stay.
	/// </summary>
	RoomUnavailable,

	/// <summary>
	/// The booking is not Pending or Confirmed, so its details cannot change.
	/// </summary>
	NotEditable
}
