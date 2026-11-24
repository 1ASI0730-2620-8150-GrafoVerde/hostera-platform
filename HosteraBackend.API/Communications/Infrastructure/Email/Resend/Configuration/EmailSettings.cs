namespace HosteraBackend.API.Communications.Infrastructure.Email.Resend.Configuration;

/// <summary>
///     The settings of the emails sent to guests, read from the <c>Communications</c> section of the configuration.
/// </summary>
public class EmailSettings
{
    /// <summary>
    ///     The name of the configuration section that holds these settings.
    /// </summary>
    public const string SectionName = "Communications";

    /// <summary>
    ///     The sender used when the configuration does not set one. The Resend test domain can only deliver to the
    ///     email address of the Resend account owner.
    /// </summary>
    public const string DefaultSenderAddress = "Hostera <onboarding@resend.dev>";

    /// <summary>
    ///     Gets or sets the sender of the emails, such as <c>Hostera &lt;bookings@example.com&gt;</c>, on a domain
    ///     verified in Resend.
    /// </summary>
    public string SenderAddress { get; set; } = DefaultSenderAddress;
}
