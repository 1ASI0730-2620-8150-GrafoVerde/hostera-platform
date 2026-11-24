namespace HosteraBackend.API.Iam.Domain.Model.Queries;

/// <summary>
///     Query to get a user by ID
/// </summary>
/// <param name="Id">The user ID to search</param>
public record GetUserByIdQuery(int Id);
