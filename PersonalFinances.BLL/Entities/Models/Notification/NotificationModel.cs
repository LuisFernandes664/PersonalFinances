using System.Data;

namespace PersonalFinances.BLL.Entities.Models.Notification
{
    /// <summary>
    /// Representa uma notificação enviada a um utilizador. Contém informações sobre o tipo da notificação, a mensagem e a data de criação.
    /// </summary>
    public abstract class NotificationModel : BaseEntity
    {
        #region Propertys

        /// <summary>
        /// Identificador único do utilizador que recebeu a notificação.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Tipo da notificação. Pode ser algo como 'start', 'end', 'reminder', entre outros.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem da notificação. Contém o conteúdo informativo enviado ao utilizador.
        /// </summary>
        public string Message { get; set; } = string.Empty;


        #endregion

        public NotificationModel()  { }

        /// <summary>
        /// Construtor que permite inicializar diretamente as propriedades da notificação.
        /// </summary>
        /// <param name="userId">Identificador único do utilizador.</param>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="message">Mensagem da notificação.</param>
        /// <param name="createdAt">Data de criação da notificação.</param>
        protected NotificationModel(string userId, string type, string message)
        {
            UserId = !string.IsNullOrEmpty(userId) ? userId : throw new ArgumentException("O ID do utilizador não pode ser nulo ou vazio.");
            Type = !string.IsNullOrEmpty(type) ? type : throw new ArgumentException("O tipo de notificação não pode ser nulo ou vazio.");
            Message = !string.IsNullOrEmpty(message) ? message : throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Construtor que mapeia os dados de um DataRow para as propriedades da notificação.
        /// Utiliza o método IfExists para garantir que valores nulos ou ausentes não causem erros.
        /// </summary>
        /// <param name="row">Linha de dados que contém as informações da notificação.</param>
        public NotificationModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            Type = row.Field<string>("type") ?? string.Empty;
            Message = row.Field<string>("message") ?? string.Empty;
        }

        // A ser implementado pelas classes derivadas
        public abstract void SendNotification();

        public async Task ProcessNotificationsAsync(List<NotificationModel> notifications)
        {
            foreach (var notification in notifications)
            {
                await Task.Run(() => notification.SendNotification());
            }
        }

        public void ProcessNotifications(NotificationModel notification)
        {
            notification.SendNotification();
        }
    }

}
