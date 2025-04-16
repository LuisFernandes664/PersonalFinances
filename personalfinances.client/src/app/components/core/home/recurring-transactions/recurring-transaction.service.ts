import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { RecurringTransaction, RecurrenceType } from '../models/recurring-transaction.model';
import { APIResponse } from '../../../../models/api-response.model';
import { environment } from '../../../../../environments/environment';
import { NotificationService } from '../../../shared/notifications/notification.service';

@Injectable({
  providedIn: 'root'
})
export class RecurringTransactionService {
  private apiUrl = `${environment.apiUrl}/RecurringTransactions`;

  // BehaviorSubject to hold and share recurring transactions
  private recurringTransactionsSubject = new BehaviorSubject<RecurringTransaction[]>([]);
  recurringTransactions$ = this.recurringTransactionsSubject.asObservable();

  // Metadata for UI display
  recurrenceTypeLabels = {
    [RecurrenceType.Daily]: 'Diário',
    [RecurrenceType.Weekly]: 'Semanal',
    [RecurrenceType.Monthly]: 'Mensal',
    [RecurrenceType.Yearly]: 'Anual'
  };

  paymentMethods = [
    { value: 'cash', text: 'Dinheiro' },
    { value: 'credit_card', text: 'Cartão de Crédito' },
    { value: 'debit_card', text: 'Cartão de Débito' },
    { value: 'bank_transfer', text: 'Transferência Bancária' }
  ];

  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) {
    this.loadRecurringTransactions();
  }

  // Get all recurring transactions for the current user
  getRecurringTransactions(): Observable<APIResponse<RecurringTransaction[]>> {
    return this.http.get<APIResponse<RecurringTransaction[]>>(this.apiUrl).pipe(
      tap(response => {
        if (response.success) {
          this.recurringTransactionsSubject.next(response.data);
        }
      })
    );
  }

  // Get a specific recurring transaction by ID
  getRecurringTransaction(id: string): Observable<APIResponse<RecurringTransaction>> {
    return this.http.get<APIResponse<RecurringTransaction>>(`${this.apiUrl}/${id}`);
  }

  // Create a new recurring transaction
  createRecurringTransaction(transaction: RecurringTransaction): Observable<APIResponse<RecurringTransaction>> {
    return this.http.post<APIResponse<RecurringTransaction>>(this.apiUrl, transaction).pipe(
      tap(response => {
        if (response.success) {
          const currentTransactions = this.recurringTransactionsSubject.getValue();
          this.recurringTransactionsSubject.next([...currentTransactions, response.data]);
          this.notificationService.showToast('success', 'Transação recorrente criada com sucesso!');
        }
      })
    );
  }

  // Update an existing recurring transaction
  updateRecurringTransaction(id: string, transaction: RecurringTransaction): Observable<APIResponse<RecurringTransaction>> {
    return this.http.put<APIResponse<RecurringTransaction>>(`${this.apiUrl}/${id}`, transaction).pipe(
      tap(response => {
        if (response.success) {
          const currentTransactions = this.recurringTransactionsSubject.getValue();
          const updatedTransactions = currentTransactions.map(t =>
            t.stampEntity === id ? { ...t, ...transaction } : t
          );
          this.recurringTransactionsSubject.next(updatedTransactions);
          this.notificationService.showToast('success', 'Transação recorrente atualizada com sucesso!');
        }
      })
    );
  }

  // Delete a recurring transaction
  deleteRecurringTransaction(id: string): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`).pipe(
      tap(response => {
        if (response.success) {
          const currentTransactions = this.recurringTransactionsSubject.getValue();
          this.recurringTransactionsSubject.next(
            currentTransactions.filter(t => t.stampEntity !== id)
          );
          this.notificationService.showToast('success', 'Transação recorrente excluída com sucesso!');
        }
      })
    );
  }

  // Get the current state of recurring transactions
  getCurrentRecurringTransactions(): RecurringTransaction[] {
    return this.recurringTransactionsSubject.getValue();
  }

  // Load all recurring transactions
  private loadRecurringTransactions(): void {
    this.getRecurringTransactions().subscribe(
      response => {
        // Data already set in subject via tap operator
      },
      error => {
        console.error('Erro ao carregar transações recorrentes:', error);
        this.notificationService.showToast('error', 'Erro ao carregar transações recorrentes');
      }
    );
  }

  // Calculate next occurrence date based on recurrence pattern
  getNextOccurrenceDate(transaction: RecurringTransaction): Date {
    const lastProcessed = transaction.lastProcessedDate ?
      new Date(transaction.lastProcessedDate) :
      new Date(transaction.startDate);

    const now = new Date();
    let nextDate = new Date(lastProcessed);

    switch (transaction.recurrenceType) {
      case RecurrenceType.Daily:
        nextDate.setDate(nextDate.getDate() + transaction.recurrenceInterval);
        break;
      case RecurrenceType.Weekly:
        nextDate.setDate(nextDate.getDate() + (7 * transaction.recurrenceInterval));
        break;
      case RecurrenceType.Monthly:
        nextDate.setMonth(nextDate.getMonth() + transaction.recurrenceInterval);
        break;
      case RecurrenceType.Yearly:
        nextDate.setFullYear(nextDate.getFullYear() + transaction.recurrenceInterval);
        break;
    }

    return nextDate;
  }

  // Format a friendly description of the recurrence pattern
  getRecurrenceDescription(transaction: RecurringTransaction): string {
    const typeName = this.recurrenceTypeLabels[transaction.recurrenceType];

    if (transaction.recurrenceInterval === 1) {
      return `A cada ${typeName.toLowerCase()}`;
    } else {
      return `A cada ${transaction.recurrenceInterval} ${typeName.toLowerCase()}s`;
    }
  }
}
