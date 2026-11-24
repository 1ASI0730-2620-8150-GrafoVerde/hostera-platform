namespace HosteraBackend.API.Communications.Domain.Model.ValueObjects;

/// <summary>
///     Where an email sent to a guest stands.
/// </summary>
public enum EDeliveryStatus
{
    /// <summary>
    ///     The email is being prepared and was not handed to the email provider yet.
    /// </summary>
    Pending,

    /// <summary>
    ///     The email provider accepted the email for delivery.
    /// </summary>
    Sent,

    /// <summary>
    ///     The email provider rejected the email or could not be reached.
    /// </summary>
    Failed,

    /// <summary>
    ///     The email was not sent because sending is not configured, such as in local development.
    /// </summary>
    Skipped
}
