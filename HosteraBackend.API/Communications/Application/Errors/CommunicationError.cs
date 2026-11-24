namespace HosteraBackend.API.Communications.Application.Errors;

/// <summary>
/// Represents errors that can occur when emailing guests.
/// </summary>
public enum CommunicationError
{
	/// <summary>
	/// One or more provided values are invalid, such as a missing booking code or recipient.
	/// </summary>
	InvalidInput
}
