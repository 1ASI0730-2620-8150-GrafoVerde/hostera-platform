using HosteraBackend.API.Communications.Application.Internal.OutboundServices;
using HosteraBackend.API.Communications.Infrastructure.Email.Resend.Configuration;
using Microsoft.Extensions.Options;
using Resend;

namespace HosteraBackend.API.Communications.Infrastructure.Email.Resend.Services;

/// <summary>
///     Sends emails through Resend.
/// </summary>
/// <remarks>
///     When <c>Resend:ApiToken</c> is not configured, as in local development, emails are skipped instead of sent.
///     A rejected email or a problem reaching Resend is reported as a failed delivery and never thrown.
/// </remarks>
/// <param name="resend">The Resend client.</param>
/// <param name="resendOptions">The options of the Resend client, which hold the API token.</param>
/// <param name="emailSettings">The settings that hold the sender of the emails.</param>
/// <param name="logger">The logger that reports the problems with Resend.</param>
public class EmailService(
    IResend resend,
    IOptions<ResendClientOptions> resendOptions,
    IOptions<EmailSettings> emailSettings,
    ILogger<EmailService> logger)
    : IEmailService
{
    private const string MissingApiTokenReason = "Resend:ApiToken is not configured.";

    /// <inheritdoc />
    public async Task<EmailDelivery> SendAsync(OutgoingEmail email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resendOptions.Value.ApiToken))
            return EmailDelivery.Skipped(MissingApiTokenReason);

        var senderAddress = string.IsNullOrWhiteSpace(emailSettings.Value.SenderAddress)
            ? EmailSettings.DefaultSenderAddress
            : emailSettings.Value.SenderAddress;

        var message = new EmailMessage
        {
            From = senderAddress,
            Subject = email.Subject,
            HtmlBody = email.HtmlBody,
            TextBody = email.TextBody
        };
        message.To.Add(email.To);

        try
        {
            var response = await resend.EmailSendAsync(message, cancellationToken);
            if (response.Success)
                return EmailDelivery.Sent(response.Content.ToString());

            var reason = response.Exception?.Message ?? "Resend did not accept the email.";
            logger.LogWarning(response.Exception, "Resend did not accept the email to {Recipient}", email.To);
            return EmailDelivery.Failed(reason);
        }
        catch (ResendException exception)
        {
            logger.LogWarning(exception, "Resend rejected the email to {Recipient} with status {StatusCode}",
                email.To, exception.StatusCode);
            return EmailDelivery.Failed(exception.Message);
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "Resend could not be reached to email {Recipient}", email.To);
            return EmailDelivery.Failed(exception.Message);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Resend did not answer in time to email {Recipient}", email.To);
            return EmailDelivery.Failed("Resend did not answer in time.");
        }
    }
}
