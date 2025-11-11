using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Schulprojekt.Models;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOrderConfirmationAsync(string toEmail, Order order)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Dein Shop", _config["EmailSettings:From"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = $"Bestellbestätigung #{order.Id}";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $@"
            <h2>Vielen Dank für deine Bestellung!</h2>
            <p>Bestellnummer: <strong>{order.Id}</strong></p>
            <p>Gesamtbetrag: <strong>{order.TotalAmount:C}</strong></p>
            <p>Lieferadresse: {order.ShippingInfo?.Address}, {order.ShippingInfo?.PostalCode} {order.ShippingInfo?.City}, {order.ShippingInfo?.Country}</p>
            <p>Zahlungsart: {order.Payment?.Method}</p>
            <hr />
            <h4>Produkte:</h4>
            <ul>
                {string.Join("", order.OrderItems.Select(item => $"<li>{item.Product?.Name} – {item.Quantity} × {item.UnitPrice:C}</li>"))}
            </ul>
        ";

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}