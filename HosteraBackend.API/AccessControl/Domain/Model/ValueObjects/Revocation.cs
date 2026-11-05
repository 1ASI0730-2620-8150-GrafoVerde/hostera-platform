namespace HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the revocation of a credential.
/// </summary>
public sealed record Revocation
{
    /// <summary>
    ///     Initializes a new <see cref="Revocation"/> with its reason, note, moment and operator.
    /// </summary>
    /// <param name="reason">The reason for the revocation.</param>
    /// <param name="note">The internal note. Required when the reason is Other.</param>
    /// <param name="at">The moment of the revocation.</param>
    /// <param name="operatorId">The identifier of the operator who revokes the credential.</param>
    /// <exception cref="ArgumentException">Thrown when the reason is Other and <paramref name="note"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="operatorId"/> is not positive.</exception>
    public Revocation(ERevocationReason reason, string? note, DateTimeOffset at, int operatorId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(operatorId);

        if (reason == ERevocationReason.Other && string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("A revocation note is required for the Other reason.", nameof(note));

        Reason = reason;
        Note = note?.Trim() ?? string.Empty;
        At = at;
        OperatorId = operatorId;
    }

    /// <summary>
    ///     Gets the reason for the revocation.
    /// </summary>
    public ERevocationReason Reason { get; }

    /// <summary>
    ///     Gets the internal note about the revocation.
    /// </summary>
    public string Note { get; }

    /// <summary>
    ///     Gets the moment of the revocation.
    /// </summary>
    public DateTimeOffset At { get; }

    /// <summary>
    ///     Gets the identifier of the operator who revoked the credential.
    /// </summary>
    public int OperatorId { get; }
}
