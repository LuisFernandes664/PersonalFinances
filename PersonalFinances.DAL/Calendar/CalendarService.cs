using System.Data.SqlClient;
using PersonalFinances.BLL.Entities.Models.Calendar;
using PersonalFinances.BLL.Interfaces.Calendar;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;
using PersonalFinances.BLL.Entities.Models.Transaction;
using System.Globalization;

namespace PersonalFinances.DAL.Calendar
{
    public class CalendarService : ICalendarService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;
        private readonly IGoalService _goalService;
        private readonly IRecurringTransactionService _recurringTransactionService;

        public CalendarService(
            DatabaseContext dbContext,
            ITransactionService transactionService,
            IBudgetService budgetService,
            IGoalService goalService,
            IRecurringTransactionService recurringTransactionService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
            _budgetService = budgetService;
            _goalService = goalService;
            _recurringTransactionService = recurringTransactionService;
        }

        public async Task<IEnumerable<CalendarEventModel>> GetUserEventsAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var query = @"
                SELECT * FROM CalendarEvents 
                WHERE user_id = @userId 
                AND ((start_date BETWEEN @startDate AND @endDate) 
                     OR (end_date BETWEEN @startDate AND @endDate)
                     OR (start_date <= @startDate AND (end_date IS NULL OR end_date >= @endDate)))";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@startDate", startDate),
                new SqlParameter("@endDate", endDate)
            };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var events = new List<CalendarEventModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                events.Add(new CalendarEventModel(row));
            }

            // Lidar com eventos recorrentes
            var expandedEvents = new List<CalendarEventModel>(events);
            foreach (var recurringEvent in events.Where(e => e.IsRecurring))
            {
                var expandedOccurrences = ExpandRecurringEvent(recurringEvent, startDate, endDate);
                expandedEvents.AddRange(expandedOccurrences);
            }

            // Remover os eventos recorrentes originais, pois eles foram substituídos pelas ocorrências expandidas
            expandedEvents.RemoveAll(e => e.IsRecurring && e.StartDate < startDate);

            return expandedEvents;
        }

        public async Task<CalendarEventModel> GetEventByIdAsync(string eventId)
        {
            var query = "SELECT * FROM CalendarEvents WHERE stamp_entity = @eventId";
            var parameters = new List<SqlParameter> { new SqlParameter("@eventId", eventId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
            {
                return new CalendarEventModel(result.Rows[0]);
            }

            return null;
        }

        public async Task<CalendarEventModel> CreateEventAsync(CalendarEventModel calendarEvent)
        {
            calendarEvent.StampEntity = Guid.NewGuid().ToString();
            calendarEvent.CreatedAt = DateTime.UtcNow;
            calendarEvent.UpdatedAt = DateTime.UtcNow;

            var query = @"
                INSERT INTO CalendarEvents (
                    stamp_entity, user_id, title, description, start_date, end_date, 
                    is_all_day, event_type, related_entity_id, color, is_recurring, 
                    recurrence_rule, created_at, updated_at)
                VALUES (
                    @stampEntity, @userId, @title, @description, @startDate, @endDate, 
                    @isAllDay, @eventType, @relatedEntityId, @color, @isRecurring, 
                    @recurrenceRule, @createdAt, @updatedAt)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", calendarEvent.StampEntity),
                new SqlParameter("@userId", calendarEvent.UserId),
                new SqlParameter("@title", calendarEvent.Title),
                new SqlParameter("@description", calendarEvent.Description ?? string.Empty),
                new SqlParameter("@startDate", calendarEvent.StartDate),
                new SqlParameter("@endDate", calendarEvent.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@isAllDay", calendarEvent.IsAllDay),
                new SqlParameter("@eventType", calendarEvent.EventType),
                new SqlParameter("@relatedEntityId", calendarEvent.RelatedEntityId ?? (object)DBNull.Value),
                new SqlParameter("@color", calendarEvent.Color ?? "#3498db"),
                new SqlParameter("@isRecurring", calendarEvent.IsRecurring),
                new SqlParameter("@recurrenceRule", calendarEvent.RecurrenceRule ?? (object)DBNull.Value),
                new SqlParameter("@createdAt", calendarEvent.CreatedAt),
                new SqlParameter("@updatedAt", calendarEvent.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);

            return calendarEvent;
        }

        public async Task UpdateEventAsync(CalendarEventModel calendarEvent)
        {
            calendarEvent.UpdatedAt = DateTime.UtcNow;

            var query = @"
                UPDATE CalendarEvents 
                SET title = @title, 
                    description = @description, 
                    start_date = @startDate, 
                    end_date = @endDate, 
                    is_all_day = @isAllDay, 
                    event_type = @eventType, 
                    related_entity_id = @relatedEntityId, 
                    color = @color, 
                    is_recurring = @isRecurring, 
                    recurrence_rule = @recurrenceRule, 
                    updated_at = @updatedAt
                WHERE stamp_entity = @stampEntity";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", calendarEvent.StampEntity),
                new SqlParameter("@title", calendarEvent.Title),
                new SqlParameter("@description", calendarEvent.Description ?? string.Empty),
                new SqlParameter("@startDate", calendarEvent.StartDate),
                new SqlParameter("@endDate", calendarEvent.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@isAllDay", calendarEvent.IsAllDay),
                new SqlParameter("@eventType", calendarEvent.EventType),
                new SqlParameter("@relatedEntityId", calendarEvent.RelatedEntityId ?? (object)DBNull.Value),
                new SqlParameter("@color", calendarEvent.Color ?? "#3498db"),
                new SqlParameter("@isRecurring", calendarEvent.IsRecurring),
                new SqlParameter("@recurrenceRule", calendarEvent.RecurrenceRule ?? (object)DBNull.Value),
                new SqlParameter("@updatedAt", calendarEvent.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteEventAsync(string eventId)
        {
            var query = "DELETE FROM CalendarEvents WHERE stamp_entity = @eventId";
            var parameters = new List<SqlParameter> { new SqlParameter("@eventId", eventId) };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<IEnumerable<CalendarEventModel>> GetEventsByTypeAsync(string userId, string eventType, DateTime? startDate, DateTime? endDate)
        {
            var sql = "SELECT * FROM CalendarEvents WHERE user_id = @userId AND event_type = @eventType";

            if (startDate.HasValue)
            {
                sql += " AND start_date >= @startDate";
            }

            if (endDate.HasValue)
            {
                sql += " AND (end_date IS NULL OR end_date <= @endDate)";
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@eventType", eventType)
            };

            if (startDate.HasValue)
            {
                parameters.Add(new SqlParameter("@startDate", startDate.Value));
            }

            if (endDate.HasValue)
            {
                parameters.Add(new SqlParameter("@endDate", endDate.Value));
            }

            var result = await SQLHelper.ExecuteQueryAsync(sql, parameters);
            var events = new List<CalendarEventModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                events.Add(new CalendarEventModel(row));
            }

            return events;
        }

        public async Task SyncFinancialEventsAsync(string userId)
        {

            using (var connection = new SqlConnection(ConfigManager.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Excluir eventos financeiros existentes do sistema (não eventos criados manualmente)
                        var query = @"
                    DELETE FROM CalendarEvents 
                    WHERE user_id = @userId 
                    AND event_type IN ('Budget', 'Goal', 'RecurringTransaction')";
                        var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };

                        await SQLHelper.ExecuteNonQueryAsync(query, parameters, transaction);

                        // Sincronizar orçamentos
                        var budgets = await _budgetService.GetBudgetsByUserAsync(userId);
                        foreach (var budget in budgets)
                        {
                            var budgetEvent = new CalendarEventModel
                            {
                                UserId = userId,
                                Title = $"Fim do orçamento: {await GetCategoryName(budget.CategoryId)}",
                                Description = $"Orçado: {budget.ValorOrcado:C} | Gastado: {await _budgetService.GetSpentAmountByBudget(budget.StampEntity):C}",
                                StartDate = budget.DataInicio,
                                EndDate = budget.DataFim,
                                IsAllDay = true,
                                EventType = "Budget",
                                RelatedEntityId = budget.StampEntity,
                                Color = "#e74c3c", // Vermelho
                                IsRecurring = false
                            };

                            await CreateEventAsync(budgetEvent);
                        }

                        // Sincronizar metas
                        var goals = await _goalService.GetGoalsByUserAsync(userId);
                        foreach (var goal in goals)
                        {
                            var progress = await _goalService.GetGoalProgressPercentage(goal.StampEntity);

                            var goalEvent = new CalendarEventModel
                            {
                                UserId = userId,
                                Title = $"Meta: {goal.Descricao}",
                                Description = $"Alvo: {goal.ValorAlvo:C} | Progresso: {progress}%",
                                StartDate = DateTime.Now,
                                EndDate = goal.DataLimite,
                                IsAllDay = true,
                                EventType = "Goal",
                                RelatedEntityId = goal.StampEntity,
                                Color = "#27ae60", // Verde
                                IsRecurring = false
                            };

                            await CreateEventAsync(goalEvent);
                        }

                        // Sincronizar transações recorrentes
                        var recurringTransactions = await _recurringTransactionService.GetUserRecurringTransactionsAsync(userId);
                        foreach (var recurringTx in recurringTransactions.Where(t => t.IsActive))
                        {
                            var eventType = recurringTx.Category == "income" ? "Income" : "Bill";
                            var color = recurringTx.Category == "income" ? "#2ecc71" : "#e74c3c";

                            // Criar regra de recorrência com base no tipo de recorrência
                            string rrule = GenerateRecurrenceRule(recurringTx);

                            var transactionEvent = new CalendarEventModel
                            {
                                UserId = userId,
                                Title = recurringTx.Description,
                                Description = $"Valor: {Math.Abs(recurringTx.Amount):C}",
                                StartDate = recurringTx.StartDate,
                                EndDate = recurringTx.EndDate,
                                IsAllDay = true,
                                EventType = eventType,
                                RelatedEntityId = recurringTx.StampEntity,
                                Color = color,
                                IsRecurring = true,
                                RecurrenceRule = rrule
                            };

                            await CreateEventAsync(transactionEvent);
                        }

                        // Commit transaction
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Métodos auxiliares
        private IEnumerable<CalendarEventModel> ExpandRecurringEvent(CalendarEventModel recurringEvent, DateTime startDate, DateTime endDate)
        {
            // Implementação simples - em uma aplicação real, você usaria uma biblioteca como Ical.Net
            var expandedEvents = new List<CalendarEventModel>();

            // Analisar a regra de recorrência
            if (string.IsNullOrEmpty(recurringEvent.RecurrenceRule))
            {
                return expandedEvents;
            }

            var rruleParts = recurringEvent.RecurrenceRule.Split(';')
                .Select(part => part.Split('='))
                .ToDictionary(parts => parts[0], parts => parts.Length > 1 ? parts[1] : string.Empty);

            string freq = rruleParts.ContainsKey("FREQ") ? rruleParts["FREQ"] : "MONTHLY";
            int interval = rruleParts.ContainsKey("INTERVAL") && int.TryParse(rruleParts["INTERVAL"], out int i) ? i : 1;
            DateTime? until = null;

            if (rruleParts.ContainsKey("UNTIL") && DateTime.TryParseExact(rruleParts["UNTIL"], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime untilDate))
            {
                until = untilDate;
            }

            // Calcular ocorrências
            DateTime current = recurringEvent.StartDate;
            while (current <= endDate && (until == null || current <= until))
            {
                if (current >= startDate)
                {
                    // Criar uma nova instância para esta ocorrência
                    var eventInstance = new CalendarEventModel
                    {
                        StampEntity = $"{recurringEvent.StampEntity}_{current:yyyyMMdd}",
                        UserId = recurringEvent.UserId,
                        Title = recurringEvent.Title,
                        Description = recurringEvent.Description,
                        StartDate = current,
                        EndDate = recurringEvent.EndDate.HasValue ? current.AddDays((recurringEvent.EndDate.Value - recurringEvent.StartDate).Days) : null,
                        IsAllDay = recurringEvent.IsAllDay,
                        EventType = recurringEvent.EventType,
                        RelatedEntityId = recurringEvent.RelatedEntityId,
                        Color = recurringEvent.Color,
                        IsRecurring = false, // Esta é uma instância, não o modelo recorrente
                        CreatedAt = recurringEvent.CreatedAt,
                        UpdatedAt = recurringEvent.UpdatedAt
                    };

                    expandedEvents.Add(eventInstance);
                }

                // Avançar para a próxima ocorrência
                switch (freq)
                {
                    case "DAILY":
                        current = current.AddDays(interval);
                        break;
                    case "WEEKLY":
                        current = current.AddDays(7 * interval);
                        break;
                    case "MONTHLY":
                        current = current.AddMonths(interval);
                        break;
                    case "YEARLY":
                        current = current.AddYears(interval);
                        break;
                    default:
                        current = current.AddMonths(interval);
                        break;
                }
            }

            return expandedEvents;
        }

        private string GenerateRecurrenceRule(RecurringTransactionModel transaction)
        {
            string freq;
            int interval = transaction.RecurrenceInterval;

            switch (transaction.RecurrenceType)
            {
                case RecurrenceType.Daily:
                    freq = "DAILY";
                    break;
                case RecurrenceType.Weekly:
                    freq = "WEEKLY";
                    break;
                case RecurrenceType.Monthly:
                    freq = "MONTHLY";
                    break;
                case RecurrenceType.Yearly:
                    freq = "YEARLY";
                    break;
                default:
                    freq = "MONTHLY";
                    break;
            }

            var rrule = $"FREQ={freq};INTERVAL={interval}";

            if (transaction.EndDate.HasValue)
            {
                rrule += $";UNTIL={transaction.EndDate.Value:yyyyMMdd}";
            }

            return rrule;
        }

        private async Task<string> GetCategoryName(string categoryId)
        {
            var query = "SELECT name FROM Categories WHERE stamp_entity = @categoryId";
            var parameters = new List<SqlParameter> { new SqlParameter("@categoryId", categoryId) };

            var result = await SQLHelper.ExecuteScalarAsync(query, parameters);
            return result != null && result.Table.Columns.Contains("name") ? result["name"].ToString() : "Categoria desconhecida";
        }
    }
}