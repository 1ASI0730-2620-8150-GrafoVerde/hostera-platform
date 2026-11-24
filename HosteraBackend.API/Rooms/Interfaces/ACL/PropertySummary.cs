namespace HosteraBackend.API.Rooms.Interfaces.ACL;

/// <summary>
///     The data of a property shared with other bounded contexts.
/// </summary>
/// <param name="Id">The identifier of the property.</param>
/// <param name="Name">The name of the property.</param>
/// <param name="Currency">The three-letter code of the currency of the property.</param>
public record PropertySummary(int Id, string Name, string Currency);
