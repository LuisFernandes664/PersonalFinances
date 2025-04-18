import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { APIResponse } from '../../../models/api-response.model';
import { Transaction } from './models/transaction.model';
import { DashboardTotals } from './models/dashboard-totals.model';
import { GoalService } from './graphycs/goal/goal.service';
import { BudgetService } from './graphycs/budget/budget.service';

// transaction.service.ts
export interface ChartSeries {
  name: string;
  data: number[];
  categories?: string[];
}

export interface ChartData {
  series: ChartSeries[];
  categories: string[];
}

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private transactionsApiUrl = `${environment.apiUrl}/Transactions`;
  private chartApiUrl = `${environment.apiUrl}/Transactions/chartdata`;
  private transactionsSubject = new BehaviorSubject<Transaction[]>([]);
  transactions$ = this.transactionsSubject.asObservable();

  budgets: any[] = [];
  goals: any[] = [];

  private totalsSubject = new BehaviorSubject<{ totalBalance: number, totalIncome: number, totalExpenses: number }>({
    totalBalance: 0,
    totalIncome: 0,
    totalExpenses: 0
  });
  totals$ = this.totalsSubject.asObservable();


  constructor(private http: HttpClient, protected goalService: GoalService, private budgetService: BudgetService) {
    this.loadTransactions();
    this.goalService.getSavingPlans().subscribe(goals => this.goals = goals.data);
    this.budgetService.getBudgets().subscribe(budgets => this.budgets = budgets.data);
  }

  // Métodos de transacções
  loadTransactions(): void {
    this.http.get<APIResponse<Transaction[]>>(this.transactionsApiUrl)
      .pipe(map(response => response.data))
      .subscribe(transactions => {
        this.transactionsSubject.next(transactions);
        // Atualizar os totais sempre que as transacções são carregadas
        this.http.get<APIResponse<{ totalBalance: number, totalIncome: number, totalExpenses: number }>>(`${this.transactionsApiUrl}/totals`)
          .pipe(map(response => response.data))
          .subscribe(totals => {
            this.totalsSubject.next(totals);
          });
      });
  }

  // SIM
  addTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<APIResponse<Transaction>>(this.transactionsApiUrl, transaction)
      .pipe(
        map(response => response.data),
        tap(() => this.loadTransactions())
      );
  }

  // SIM
  updateTransaction(id: string, transaction: Transaction): Observable<any> {
    return this.http.put<APIResponse<any>>(`${this.transactionsApiUrl}/${id}`, transaction)
      .pipe(tap(() => this.loadTransactions()));
  }

  // SIM
  deleteTransaction(id: string): Observable<any> {
    return this.http.delete<APIResponse<any>>(`${this.transactionsApiUrl}/${id}`)
      .pipe(tap(() => this.loadTransactions()));
  }

  // SIM
  getDashboardTotals(): Observable<DashboardTotals> {
    return this.http.get<APIResponse<DashboardTotals>>(`${this.transactionsApiUrl}/dashboard-totals`)
      .pipe(map(response => response.data));
  }

  // SIM
  getCurrentTransactions(): Transaction[] {
    return this.transactionsSubject.getValue();
  }

  // SIM
  getChartData(interval: string): Observable<APIResponse<{ date: string; value: number }[]>> {
    return this.http.get<APIResponse<{ date: string; value: number }[]>>(`${this.chartApiUrl}?interval=${interval}`);
  }

  // SIM
  refreshTotals(): Observable<{ totalBalance: number, totalIncome: number, totalExpenses: number }> {
    return this.totals$;
  }

  getTransactionByStampEntityAsync(transactionId: string): Observable<Transaction> {
    return this.http.get<APIResponse<Transaction>>(`${this.transactionsApiUrl}/stamp/${transactionId}`)
      .pipe(map(response => response.data));
  }

}
