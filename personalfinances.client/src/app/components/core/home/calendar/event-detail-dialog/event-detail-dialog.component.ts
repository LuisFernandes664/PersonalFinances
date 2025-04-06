import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CalendarEvent } from '../../models/calendar-event.model';

@Component({
  selector: 'app-event-detail-dialog',
  templateUrl: './event-detail-dialog.component.html',
  styleUrls: ['./event-detail-dialog.component.scss']
})
export class EventDetailDialogComponent implements OnInit {
  isEditMode: boolean = false;
  eventForm: FormGroup;

  // Tipos de eventos disponíveis
  eventTypes = [
    { value: 'personal', label: 'Evento Pessoal', color: '#2196f3' },
    { value: 'recurring_payment', label: 'Pagamento Recorrente', color: '#4caf50' },
    { value: 'bill_due', label: 'Vencimento de Conta', color: '#f44336' },
    { value: 'goal', label: 'Meta Financeira', color: '#ff9800' },
    { value: 'budget', label: 'Orçamento', color: '#9c27b0' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<EventDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { event: CalendarEvent }
  ) {
    this.eventForm = this.fb.group({
      title: [data.event.title, [Validators.required, Validators.maxLength(100)]],
      description: [data.event.description, Validators.maxLength(500)],
      startDate: [new Date(data.event.startDate), Validators.required],
      endDate: [data.event.endDate ? new Date(data.event.endDate) : null],
      isAllDay: [data.event.isAllDay],
      eventType: [data.event.eventType, Validators.required],
      color: [data.event.color, Validators.required],
      isRecurring: [data.event.isRecurring],
      recurrenceRule: [data.event.recurrenceRule || '']
    });
  }

  ngOnInit(): void {
    // Desabilitar o formulário inicialmente (modo visualização)
    this.eventForm.disable();

    // Atualizar a cor quando o tipo de evento mudar
    this.eventForm.get('eventType')?.valueChanges.subscribe(value => {
      if (this.isEditMode) {  // Apenas em modo de edição
        const eventType = this.eventTypes.find(type => type.value === value);
        if (eventType) {
          this.eventForm.patchValue({ color: eventType.color });
        }
      }
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;

    if (this.isEditMode) {
      this.eventForm.enable();
    } else {
      this.eventForm.disable();
      // Restaurar valores originais
      this.eventForm.patchValue({
        title: this.data.event.title,
        description: this.data.event.description,
        startDate: new Date(this.data.event.startDate),
        endDate: this.data.event.endDate ? new Date(this.data.event.endDate) : null,
        isAllDay: this.data.event.isAllDay,
        eventType: this.data.event.eventType,
        color: this.data.event.color,
        isRecurring: this.data.event.isRecurring,
        recurrenceRule: this.data.event.recurrenceRule || ''
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onDelete(): void {
    if (confirm('Tem certeza que deseja excluir este evento?')) {
      this.dialogRef.close({ action: 'delete' });
    }
  }

  onSave(): void {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    const formValues = this.eventForm.value;

    // Criar objeto de evento atualizado
    const updatedEvent: CalendarEvent = {
      ...this.data.event,
      title: formValues.title,
      description: formValues.description,
      startDate: formValues.startDate,
      endDate: formValues.isAllDay ? formValues.startDate : formValues.endDate,
      isAllDay: formValues.isAllDay,
      eventType: formValues.eventType,
      color: formValues.color,
      isRecurring: formValues.isRecurring,
      recurrenceRule: formValues.isRecurring ? formValues.recurrenceRule : undefined
    };

    this.dialogRef.close({ action: 'update', event: updatedEvent });
  }

  // Métodos auxiliares
  getEventTypeLabel(value: string): string {
    const eventType = this.eventTypes.find(type => type.value === value);
    return eventType ? eventType.label : '';
  }

  formatDate(date: string | Date | null): string {
    if (!date) return '';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('pt-PT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
