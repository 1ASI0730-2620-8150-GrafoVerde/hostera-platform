namespace HosteraBackend.API.Iam.Domain.Model.Queries;

/// <summary>
///     Query to get a user by username
/// </summary>
/// <param name="Username">The username to search</param>
public record GetUserByUsernameQuery(string Username);
