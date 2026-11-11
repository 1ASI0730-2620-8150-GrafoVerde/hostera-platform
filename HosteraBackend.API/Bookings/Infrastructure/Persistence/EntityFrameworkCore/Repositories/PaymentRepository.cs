using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IPaymentRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class PaymentRepository(AppDbContext context)
    : BaseRepository<Payment>(context), IPaymentRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> FindByBookingIdAsync(int bookingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .Where(payment => payment.BookingId == bookingId)
            .OrderBy(payment => payment.PaidAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .Where(payment => payment.PropertyId == propertyId)
            .OrderBy(payment => payment.PaidAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Payment?> FindByBookingIdAndIdempotencyKeyAsync(int bookingId, string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .FirstOrDefaultAsync(payment => payment.BookingId == bookingId
                                            && payment.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<int, decimal>> SumAmountsByBookingIdsAsync(
        IReadOnlyCollection<int> bookingIds, CancellationToken cancellationToken = default)
    {
        if (bookingIds.Count == 0)
            return new Dictionary<int, decimal>();

        return await Context.Set<Payment>()
            .Where(payment => bookingIds.Contains(payment.BookingId))
            .GroupBy(payment => payment.BookingId)
            .Select(group => new { BookingId = group.Key, Amount = group.Sum(payment => payment.Amount) })
            .ToDictionaryAsync(sum => sum.BookingId, sum => sum.Amount, cancellationToken);
    }
}
