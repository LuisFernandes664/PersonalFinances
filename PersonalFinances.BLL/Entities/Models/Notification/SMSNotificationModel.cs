
using PersonalFinances.BLL.Interfaces.Notification;
using PersonalFinances.BLL.Interfaces.Notification.Senders;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    public class SMSNotificationModel : IAsyncNotification
    {
        public string PhoneNumber { get; private set; }
        public string Message { get; private set; }

        private readonly ISmsSender _smsSender;

        public SMSNotificationModel(ISmsSender smsSender, string phoneNumber, string message)
        {
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            PhoneNumber = !string.IsNullOrEmpty(phoneNumber) ? phoneNumber : throw new ArgumentException("O número de telefone não pode ser nulo ou vazio.");
            Message = !string.IsNullOrEmpty(message) ? message : throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
        }

        public async Task SendNotificationAsync()
        {
            await _smsSender.SendSmsAsync(PhoneNumber, Message);
        }

        public static SMSNotificationModel CreatePasswordResetNotification(string phoneNumber, string resetCode, ISmsSender smsSender)
        {
            return new SMSNotificationModel(
                smsSender,
                phoneNumber,
                $"Seu código de redefinição de password é: {resetCode}"
            );
        }
    }

}
