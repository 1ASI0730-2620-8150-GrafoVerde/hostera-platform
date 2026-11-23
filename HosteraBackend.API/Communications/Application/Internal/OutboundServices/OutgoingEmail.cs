namespace HosteraBackend.API.Communications.Application.Internal.OutboundServices;

/// <summary>
///     An email ready to be handed to the email provider.
/// </summary>
/// <param name="To">The email address of the recipient.</param>
/// <param name="Subject">The subject line.</param>
/// <param name="HtmlBody">The body shown by email clients that display HTML.</param>
/// <param name="TextBody">The plain-text body shown by the other email clients.</param>
public record OutgoingEmail(string To, string Subject, string HtmlBody, string TextBody);
