using PersonalFinances.BLL.Interfaces.Notification.Senders;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    public class EmailNotificationModel : NotificationModel
    {
        private readonly IEmailSender _emailSender;

        public EmailNotificationModel(IEmailSender emailSender, string recipient, string message)
            : base(recipient, message)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        public override async Task SendNotificationAsync()
        {
            await _emailSender.SendEmailAsync(Recipient, "Notificação", Message);
        }

        public static EmailNotificationModel CreatePasswordResetNotification(string emailAddress, string resetLink, IEmailSender emailSender)
        {
            return new EmailNotificationModel(
                emailSender,
                emailAddress,
                $"Clique no link para redefinir sua password: {resetLink}"
            );
        }
    }
}
