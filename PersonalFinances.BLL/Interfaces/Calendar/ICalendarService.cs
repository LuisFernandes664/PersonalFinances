using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Entities.Models.Calendar;

namespace PersonalFinances.BLL.Interfaces.Calendar
{
    public interface ICalendarService
    {
        Task<IEnumerable<CalendarEventModel>> GetUserEventsAsync(string userId, DateTime startDate, DateTime endDate);
        Task<CalendarEventModel> GetEventByIdAsync(string eventId);
        Task<CalendarEventModel> CreateEventAsync(CalendarEventModel calendarEvent);
        Task UpdateEventAsync(CalendarEventModel calendarEvent);
        Task DeleteEventAsync(string eventId);
        Task<IEnumerable<CalendarEventModel>> GetEventsByTypeAsync(string userId, string eventType, DateTime? startDate, DateTime? endDate);
        Task SyncFinancialEventsAsync(string userId);
    }

}
