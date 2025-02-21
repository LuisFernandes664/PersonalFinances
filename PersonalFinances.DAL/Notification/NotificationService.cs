using PersonalFinances.BLL.Entities.Models.Notification;
using PersonalFinances.BLL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Notification
{
    public class NotificationService
    {
        private readonly NotificationFactory _notificationFactory;

        public NotificationService(NotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
        }

        /// <summary>
        /// Envia uma notificação para os tipos especificados com os respectivos destinatários.
        /// </summary>
        public async Task SendNotificationAsync(IEnumerable<(NotificationType Type, string Recipient)> channels, string message)
        {
            if (channels == null || !channels.Any())
            {
                throw new ArgumentException("Pelo menos um canal de notificação deve ser especificado.");
            }

            var tasks = new List<Task>();
            foreach (var (type, recipient) in channels)
            {
                var notification = _notificationFactory.CreateNotification(type, recipient, message);
                tasks.Add(notification.SendNotificationAsync());
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Envia notificações para múltiplos canais e mensagens diferentes.
        /// </summary>
        public async Task SendNotificationsAsync(IEnumerable<(NotificationType Type, string Recipient, string Message)> notifications)
        {
            if (notifications == null || !notifications.Any())
            {
                throw new ArgumentException("Pelo menos uma notificação deve ser especificada.");
            }

            var tasks = new List<Task>();
            foreach (var (type, recipient, message) in notifications)
            {
                var notification = _notificationFactory.CreateNotification(type, recipient, message);
                tasks.Add(notification.SendNotificationAsync());
            }
            await Task.WhenAll(tasks);
        }
    }
}
