namespace HosteraBackend.API.AccessControl.Application.Errors;

/// <summary>
/// Represents errors that can occur in the credential operations.
/// </summary>
public enum CredentialError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty holder name or a guest card without an end of validity.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The requested credential does not exist.
	/// </summary>
	CredentialNotFound,

	/// <summary>
	/// The requested staff member does not exist in the property.
	/// </summary>
	StaffMemberNotFound,

	/// <summary>
	/// The staff member already holds a usable credential.
	/// </summary>
	StaffCredentialExists,

	/// <summary>
	/// A usable credential of the property already uses the card identifier.
	/// </summary>
	DuplicateCardId,

	/// <summary>
	/// The credential is expired or revoked, so it can no longer be revoked or replaced.
	/// </summary>
	CredentialNotUsable
}
