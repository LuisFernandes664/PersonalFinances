import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TransactionService } from '../transaction.service';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  constructor(private transactionService: TransactionService) { }

  getTotals(): Observable<{ totalBalance: number, totalIncome: number, totalExpenses: number }> {
    return this.transactionService.getTotals();
  }
}
