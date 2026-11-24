namespace HosteraBackend.API.Iam.Application.Errors;

/// <summary>
/// Represents errors that can occur in the identity and access management operations.
/// </summary>
public enum IamError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty username or a password that is too short.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The requested user does not exist.
	/// </summary>
	UserNotFound,

	/// <summary>
	/// Another user already uses the username.
	/// </summary>
	UsernameAlreadyTaken,

	/// <summary>
	/// The username or the password is not correct.
	/// </summary>
	InvalidCredentials,

	/// <summary>
	/// The request has no bearer token, or its token is not valid or has expired.
	/// </summary>
	AuthenticationRequired
}
