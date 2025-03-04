using PersonalFinances.BLL.Entities.Models.Notification;
using PersonalFinances.BLL.Enum;
using PersonalFinances.BLL.Interfaces.Notification.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Notification
{
    public class NotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public NotificationService(IEmailSender emailSender, ISmsSender smsSender)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        }

        /// <summary>
        /// Envia uma única notificação para um ou mais canais usando diretamente `NotificationModel`.
        /// </summary>
        public async Task SendNotificationAsync(IEnumerable<(NotificationType Type, string Recipient)> channels, string message)
        {
            if (channels == null || !channels.Any())
                throw new ArgumentException("Pelo menos um canal de notificação deve ser especificado.");

            var tasks = new List<Task>();

            foreach (var (type, recipient) in channels)
            {
                NotificationModel notification = CreateNotification(type, recipient, message);
                tasks.Add(notification.SendNotificationAsync());
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Envia múltiplas notificações, permitindo diferentes mensagens para cada canal.
        /// </summary>
        public async Task SendNotificationsAsync(IEnumerable<(NotificationType Type, string Recipient, string Message)> notifications)
        {
            if (notifications == null || !notifications.Any())
                throw new ArgumentException("Pelo menos uma notificação deve ser especificada.");

            var tasks = new List<Task>();

            foreach (var (type, recipient, message) in notifications)
            {
                NotificationModel notification = CreateNotification(type, recipient, message);
                tasks.Add(notification.SendNotificationAsync());
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Cria manualmente a notificação com base no tipo especificado.
        /// </summary>
        private NotificationModel CreateNotification(NotificationType type, string recipient, string message)
        {
            return type switch
            {
                NotificationType.Email => new EmailNotificationModel(_emailSender, recipient, message),
                NotificationType.SMS => new SMSNotificationModel(_smsSender, recipient, message),
                _ => throw new ArgumentException($"Tipo de notificação '{type}' não suportado.")
            };
        }
    }
}
