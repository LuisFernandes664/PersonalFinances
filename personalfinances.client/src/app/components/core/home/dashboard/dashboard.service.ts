import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Transaction } from '../transaction.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private transactions = new BehaviorSubject<Transaction[]>([]);
  transactions$ = this.transactions.asObservable();

  constructor() {}

  setTransactions(transactions: Transaction[]) {
    this.transactions.next(transactions);
  }

  getTransactions() {
    return this.transactions$;
  }

  getTotalBalance(): number {
    return this.transactions.getValue().reduce((acc, transaction) => acc + transaction.amount, 0);
  }



  getTotalIncome(): number {
    return this.transactions.getValue()
      .filter(transaction => transaction.category === 'income')
      .reduce((acc, transaction) => acc + transaction.amount, 0);
  }

  getTotalExpenses(): number {
    return this.transactions.getValue()
      .filter(transaction => transaction.category === 'expense')
      .reduce((acc, transaction) => acc + transaction.amount, 0);
  }
}
