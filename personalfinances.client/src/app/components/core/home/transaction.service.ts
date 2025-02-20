import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { APIResponse } from '../../../models/api-response.model';
import { Transaction } from './models/transaction.model';
import { DashboardTotals } from './models/dashboard-totals.model';

// transaction.service.ts
export interface ChartSeries {
  name: string;
  data: number[];
  categories?: string[]; // Opcional, já que parece vir na série
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

  constructor(private http: HttpClient) {
    this.loadTransactions();
  }

  // Métodos de transacções
  loadTransactions(): void {
    this.http.get<APIResponse<Transaction[]>>(this.transactionsApiUrl)
      .pipe(map(response => response.data))
      .subscribe(transactions => this.transactionsSubject.next(transactions));
  }

  getTransactions(): Observable<Transaction[]> {
    return this.http.get<APIResponse<Transaction[]>>(this.transactionsApiUrl)
      .pipe(map(response => response.data));
  }

  getTransactionById(id: string): Observable<Transaction> {
    return this.http.get<APIResponse<Transaction>>(`${this.transactionsApiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  addTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<APIResponse<Transaction>>(this.transactionsApiUrl, transaction)
      .pipe(
        map(response => response.data),
        tap(() => this.loadTransactions())
      );
  }

  updateTransaction(id: string, transaction: Transaction): Observable<any> {
    return this.http.put<APIResponse<any>>(`${this.transactionsApiUrl}/${id}`, transaction)
      .pipe(tap(() => this.loadTransactions()));
  }

  deleteTransaction(id: string): Observable<any> {
    return this.http.delete<APIResponse<any>>(`${this.transactionsApiUrl}/${id}`)
      .pipe(tap(() => this.loadTransactions()));
  }

  getTotals(): Observable<{ totalBalance: number, totalIncome: number, totalExpenses: number }> {
    return this.http.get<APIResponse<{ totalBalance: number, totalIncome: number, totalExpenses: number }>>(`${this.transactionsApiUrl}/totals`)
      .pipe(map(response => response.data));
  }

  getDashboardTotals(): Observable<DashboardTotals> {
    return this.http.get<APIResponse<DashboardTotals>>(`${this.transactionsApiUrl}/dashboard-totals`)
      .pipe(map(response => response.data));
  }

  getCurrentTransactions(): Transaction[] {
    return this.transactionsSubject.getValue();
  }

  // Métodos para dados do gráfico
  // getChartData(interval: string): Observable<APIResponse<{ date: string; value: number }[]>> {
  //   return this.http.get<APIResponse<{ date: string; value: number }[]>>(`${this.chartApiUrl}?interval=${interval}`);
  // }

  getChartData(interval: string): Observable<APIResponse<ChartData>> {
    return this.http.get<APIResponse<ChartData>>(`${this.transactionsApiUrl}/chartdata?interval=${interval}`);
  }
}
