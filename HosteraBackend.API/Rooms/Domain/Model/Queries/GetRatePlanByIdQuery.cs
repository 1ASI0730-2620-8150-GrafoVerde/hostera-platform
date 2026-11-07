namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get a rate plan by ID
/// </summary>
/// <param name="RatePlanId">The rate plan ID to search</param>
public record GetRatePlanByIdQuery(int RatePlanId);
