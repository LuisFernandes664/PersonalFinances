import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CalendarEvent } from '../models/calendar-event.model';
import { APIResponse } from '../../../../models/api-response.model';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private apiUrl = `${environment.apiUrl}/Calendar`;

  constructor(private http: HttpClient) { }

  getEvents(startDate: Date, endDate: Date): Observable<APIResponse<CalendarEvent[]>> {
    const start = this.formatDate(startDate);
    const end = this.formatDate(endDate);
    return this.http.get<APIResponse<CalendarEvent[]>>(`${this.apiUrl}?startDate=${start}&endDate=${end}`);
  }

  getEvent(id: string): Observable<APIResponse<CalendarEvent>> {
    return this.http.get<APIResponse<CalendarEvent>>(`${this.apiUrl}/${id}`);
  }

  createEvent(event: CalendarEvent): Observable<APIResponse<CalendarEvent>> {
    return this.http.post<APIResponse<CalendarEvent>>(this.apiUrl, event);
  }

  updateEvent(id: string, event: CalendarEvent): Observable<APIResponse<CalendarEvent>> {
    return this.http.put<APIResponse<CalendarEvent>>(`${this.apiUrl}/${id}`, event);
  }

  deleteEvent(id: string): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`);
  }

  getEventsByType(eventType: string, startDate?: Date, endDate?: Date): Observable<APIResponse<CalendarEvent[]>> {
    let url = `${this.apiUrl}/type/${eventType}`;

    if (startDate && endDate) {
      const start = this.formatDate(startDate);
      const end = this.formatDate(endDate);
      url += `?startDate=${start}&endDate=${end}`;
    }

    return this.http.get<APIResponse<CalendarEvent[]>>(url);
  }

  syncFinancialEvents(): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}/sync`, {});
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
