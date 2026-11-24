using System.Globalization;
using System.Net;
using System.Text;
using HosteraBackend.API.Communications.Application.Internal.OutboundServices;
using HosteraBackend.API.Communications.Domain.Model.Commands;
using HosteraBackend.API.Communications.Domain.Model.ValueObjects;
using HosteraBackend.API.Communications.Resources;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.Communications.Application.Internal.EmailTemplates;

/// <summary>
///     Writes the email that confirms a new booking to its guest, in the language the guest prefers.
/// </summary>
/// <remarks>
///     The texts come from <see cref="CommunicationsMessages" />. Dates and amounts follow the conventions of the
///     language of the email, and every value given by a person is encoded before it is placed in the HTML body.
/// </remarks>
/// <param name="localizer">The localizer of the email texts.</param>
public class BookingConfirmationEmailTemplate(IStringLocalizer<CommunicationsMessages> localizer)
{
    /// <summary>
    ///     Writes the confirmation email of a booking.
    /// </summary>
    /// <param name="command">The command with the guest and the details of the stay.</param>
    /// <returns>The email ready to be sent to the guest.</returns>
    public OutgoingEmail Compose(SendBookingConfirmationCommand command)
    {
        var culture = CultureInfo.GetCultureInfo(command.Language == EEmailLanguage.Es ? "es" : "en");
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = culture;
        try
        {
            var details = new (string Label, string Value)[]
            {
                (Text("BookingCode"), command.BookingCode),
                (Text("Property"), command.PropertyName),
                (Text("CheckIn"), command.CheckInDate.ToString("D", culture)),
                (Text("CheckOut"), command.CheckOutDate.ToString("D", culture)),
                (Text("RoomType"), command.RoomTypeName),
                (Text("Guests"), command.Guests.ToString(culture)),
                (Text("Total"), $"{command.Currency} {command.TotalAmount.ToString("N2", culture)}".Trim())
            };

            var subject = Text("Subject", command.BookingCode, command.PropertyName);
            var heading = Text("Heading");
            var greeting = Text("Greeting", command.GuestName);
            var intro = Text("Intro", command.PropertyName);
            var closing = Text("Closing");
            var signature = Text("Signature", command.PropertyName);

            return new OutgoingEmail(command.GuestEmail, subject,
                HtmlBody(culture, heading, greeting, intro, details, closing, signature),
                TextBody(greeting, intro, details, closing, signature));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private string Text(string part, params object[] arguments) =>
        localizer[$"BookingConfirmation.{part}", arguments].Value;

    private static string HtmlBody(CultureInfo culture, string heading, string greeting, string intro,
        IEnumerable<(string Label, string Value)> details, string closing, string signature)
    {
        var html = new StringBuilder();
        html.Append($"<!DOCTYPE html><html lang=\"{culture.TwoLetterISOLanguageName}\"><body ");
        html.Append("style=\"margin:0;padding:24px;background:#f5f6f8;font-family:Arial,Helvetica,sans-serif;color:#1f2933;\">");
        html.Append("<div style=\"max-width:560px;margin:0 auto;background:#ffffff;border-radius:8px;padding:32px;\">");
        html.Append($"<h1 style=\"margin:0 0 16px;font-size:22px;\">{Encode(heading)}</h1>");
        html.Append($"<p style=\"margin:0 0 12px;\">{Encode(greeting)}</p>");
        html.Append($"<p style=\"margin:0 0 20px;\">{Encode(intro)}</p>");
        html.Append("<table style=\"width:100%;border-collapse:collapse;margin:0 0 20px;\">");
        foreach (var (label, value) in details)
        {
            html.Append("<tr>");
            html.Append($"<td style=\"padding:8px 0;border-bottom:1px solid #e4e7eb;color:#52606d;\">{Encode(label)}</td>");
            html.Append($"<td style=\"padding:8px 0;border-bottom:1px solid #e4e7eb;text-align:right;font-weight:bold;\">{Encode(value)}</td>");
            html.Append("</tr>");
        }

        html.Append("</table>");
        html.Append($"<p style=\"margin:0 0 20px;\">{Encode(closing)}</p>");
        html.Append($"<p style=\"margin:0;color:#52606d;\">{Encode(signature)}</p>");
        html.Append("</div></body></html>");
        return html.ToString();
    }

    private static string TextBody(string greeting, string intro, IEnumerable<(string Label, string Value)> details,
        string closing, string signature)
    {
        var text = new StringBuilder();
        text.AppendLine(greeting).AppendLine();
        text.AppendLine(intro).AppendLine();
        foreach (var (label, value) in details)
            text.AppendLine($"{label}: {value}");
        text.AppendLine().AppendLine(closing).AppendLine();
        text.AppendLine(signature);
        return text.ToString();
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
