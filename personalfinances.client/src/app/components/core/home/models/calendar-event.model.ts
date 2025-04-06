export interface CalendarEvent {
  stampEntity: string;
  userId: string;
  title: string;
  description: string;
  startDate: Date | string;
  endDate?: Date | string;
  isAllDay: boolean;
  eventType: string;
  relatedEntityId?: string;
  color: string;
  isRecurring: boolean;
  recurrenceRule?: string;
}
