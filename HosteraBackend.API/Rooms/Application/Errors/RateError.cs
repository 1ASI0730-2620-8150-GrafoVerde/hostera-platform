namespace HosteraBackend.API.Rooms.Application.Errors;

/// <summary>
/// Represents errors that can occur in the rate operations.
/// </summary>
public enum RateError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty name or a price that is not positive.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The date range ends before it starts or is longer than the supported period.
	/// </summary>
	InvalidDateRange,

	/// <summary>
	/// The requested rate plan does not exist.
	/// </summary>
	RatePlanNotFound,

	/// <summary>
	/// Another rate plan of the property already uses the name.
	/// </summary>
	DuplicateRatePlanName,

	/// <summary>
	/// The room type is not sold under the rate plan.
	/// </summary>
	RoomTypeNotInRatePlan
}
