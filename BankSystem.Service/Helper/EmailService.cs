using System.Net;
using System.Net.Mail;

namespace BankSystem.Service.Helper
{
    public class EmailService
    {
        private readonly string _fromEmail = "stcbank96@gmail.com";  // Store securely (e.g., in environment variables)
        private readonly string _password = "skamzazvrlulwham";     // Store securely (e.g., in environment variables)
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public async Task SendEmailAsync(string toEmail, string otp, int? transactionId = null)
        {
            var subject = "Your OTP for Banking Transaction";
            var body = $"Your OTP is: {otp}";

            if (transactionId.HasValue)
            {
                body += $"\nTransaction ID: {transactionId.Value}";
            }

            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_fromEmail, _password);
                    smtpClient.EnableSsl = true;

                    using (var mailMessage = new MailMessage(_fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                Console.WriteLine($"[EmailService] OTP: {otp} sent to {toEmail} (TxnId: {transactionId})");
            }
            catch (Exception ex)
            {
                // Log the exception (you may want to log this to a file or monitoring system in a real application)
                Console.WriteLine($"[EmailService] Error: {ex.Message}");
            }
        }
    }
}
