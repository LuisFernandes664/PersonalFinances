import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Transaction } from './transaction.model';
import { environment } from '../../../../environments/environment';
import { APIResponse } from '../../../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = `${environment.apiUrl}/Transactions`;
  private transactionsSubject = new BehaviorSubject<Transaction[]>([]);
  transactions$ = this.transactionsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadTransactions();
  }

  // Carrega as transacções da API e atualiza o BehaviourSubject
  loadTransactions(): void {
    this.http.get<APIResponse<Transaction[]>>(this.apiUrl)
      .pipe(
        map(response => response.data)
      )
      .subscribe(transactions => this.transactionsSubject.next(transactions));
  }

  // Obtém todas as transacções (diretamente da API)
  getTransactions(): Observable<Transaction[]> {
    return this.http.get<APIResponse<Transaction[]>>(this.apiUrl)
      .pipe(
        map(response => response.data)
      );
  }

  // Obtém uma transacção pelo id (ou stamp_entity)
  getTransactionById(id: string): Observable<Transaction> {
    return this.http.get<APIResponse<Transaction>>(`${this.apiUrl}/${id}`)
      .pipe(
        map(response => response.data)
      );
  }

  // Adiciona uma nova transacção
  addTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<APIResponse<Transaction>>(this.apiUrl, transaction)
      .pipe(
        map(response => response.data),
        tap(() => this.loadTransactions())
      );
  }

  // Atualiza uma transacção
  updateTransaction(id: string, transaction: Transaction): Observable<any> {
    return this.http.put<APIResponse<any>>(`${this.apiUrl}/${id}`, transaction)
      .pipe(
        tap(() => this.loadTransactions())
      );
  }

  // Elimina uma transacção
  deleteTransaction(id: string): Observable<any> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`)
      .pipe(
        tap(() => this.loadTransactions())
      );
  }

  // Obtém os totais (balance, income, expenses)
  getTotals(): Observable<{ totalBalance: number, totalIncome: number, totalExpenses: number }> {
    return this.http.get<APIResponse<{ totalBalance: number, totalIncome: number, totalExpenses: number }>>(`${this.apiUrl}/totals`)
      .pipe(
        map(response => response.data)
      );
  }

  getCurrentTransactions(): Transaction[] {
    return this.transactionsSubject.getValue();
  }

}
