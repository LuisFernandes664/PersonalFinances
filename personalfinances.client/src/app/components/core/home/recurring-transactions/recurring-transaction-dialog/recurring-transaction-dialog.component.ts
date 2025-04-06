import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RecurrenceType, RecurringTransaction } from '../../models/recurring-transaction.model';
import { SelectListItem } from '../../../../../models/select-list-item.model';

@Component({
  selector: 'app-recurring-transaction-dialog',
  templateUrl: './recurring-transaction-dialog.component.html',
  styleUrls: ['./recurring-transaction-dialog.component.scss']
})
export class RecurringTransactionDialogComponent implements OnInit {
  recurringTransactionForm: FormGroup;
  isEditing: boolean = false;
  paymentMethods: SelectListItem[] = [];
  recurrenceTypes: { value: RecurrenceType, label: string }[] = [];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<RecurringTransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.paymentMethods = data.paymentMethods || [];
    this.recurrenceTypes = data.recurrenceTypes || [];
    this.isEditing = data.isEditing || false;

    // Inicializar formulário
    this.recurringTransactionForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(100)]],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      category: ['expense', Validators.required],
      paymentMethod: ['', Validators.required],
      recipient: ['', Validators.maxLength(100)],
      recurrenceType: [RecurrenceType.Monthly, Validators.required],
      recurrenceInterval: [1, [Validators.required, Validators.min(1)]],
      startDate: [new Date(), Validators.required],
      endDate: [null],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    // Se for edição, preencher o formulário com os dados da transação
    if (this.isEditing && this.data.transaction) {
      const transaction = this.data.transaction;

      // Converter datas de string para objeto Date
      const startDate = transaction.startDate ? new Date(transaction.startDate) : null;
      const endDate = transaction.endDate ? new Date(transaction.endDate) : null;

      this.recurringTransactionForm.patchValue({
        description: transaction.description,
        amount: Math.abs(transaction.amount), // sempre mostra valor positivo no formulário
        category: transaction.category,
        paymentMethod: transaction.paymentMethod,
        recipient: transaction.recipient,
        recurrenceType: transaction.recurrenceType,
        recurrenceInterval: transaction.recurrenceInterval,
        startDate: startDate,
        endDate: endDate,
        isActive: transaction.isActive
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.recurringTransactionForm.invalid) {
      this.recurringTransactionForm.markAllAsTouched();
      return;
    }

    const formValues = this.recurringTransactionForm.value;

    // Ajusta o valor com base na categoria (negativo para despesas)
    const amount = formValues.category === 'expense'
      ? -Math.abs(formValues.amount)
      : Math.abs(formValues.amount);

    const recurringTransaction: RecurringTransaction = {
      ...this.isEditing ? this.data.transaction : {},
      description: formValues.description,
      amount: amount,
      category: formValues.category,
      paymentMethod: formValues.paymentMethod,
      recipient: formValues.recipient,
      recurrenceType: formValues.recurrenceType,
      recurrenceInterval: formValues.recurrenceInterval,
      startDate: formValues.startDate,
      endDate: formValues.endDate,
      isActive: formValues.isActive,
    };

    this.dialogRef.close(recurringTransaction);
  }

  getRecurrenceDescription(): string {
    const form = this.recurringTransactionForm.value;
    const interval = form.recurrenceInterval;
    const type = form.recurrenceType;

    const recurrenceType = this.recurrenceTypes.find(rt => rt.value === type);
    const typeName = recurrenceType ? recurrenceType.label.toLowerCase() : '';

    if (interval === 1) {
      return `A cada ${typeName.slice(0, -1)}`;  // Remove o 'l' do final (mensal -> mensa)
    } else {
      return `A cada ${interval} ${typeName}`;
    }
  }
}
