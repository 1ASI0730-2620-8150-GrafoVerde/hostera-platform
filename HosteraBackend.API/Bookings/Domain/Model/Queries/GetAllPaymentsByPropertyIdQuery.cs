namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get the payments received for the bookings of a property
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
public record GetAllPaymentsByPropertyIdQuery(int PropertyId);
