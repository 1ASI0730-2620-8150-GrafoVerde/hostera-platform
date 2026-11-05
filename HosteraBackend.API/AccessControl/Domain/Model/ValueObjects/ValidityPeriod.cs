namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the period in which a credential grants access.
/// </summary>
public sealed record ValidityPeriod
{
    /// <summary>
    ///     Initializes a new <see cref="ValidityPeriod"/> with the given start and end.
    /// </summary>
    /// <param name="validFrom">The moment the credential starts granting access.</param>
    /// <param name="validUntil">The moment the credential stops granting access, or null for permanent access. Must be later than <paramref name="validFrom"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="validUntil"/> is not later than <paramref name="validFrom"/>.</exception>
    public ValidityPeriod(DateTimeOffset validFrom, DateTimeOffset? validUntil)
    {
        if (validUntil <= validFrom)
            throw new ArgumentException("ValidUntil must be later than ValidFrom.", nameof(validUntil));

        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }

    /// <summary>
    ///     Gets the moment the credential starts granting access.
    /// </summary>
    public DateTimeOffset ValidFrom { get; }

    /// <summary>
    ///     Gets the moment the credential stops granting access, or null for permanent access.
    /// </summary>
    public DateTimeOffset? ValidUntil { get; }

    /// <summary>
    ///     Creates a period with the same start that ends at the given moment, unless it already ends earlier.
    /// </summary>
    /// <param name="moment">The moment the access must end.</param>
    /// <returns>The period that ends at the earliest of both moments.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="moment"/> is not later than the start of the period.</exception>
    public ValidityPeriod EndingAt(DateTimeOffset moment) =>
        ValidUntil < moment ? this : new ValidityPeriod(ValidFrom, moment);
}
