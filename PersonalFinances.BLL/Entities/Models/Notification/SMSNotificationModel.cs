using PersonalFinances.BLL.Interfaces.Notification.Senders;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    public class SMSNotificationModel : NotificationModel
    {
        private readonly ISmsSender _smsSender;

        public SMSNotificationModel(ISmsSender smsSender, string recipient, string message)
            : base(recipient, message)
        {
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        }

        public override async Task SendNotificationAsync()
        {
            await _smsSender.SendSmsAsync(Recipient, Message);
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
