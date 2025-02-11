using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Notification
{
    public interface IAsyncNotification
    {
        Task SendNotificationAsync();
    }
}
