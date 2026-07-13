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

        public async Task SendOrderConfirmationEmailAsync(string customerEmail, int orderId, string productName, decimal amount)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kinetique", _config["EmailSettings:SenderEmail"]));
                message.To.Add(new MailboxAddress("Customer", customerEmail));
                message.Subject = $"Order Confirmation — Kinetique Order #{orderId}";

                message.Body = new TextPart("html")
                {
                    Text = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <div style='background: #0a0a2e; padding: 30px; text-align: center;'>
                                <h1 style='color: #00d4ff; margin: 0;'>👕 Kinetique</h1>
                            </div>
                            <div style='padding: 30px; background: #f9f9f9;'>
                                <h2 style='color: #1a1a1a;'>Order Confirmed! 🎉</h2>
                                <p style='color: #555;'>Thank you for your order. We have received your payment and your order is now being processed.</p>
                                
                                <div style='background: white; border-radius: 10px; padding: 20px; margin: 20px 0; border: 1px solid #eee;'>
                                    <h3 style='color: #1a1a1a; margin-top: 0;'>Order Details</h3>
                                    <p><strong>Order ID:</strong> #{orderId}</p>
                                    <p><strong>Product:</strong> {productName}</p>
                                    <p><strong>Amount Paid:</strong> R{amount:F2}</p>
                                    <p><strong>Status:</strong> <span style='color: #00d4ff;'>Processing</span></p>
                                </div>

                                <p style='color: #555;'>You will receive another email when your order is shipped.</p>
                                <p style='color: #555;'>If you have any questions, reply to this email or contact us at support@kinetique.co.za</p>
                            </div>
                            <div style='background: #0a0a2e; padding: 20px; text-align: center;'>
                                <p style='color: #666; font-size: 13px; margin: 0;'>© {DateTime.Now.Year} Kinetique. All rights reserved.</p>
                            </div>
                        </div>
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