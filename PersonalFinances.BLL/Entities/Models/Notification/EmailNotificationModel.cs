using PersonalFinances.BLL.Interfaces.Notification;
using PersonalFinances.BLL.Interfaces.Notification.Senders;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    public class EmailNotificationModel : IAsyncNotification
    {
        public string EmailAddress { get; private set; }
        public string Message { get; private set; }

        private readonly IEmailSender _emailSender;

        public EmailNotificationModel(IEmailSender emailSender, string emailAddress, string message)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            EmailAddress = !string.IsNullOrEmpty(emailAddress) ? emailAddress : throw new ArgumentException("O endereço de e-mail não pode ser nulo ou vazio.");
            Message = !string.IsNullOrEmpty(message) ? message : throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
        }

        public async Task SendNotificationAsync()
        {
            await _emailSender.SendEmailAsync(EmailAddress, "Notificação", Message);
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
