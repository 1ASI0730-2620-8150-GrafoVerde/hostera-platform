namespace HosteraBackend.API.Communications.Application.Internal.OutboundServices;

/// <summary>
///     Sends emails through the email provider.
/// </summary>
/// <remarks>
///     Sending never throws because of the provider: a rejected email, a missing configuration or a network problem
///     is reported in the returned <see cref="EmailDelivery" />.
/// </remarks>
public interface IEmailService
{
    /// <summary>
    ///     Sends an email.
    /// </summary>
    /// <param name="email">The email to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the email was sent, failed or skipped.</returns>
    Task<EmailDelivery> SendAsync(OutgoingEmail email, CancellationToken cancellationToken);
}
