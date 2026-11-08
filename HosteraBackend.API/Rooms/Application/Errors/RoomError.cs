namespace HosteraBackend.API.Rooms.Application.Errors;

/// <summary>
/// Represents errors that can occur in the room and room type operations.
/// </summary>
public enum RoomError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty name or a capacity out of range.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The requested property does not exist.
	/// </summary>
	PropertyNotFound,

	/// <summary>
	/// The requested room type does not exist.
	/// </summary>
	RoomTypeNotFound,

	/// <summary>
	/// Another room type of the property already uses the name.
	/// </summary>
	DuplicateRoomTypeName,

	/// <summary>
	/// The requested room does not exist.
	/// </summary>
	RoomNotFound,

	/// <summary>
	/// Another room of the property already uses the number.
	/// </summary>
	DuplicateRoomNumber,

	/// <summary>
	/// The room type belongs to a different property than the room.
	/// </summary>
	RoomTypeFromAnotherProperty,

	/// <summary>
	/// The room type is inactive, so new rooms cannot be assigned to it.
	/// </summary>
	InactiveRoomType,

	/// <summary>
	/// The room type cannot be deleted while a room belongs to it or a rate plan lists it.
	/// </summary>
	RoomTypeInUse,

	/// <summary>
	/// The date range ends before it starts or is longer than the supported period.
	/// </summary>
	InvalidDateRange,

	/// <summary>
	/// A booking holds the room on a day of the range, so the day cannot take an operational status.
	/// </summary>
	RoomHeldByBooking
}
