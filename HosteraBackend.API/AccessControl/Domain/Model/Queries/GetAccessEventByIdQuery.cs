namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get an access event by its ID
/// </summary>
/// <param name="Id">The access event ID to search</param>
public record GetAccessEventByIdQuery(int Id);
