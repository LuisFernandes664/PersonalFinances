
using PersonalFinances.BLL.Interfaces.Notification.Senders;

namespace PersonalFinances.BLL.Entities.Models.Notification.Senders
{
    public class SmsSender : ISmsSender
    {

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }
    }

}
