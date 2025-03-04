using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Interfaces.Notification.Senders;

namespace PersonalFinances.BLL.Entities.Models.Notification.Senders
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "noreply@focustrack.com";
        private readonly string _smtpPass = "passs";

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }

        }

    }
}
