import { Component, ViewEncapsulation } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { Transaction } from '../models/transaction.model';
import { MatDialog } from '@angular/material/dialog';
import { TransactionDialogComponent } from './transaction-dialog/transaction-dialog.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardComponent {
  transactions: Transaction[] = [];
  showModal: boolean = false;
  editingIndex: number | null = null;

  totals = {
    totalIncome: 0,
    totalExpenses: 0,
    totalBalance: 0
  };

  constructor(private transactionService: TransactionService, private dialog: MatDialog) { }

  ngOnInit() {
    this.loadTransactions();
  }

  openModal() {
    this.showModal = true;

    const dialogRef = this.dialog.open(TransactionDialogComponent, {
      width: '600px',
      disableClose: true,
      data: { isEdit: false }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Transação adicionada com sucesso
        // result contém a transação criada
        console.log('Nova transação:', result);

        // Atualizar a lista de transações ou fazer outra ação
        this.loadTransactions();
      }
    });

  }

  loadTransactions() {
    this.transactionService.transactions$.subscribe(data => {
      this.transactions = data;
    });

    this.transactionService.refreshTotals().subscribe(totals => {
      totals.totalBalance = Math.abs(totals.totalBalance);
      totals.totalExpenses = Math.abs(totals.totalExpenses);
      totals.totalIncome = Math.abs(totals.totalIncome);
      this.totals = totals;

    });
  }

  closeModal() {
    this.showModal = false;
    this.editingIndex = null;
  }

  editTransaction(transaction: Transaction) {
    const dialogRef = this.dialog.open(TransactionDialogComponent, {
      width: '500px',
      disableClose: true,
      data: {
        isEdit: true,
        transaction: transaction
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Transação atualizada com sucesso
        this.loadTransactions();
      }
    });
  }

  deleteTransaction(index: number) {
    const transaction = this.transactions[index];
    if (transaction && transaction.stampEntity) {
      this.transactionService.deleteTransaction(transaction.stampEntity).subscribe(() => {
      });
    }
  }

}
