import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TransactionService } from '../transaction.service';
import { RecurrenceType, RecurringTransaction } from '../models/recurring-transaction.model';
import { SelectListItem } from '../../../../models/select-list-item.model';
import { RecurringTransactionService } from './recurring-transaction.service';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { RecurringTransactionDialogComponent } from './recurring-transaction-dialog/recurring-transaction-dialog.component';

@Component({
  selector: 'app-recurring-transactions',
  templateUrl: './recurring-transactions.component.html',
  styleUrls: ['./recurring-transactions.component.scss']
})
export class RecurringTransactionsComponent implements OnInit {
  recurringTransactions: RecurringTransaction[] = [];
  filteredTransactions: RecurringTransaction[] = [];
  isLoading: boolean = false;
  filterForm: FormGroup;

  // Define os tipos de recorrência para exibição no template
  recurrenceTypes = [
    { value: RecurrenceType.Daily, label: 'Diário' },
    { value: RecurrenceType.Weekly, label: 'Semanal' },
    { value: RecurrenceType.Monthly, label: 'Mensal' },
    { value: RecurrenceType.Yearly, label: 'Anual' }
  ];

  paymentMethods: SelectListItem[] = [
    { value: 'cash', text: 'Dinheiro' },
    { value: 'credit_card', text: 'Cartão de Crédito' },
    { value: 'debit_card', text: 'Cartão de Débito' },
    { value: 'bank_transfer', text: 'Transferência Bancária' }
  ];

  constructor(
    private recurringTransactionService: RecurringTransactionService,
    private transactionService: TransactionService,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {
    this.filterForm = this.fb.group({
      active: [true],
      category: ['all'],
      recurrenceType: ['all']
    });
  }

  ngOnInit(): void {
    this.loadRecurringTransactions();

    // Subscribe para mudanças no formulário de filtro
    this.filterForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }

  loadRecurringTransactions(): void {
    this.isLoading = true;
    this.recurringTransactionService.getRecurringTransactions()
      .subscribe(
        response => {
          this.recurringTransactions = response.data;
          this.applyFilters();
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          this.notificationService.showToast('error', 'Erro ao carregar transações recorrentes. Por favor, tente novamente.');
          console.error('Erro ao carregar transações recorrentes:', error);
        }
      );
  }

  applyFilters(): void {
    const filterValues = this.filterForm.value;

    this.filteredTransactions = this.recurringTransactions.filter(transaction => {
      // Filtro de status ativo/inativo
      if (filterValues.active !== null && transaction.isActive !== filterValues.active) {
        return false;
      }

      // Filtro de categoria
      if (filterValues.category !== 'all' && transaction.category !== filterValues.category) {
        return false;
      }

      // Filtro de tipo de recorrência
      if (filterValues.recurrenceType !== 'all' &&
          transaction.recurrenceType !== parseInt(filterValues.recurrenceType)) {
        return false;
      }

      return true;
    });
  }

  openNewTransactionDialog(): void {
    const dialogRef = this.dialog.open(RecurringTransactionDialogComponent, {
      width: '600px',
      data: {
        paymentMethods: this.paymentMethods,
        recurrenceTypes: this.recurrenceTypes
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.createRecurringTransaction(result);
      }
    });
  }

  openEditDialog(transaction: RecurringTransaction): void {
    const dialogRef = this.dialog.open(RecurringTransactionDialogComponent, {
      width: '600px',
      data: {
        transaction: transaction,
        paymentMethods: this.paymentMethods,
        recurrenceTypes: this.recurrenceTypes,
        isEditing: true
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateRecurringTransaction(transaction.stampEntity, result);
      }
    });
  }

  createRecurringTransaction(transaction: RecurringTransaction): void {
    this.isLoading = true;
    this.recurringTransactionService.createRecurringTransaction(transaction)
      .subscribe(
        response => {
          this.notificationService.showToast('success', 'Transação recorrente criada com sucesso!');
          this.loadRecurringTransactions();
        },
        error => {
          this.isLoading = false;
          this.notificationService.showToast('error', 'Erro ao criar transação recorrente. Por favor, tente novamente.');
          console.error('Erro ao criar transação recorrente:', error);
        }
      );
  }

  updateRecurringTransaction(id: string, transaction: RecurringTransaction): void {
    this.isLoading = true;
    this.recurringTransactionService.updateRecurringTransaction(id, transaction)
      .subscribe(
        response => {
          this.notificationService.showToast('success', 'Transação recorrente atualizada com sucesso!');
          this.loadRecurringTransactions();
        },
        error => {
          this.isLoading = false;
          this.notificationService.showToast('error', 'Erro ao atualizar transação recorrente. Por favor, tente novamente.');
          console.error('Erro ao atualizar transação recorrente:', error);
        }
      );
  }

  toggleTransactionStatus(transaction: RecurringTransaction): void {
    const updatedTransaction = { ...transaction, isActive: !transaction.isActive };
    this.updateRecurringTransaction(transaction.stampEntity, updatedTransaction);
  }

  deleteRecurringTransaction(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta transação recorrente? Esta ação não pode ser desfeita.')) {
      this.isLoading = true;
      this.recurringTransactionService.deleteRecurringTransaction(id)
        .subscribe(
          response => {
            this.notificationService.showToast('success', 'Transação recorrente excluída com sucesso!');
            this.loadRecurringTransactions();
          },
          error => {
            this.isLoading = false;
            this.notificationService.showToast('error', 'Erro ao excluir transação recorrente. Por favor, tente novamente.');
            console.error('Erro ao excluir transação recorrente:', error);
          }
        );
    }
  }

  getRecurrenceTypeLabel(type: RecurrenceType): string {
    const recurrenceType = this.recurrenceTypes.find(rt => rt.value === type);
    return recurrenceType ? recurrenceType.label : 'Desconhecido';
  }

  getFormattedDate(date: string | Date): string {
    if (!date) return 'N/A';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString();
  }
}
