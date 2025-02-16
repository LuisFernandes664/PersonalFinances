import { Component, ViewEncapsulation } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardComponent {
  transactions: any[] = [];
  showModal: boolean = false;
  editingIndex: number | null = null;

  constructor(private transactionService: TransactionService, private dashboardService: DashboardService) { }

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
    this.transactionService.deleteTransaction(index);
    this.dashboardService.getTransactions().subscribe(data => {
      this.transactions = data;
    });
  }


}
