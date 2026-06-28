using MailKit.Net.Smtp;
using MimeKit;

namespace KinetiqueAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderStatusEmailAsync(string customerEmail, int orderId, string status)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Kinetique", _config["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("Customer", customerEmail));
            message.Subject = $"Kinetique Order #{orderId} — {status}";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Order Update — Kinetique</h2>
                    <p>Your order <strong>#{orderId}</strong> has been updated.</p>
                    <p>Current Status: <strong>{status.ToUpper()}</strong></p>
                    <br/>
                    <p>Thank you for shopping with Kinetique!</p>
                "
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:SmtpPort"]!),
                false
            );
            await client.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:SenderPassword"]
            );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}