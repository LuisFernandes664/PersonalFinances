using PersonalFinances.BLL.Interfaces.Notification;
using System.Data;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    /// <summary>
    /// Representa uma notificação enviada a um utilizador. 
    /// Esta classe deve ser herdada por tipos específicos de notificações (exemplo: Email, SMS).
    /// </summary>
    public abstract class NotificationModel : BaseEntity, IAsyncNotification
    {
        #region Propriedades

        /// <summary>
        /// Identificador único do utilizador que recebeu a notificação.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Tipo da notificação (ex: 'start', 'end', 'reminder').
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Mensagem da notificação.
        /// </summary>
        public string Message { get; private set; }

        public string Recipient { get; protected set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor protegido para impedir instância direta da classe base.
        /// </summary>
        protected NotificationModel() { }

        protected NotificationModel(string recipient, string message)
        {
            Recipient = !string.IsNullOrEmpty(recipient) ? recipient : throw new ArgumentException("O destinatário não pode ser nulo ou vazio.");
            Message = !string.IsNullOrEmpty(message) ? message : throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
        }


        /// <summary>
        /// Construtor principal que inicializa as propriedades essenciais da notificação.
        /// </summary>
        protected NotificationModel(string userId, string type, string message)
        {
            UserId = !string.IsNullOrEmpty(userId) ? userId : throw new ArgumentException("O ID do utilizador não pode ser nulo ou vazio.");
            Type = !string.IsNullOrEmpty(type) ? type : throw new ArgumentException("O tipo de notificação não pode ser nulo ou vazio.");
            Message = !string.IsNullOrEmpty(message) ? message : throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor que mapeia os dados de um DataRow para um objeto NotificationModel.
        /// </summary>
        public NotificationModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            Type = row.Field<string>("type") ?? string.Empty;
            Message = row.Field<string>("message") ?? string.Empty;
        }

        #endregion

        #region Métodos Abstratos

        /// <summary>
        /// Método abstrato que deve ser implementado por subclasses para enviar a notificação.
        /// </summary>
        public abstract Task SendNotificationAsync();

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Processa múltiplas notificações de forma assíncrona.
        /// </summary>
        public static async Task ProcessNotificationsAsync(List<NotificationModel> notifications)
        {
            if (notifications == null || notifications.Count == 0) return;

            var tasks = new List<Task>();
            foreach (var notification in notifications)
            {
                tasks.Add(notification.SendNotificationAsync());
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Processa uma única notificação.
        /// </summary>
        public static async Task ProcessNotificationAsync(NotificationModel notification)
        {
            if (notification == null) return;
            await notification.SendNotificationAsync();
        }

        #endregion
    }
}
