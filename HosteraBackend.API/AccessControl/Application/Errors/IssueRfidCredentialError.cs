namespace HosteraBackend.API.AccessControl.Application.Errors;

/// <summary>
/// Represents errors that can occur when issuing an RFID credential.
/// </summary>
public enum IssueRfidCredentialError
{
	/// <summary>
	/// The provided card number is invalid.
	/// </summary>
	InvalidCardNumber,

	/// <summary>
	/// The provided validity period is invalid.
	/// </summary>
	InvalidValidityPeriod,

	/// <summary>
	/// The provided holder name or room number is empty.
	/// </summary>
	MissingHolderOrRoom,

	/// <summary>
	/// An active credential with the same card number already exists.
	/// </summary>
	DuplicateCardNumber,

	/// <summary>
	/// An unexpected error occurred during the operation.
	/// </summary>
	UnexpectedError
}
