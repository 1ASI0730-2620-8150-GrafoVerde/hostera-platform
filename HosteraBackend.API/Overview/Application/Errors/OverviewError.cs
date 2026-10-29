namespace HosteraBackend.API.Overview.Application.Errors;

/// <summary>
/// Represents errors that can occur in the operational overview queries.
/// </summary>
public enum OverviewError
{
	/// <summary>
	/// One or more provided values are invalid, such as a performance range that ends before it starts.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The requested property does not exist.
	/// </summary>
	PropertyNotFound
}
