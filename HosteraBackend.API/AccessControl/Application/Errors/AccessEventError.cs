namespace HosteraBackend.API.AccessControl.Application.Errors;

/// <summary>
/// Represents errors that can occur in the access event operations.
/// </summary>
public enum AccessEventError
{
	/// <summary>
	/// One or more provided values are invalid, such as a room door reader without its room or a moment in the future.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The reader reported an access point that is not registered in the property.
	/// </summary>
	AccessPointNotRegistered,

	/// <summary>
	/// No credential of the property was written to the presented card.
	/// </summary>
	CredentialNotFound,

	/// <summary>
	/// The requested access event does not exist.
	/// </summary>
	AccessEventNotFound
}
