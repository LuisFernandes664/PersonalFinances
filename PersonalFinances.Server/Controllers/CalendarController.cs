using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Calendar;
using PersonalFinances.BLL.Interfaces.Calendar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var events = await _calendarService.GetUserEventsAsync(userId, startDate, endDate);
            return Ok(APIResponse<IEnumerable<CalendarEventModel>>.SuccessResponse(events, "Eventos obtidos com sucesso."));
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetCalendarEvent(string eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var calendarEvent = await _calendarService.GetEventByIdAsync(eventId);

            if (calendarEvent == null)
                return NotFound(APIResponse<object>.FailResponse("Evento não encontrado."));

            if (calendarEvent.UserId != userId)
                return Forbid();

            return Ok(APIResponse<CalendarEventModel>.SuccessResponse(calendarEvent, "Evento obtido com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCalendarEvent([FromBody] CalendarEventModel calendarEvent)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            calendarEvent.UserId = userId;
            var createdEvent = await _calendarService.CreateEventAsync(calendarEvent);

            return Ok(APIResponse<CalendarEventModel>.SuccessResponse(createdEvent, "Evento criado com sucesso."));
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateCalendarEvent(string eventId, [FromBody] CalendarEventModel calendarEvent)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingEvent = await _calendarService.GetEventByIdAsync(eventId);

            if (existingEvent == null)
                return NotFound(APIResponse<object>.FailResponse("Evento não encontrado."));

            if (existingEvent.UserId != userId)
                return Forbid();

            calendarEvent.StampEntity = eventId;
            calendarEvent.UserId = userId;

            await _calendarService.UpdateEventAsync(calendarEvent);
            return Ok(APIResponse<CalendarEventModel>.SuccessResponse(calendarEvent, "Evento atualizado com sucesso."));
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteCalendarEvent(string eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingEvent = await _calendarService.GetEventByIdAsync(eventId);

            if (existingEvent == null)
                return NotFound(APIResponse<object>.FailResponse("Evento não encontrado."));

            if (existingEvent.UserId != userId)
                return Forbid();

            await _calendarService.DeleteEventAsync(eventId);
            return Ok(APIResponse<object>.SuccessResponse(null, "Evento removido com sucesso."));
        }

        [HttpGet("type/{eventType}")]
        public async Task<IActionResult> GetEventsByType(
            string eventType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var events = await _calendarService.GetEventsByTypeAsync(userId, eventType, startDate, endDate);
            return Ok(APIResponse<IEnumerable<CalendarEventModel>>.SuccessResponse(events, "Eventos obtidos com sucesso."));
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncFinancialEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            await _calendarService.SyncFinancialEventsAsync(userId);
            return Ok(APIResponse<object>.SuccessResponse(null, "Eventos financeiros sincronizados com sucesso."));
        }
    }
}