using HosteraBackend.API.Communications.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Communications.Application.Internal.OutboundServices;

/// <summary>
///     What happened when an email was handed to the email provider.
/// </summary>
/// <param name="Status">Whether the email was sent, failed or skipped.</param>
/// <param name="ProviderMessageId">The identifier the email provider gave to a sent email, or null.</param>
/// <param name="Reason">Why the email failed or was skipped, or null when it was sent.</param>
public record EmailDelivery(EDeliveryStatus Status, string? ProviderMessageId, string? Reason)
{
    /// <summary>
    ///     Describes an email that the email provider accepted.
    /// </summary>
    /// <param name="providerMessageId">The identifier the email provider gave to the email.</param>
    /// <returns>A sent delivery.</returns>
    public static EmailDelivery Sent(string? providerMessageId) => new(EDeliveryStatus.Sent, providerMessageId, null);

    /// <summary>
    ///     Describes an email that the email provider rejected or that could not reach it.
    /// </summary>
    /// <param name="reason">Why the email was not sent.</param>
    /// <returns>A failed delivery.</returns>
    public static EmailDelivery Failed(string reason) => new(EDeliveryStatus.Failed, null, reason);

    /// <summary>
    ///     Describes an email that was not sent because sending is not configured.
    /// </summary>
    /// <param name="reason">Why the email was not sent.</param>
    /// <returns>A skipped delivery.</returns>
    public static EmailDelivery Skipped(string reason) => new(EDeliveryStatus.Skipped, null, reason);
}
