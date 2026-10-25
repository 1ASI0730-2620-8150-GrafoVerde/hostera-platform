namespace HosteraBackend.API.AccessControl.Application.Errors;

/// <summary>
/// Represents errors that can occur when issuing a credential.
/// </summary>
public enum IssueCredentialError
{
	/// <summary>
	/// The provided card identifier is invalid.
	/// </summary>
	InvalidCardId,

	/// <summary>
	/// The provided validity period is invalid.
	/// </summary>
	InvalidValidityPeriod,

	/// <summary>
	/// The provided access scope is invalid.
	/// </summary>
	InvalidAccessScope,

	/// <summary>
	/// A required field of the credential is missing.
	/// </summary>
	MissingRequiredFields,

	/// <summary>
	/// A usable credential with the same card identifier already exists in the property.
	/// </summary>
	DuplicateCardId,

	/// <summary>
	/// An unexpected error occurred during the operation.
	/// </summary>
	UnexpectedError
}
