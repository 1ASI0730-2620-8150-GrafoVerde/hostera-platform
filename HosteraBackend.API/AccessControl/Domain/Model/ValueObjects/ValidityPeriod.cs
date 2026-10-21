namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the period in which an RFID credential grants access.
/// </summary>
public sealed record ValidityPeriod
{
    /// <summary>
    ///     Initializes a new <see cref="ValidityPeriod"/> with the given start and end.
    /// </summary>
    /// <param name="validFrom">The moment the credential starts granting access.</param>
    /// <param name="validUntil">The moment the credential stops granting access. Must be later than <paramref name="validFrom"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="validUntil"/> is not later than <paramref name="validFrom"/>.</exception>
    public ValidityPeriod(DateTimeOffset validFrom, DateTimeOffset validUntil)
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
    ///     Gets the moment the credential stops granting access.
    /// </summary>
    public DateTimeOffset ValidUntil { get; }

    /// <summary>
    ///     Indicates whether the given moment falls within this period.
    /// </summary>
    /// <param name="moment">The moment to evaluate.</param>
    /// <returns>True when the moment is between ValidFrom and ValidUntil.</returns>
    public bool Includes(DateTimeOffset moment) => moment >= ValidFrom && moment < ValidUntil;
}
