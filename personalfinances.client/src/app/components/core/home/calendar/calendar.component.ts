import { Component, OnInit, ViewChild } from '@angular/core';
import { CalendarOptions, Calendar, EventClickArg, DateSelectArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { MatDialog } from '@angular/material/dialog';
import { CalendarService } from '../services/calendar.service';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { CalendarEvent } from '../models/calendar-event.model';
import { CreateEventDialogComponent } from './create-event-dialog/create-event-dialog.component';
import { EventDetailDialogComponent } from './event-detail-dialog/event-detail-dialog.component';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  @ViewChild('calendar') calendarComponent: any;

  isLoading: boolean = false;
  selectedViewType: string = 'month'; // 'month', 'week', 'day'

  // Configurações do calendário
  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    editable: true,
    selectable: true,
    selectMirror: true,
    dayMaxEvents: true,
    weekends: true,
    events: [],
    select: this.handleDateSelect.bind(this),
    eventClick: this.handleEventClick.bind(this),
    eventColor: '#2196f3'
  };

  constructor(
    private calendarService: CalendarService,
    private dialog: MatDialog,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;

    // Obter datas de início e fim com base na visualização atual
    const calendarApi = this.calendarComponent?.getApi();
    let startDate = new Date();
    let endDate = new Date();

    if (calendarApi) {
      startDate = calendarApi.view.activeStart;
      endDate = calendarApi.view.activeEnd;
    } else {
      // Sem API do calendário disponível, usar intervalo padrão (3 meses)
      startDate = new Date();
      startDate.setMonth(startDate.getMonth() - 1);
      endDate = new Date();
      endDate.setMonth(endDate.getMonth() + 2);
    }

    this.calendarService.getEvents(startDate, endDate).subscribe(
      response => {
        const events = response.data.map(event => this.mapToCalendarEvent(event));
        this.calendarOptions = {
          ...this.calendarOptions,
          events: events
        };
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao carregar eventos. Por favor, tente novamente.');
        console.error('Erro ao carregar eventos:', error);
      }
    );
  }

  // Converter evento do backend para formato do FullCalendar
  mapToCalendarEvent(event: CalendarEvent): any {
    return {
      id: event.stampEntity,
      title: event.title,
      start: event.startDate,
      end: event.endDate,
      allDay: event.isAllDay,
      backgroundColor: event.color,
      borderColor: event.color,
      extendedProps: {
        description: event.description,
        eventType: event.eventType,
        relatedEntityId: event.relatedEntityId
      }
    };
  }

  handleDateSelect(selectInfo: DateSelectArg): void {
    const dialogRef = this.dialog.open(CreateEventDialogComponent, {
      width: '500px',
      data: {
        startDate: selectInfo.start,
        endDate: selectInfo.end,
        allDay: selectInfo.allDay
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.calendarService.createEvent(result).subscribe(
          response => {
            this.notificationService.showToast('success', 'Evento criado com sucesso!');
            this.loadEvents();
          },
          error => {
            this.notificationService.showToast('error', 'Erro ao criar evento. Por favor, tente novamente.');
            console.error('Erro ao criar evento:', error);
          }
        );
      }
    });
  }

  handleEventClick(clickInfo: EventClickArg): void {
    // Obter o ID do evento e buscar detalhes completos
    const eventId = clickInfo.event.id;

    this.calendarService.getEvent(eventId).subscribe(
      response => {
        const dialogRef = this.dialog.open(EventDetailDialogComponent, {
          width: '500px',
          data: { event: response.data }
        });

        dialogRef.afterClosed().subscribe(result => {
          if (result) {
            if (result.action === 'delete') {
              this.deleteEvent(eventId);
            } else if (result.action === 'update') {
              this.updateEvent(result.event);
            }
          }
        });
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao carregar detalhes do evento. Por favor, tente novamente.');
        console.error('Erro ao carregar detalhes do evento:', error);
      }
    );
  }

  createEvent(): void {
    const dialogRef = this.dialog.open(CreateEventDialogComponent, {
      width: '500px',
      data: {
        startDate: new Date(),
        endDate: new Date(new Date().getTime() + 60 * 60 * 1000), // +1 hora
        allDay: false
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.calendarService.createEvent(result).subscribe(
          response => {
            this.notificationService.showToast('success', 'Evento criado com sucesso!');
            this.loadEvents();
          },
          error => {
            this.notificationService.showToast('error', 'Erro ao criar evento. Por favor, tente novamente.');
            console.error('Erro ao criar evento:', error);
          }
        );
      }
    });
  }

  updateEvent(event: CalendarEvent): void {
    this.calendarService.updateEvent(event.stampEntity, event).subscribe(
      response => {
        this.notificationService.showToast('success', 'Evento atualizado com sucesso!');
        this.loadEvents();
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao atualizar evento. Por favor, tente novamente.');
        console.error('Erro ao atualizar evento:', error);
      }
    );
  }

  deleteEvent(eventId: string): void {
    this.calendarService.deleteEvent(eventId).subscribe(
      response => {
        this.notificationService.showToast('success', 'Evento excluído com sucesso!');
        this.loadEvents();
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao excluir evento. Por favor, tente novamente.');
        console.error('Erro ao excluir evento:', error);
      }
    );
  }

  syncFinancialEvents(): void {
    this.isLoading = true;
    this.calendarService.syncFinancialEvents().subscribe(
      response => {
        this.notificationService.showToast('success', 'Eventos financeiros sincronizados com sucesso!');
        this.loadEvents();
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao sincronizar eventos financeiros. Por favor, tente novamente.');
        console.error('Erro ao sincronizar eventos:', error);
      }
    );
  }

  changeView(viewType: string): void {
    this.selectedViewType = viewType;

    const calendarApi = this.calendarComponent.getApi();
    if (viewType === 'month') {
      calendarApi.changeView('dayGridMonth');
    } else if (viewType === 'week') {
      calendarApi.changeView('timeGridWeek');
    } else if (viewType === 'day') {
      calendarApi.changeView('timeGridDay');
    }
  }
}
