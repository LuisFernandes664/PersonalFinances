using PersonalFinances.BLL.Enum;
using PersonalFinances.BLL.Interfaces.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    public class NotificationFactory
    {
        private readonly Dictionary<NotificationType, Func<string, string, IAsyncNotification>> _notificationCreators;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public NotificationFactory(ISmsSender smsSender, IEmailSender emailSender)
        {
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

            _notificationCreators = new Dictionary<NotificationType, Func<string, string, IAsyncNotification>>
            {
                { NotificationType.SMS, (recipient, message) => new SMSNotificationModel(_smsSender, recipient, message) },
                { NotificationType.Email, (recipient, message) => new EmailNotificationModel(_emailSender, recipient, message) }
            };
        }

        public IAsyncNotification CreateNotification(NotificationType type, string recipient, string message)
        {
            if (!_notificationCreators.ContainsKey(type))
            {
                throw new ArgumentException($"Tipo de notificação '{type}' não suportado.");
            }

            return _notificationCreators[type](recipient, message);
        }
    }

}
