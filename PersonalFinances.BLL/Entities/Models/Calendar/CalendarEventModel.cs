using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Calendar
{
    public class CalendarEventModel : BaseEntity
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsAllDay { get; set; }
        public string EventType { get; set; }
        public string RelatedEntityId { get; set; }
        public string Color { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurrenceRule { get; set; }

        public CalendarEventModel() : base()
        {
            UserId = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            EventType = "General";
            Color = "#3498db";
            IsAllDay = true;
            IsRecurring = false;
        }

        public CalendarEventModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            Title = row.Field<string>("title") ?? string.Empty;
            Description = row.Field<string>("description") ?? string.Empty;
            StartDate = row.Field<DateTime>("start_date");
            EndDate = row.Field<DateTime?>("end_date");
            IsAllDay = row.Field<bool>("is_all_day");
            EventType = row.Field<string>("event_type") ?? "General";
            RelatedEntityId = row.Field<string>("related_entity_id");
            Color = row.Field<string>("color") ?? "#3498db";
            IsRecurring = row.Field<bool>("is_recurring");
            RecurrenceRule = row.Field<string>("recurrence_rule");
        }
    }

}
