namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get the sum of the payments of each of several bookings
/// </summary>
/// <param name="BookingIds">The booking IDs to search</param>
public record GetPaidAmountsByBookingIdsQuery(IReadOnlyCollection<int> BookingIds);
