import { Component, ViewEncapsulation } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { Transaction } from '../models/transaction.model';

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

  constructor(private transactionService: TransactionService) { }

  ngOnInit() {
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

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.editingIndex = null;
  }

  editTransaction(index: number) {
    this.editingIndex = index;
    this.showModal = true;
  }

  deleteTransaction(index: number) {
    const transaction = this.transactions[index];
    if (transaction && transaction.stampEntity) {
      this.transactionService.deleteTransaction(transaction.stampEntity).subscribe(() => {
      });
    }
  }

}
