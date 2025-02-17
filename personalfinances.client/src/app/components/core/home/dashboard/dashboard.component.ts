import { Component, ViewEncapsulation } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { DashboardService } from './dashboard.service';
import { Transaction } from '../transaction.model';

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

  constructor(
    private transactionService: TransactionService,
    private dashboardService: DashboardService
  ) { }

  ngOnInit() {
    this.transactionService.transactions$.subscribe(data => {
      this.transactions = data;
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
        // Após eliminação, a lista será actualizada pelo loadTransactions() no serviço
      });
    }
  }

}
