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
        public Task SendEmailAsync(string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@focustrack.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                smtpClient.Credentials = new NetworkCredential("user", "passs");
                smtpClient.EnableSsl = true;
                Console.WriteLine($"Enviar e-mail para {to}: {body}");
                smtpClient.Send(mailMessage);
                return Task.CompletedTask;
            }
        }

    }
}
