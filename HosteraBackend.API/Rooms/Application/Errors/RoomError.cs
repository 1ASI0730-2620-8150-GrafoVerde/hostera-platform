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
	DuplicateRoomTypeName
}
