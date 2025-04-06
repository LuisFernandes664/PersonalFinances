import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CalendarEvent } from '../../models/calendar-event.model';

@Component({
  selector: 'app-create-event-dialog',
  templateUrl: './create-event-dialog.component.html',
  styleUrls: ['./create-event-dialog.component.scss']
})
export class CreateEventDialogComponent implements OnInit {
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
    public dialogRef: MatDialogRef<CreateEventDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.eventForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)],
      startDate: [data.startDate, Validators.required],
      endDate: [data.endDate],
      isAllDay: [data.allDay],
      eventType: ['personal', Validators.required],
      color: [this.eventTypes[0].color, Validators.required],
      isRecurring: [false],
      recurrenceRule: ['']
    });
  }

  ngOnInit(): void {
    // Atualizar a cor quando o tipo de evento mudar
    this.eventForm.get('eventType')?.valueChanges.subscribe(value => {
      const eventType = this.eventTypes.find(type => type.value === value);
      if (eventType) {
        this.eventForm.patchValue({ color: eventType.color });
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    const formValues = this.eventForm.value;

    // Criar objeto de evento
    const event: CalendarEvent = {
      stampEntity: '', // Será preenchido pelo backend
      userId: '', // Será preenchido pelo backend com o usuário autenticado
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

    this.dialogRef.close(event);
  }

  // Métodos auxiliares
  getEventTypeLabel(value: string): string {
    const eventType = this.eventTypes.find(type => type.value === value);
    return eventType ? eventType.label : '';
  }
}
