﻿using System.Net;
using System.Net.Mail;

namespace BankSystem.Service.Helper
{
    public class EmailService
    {
        private readonly string _fromEmail = "stcbank96@gmail.com";
        private readonly string _password = "skamzazvrlulwham";
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public async Task SendEmailAsync(string toEmail, string otp, int? transactionId = null)
        {
            var subject = "Your OTP for Banking Transaction";
            var body = $"Your OTP is: {otp}";

            if (transactionId.HasValue)
            {
                body += $"\nTransaction ID: {transactionId.Value}";
                subject = $"Transaction OTP - ID: {transactionId.Value}";
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
                        Body = body,
                        IsBodyHtml = false
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                Console.WriteLine($"[EmailService] OTP: {otp} sent to {toEmail} (TxnId: {transactionId})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error: {ex.Message}");

                throw new ApplicationException("Error sending email", ex);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_fromEmail, _password);

                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, "STC Bank"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }

    }
}
