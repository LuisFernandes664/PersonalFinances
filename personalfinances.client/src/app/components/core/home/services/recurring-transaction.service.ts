import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RecurringTransaction } from '../models/recurring-transaction.model';
import { APIResponse } from '../../../../models/api-response.model';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RecurringTransactionService {
  private apiUrl = `${environment.apiUrl}/RecurringTransactions`;

  constructor(private http: HttpClient) { }

  getRecurringTransactions(): Observable<APIResponse<RecurringTransaction[]>> {
    return this.http.get<APIResponse<RecurringTransaction[]>>(this.apiUrl);
  }

  getRecurringTransaction(id: string): Observable<APIResponse<RecurringTransaction>> {
    return this.http.get<APIResponse<RecurringTransaction>>(`${this.apiUrl}/${id}`);
  }

  createRecurringTransaction(transaction: RecurringTransaction): Observable<APIResponse<RecurringTransaction>> {
    return this.http.post<APIResponse<RecurringTransaction>>(this.apiUrl, transaction);
  }

  updateRecurringTransaction(id: string, transaction: RecurringTransaction): Observable<APIResponse<RecurringTransaction>> {
    return this.http.put<APIResponse<RecurringTransaction>>(`${this.apiUrl}/${id}`, transaction);
  }

  deleteRecurringTransaction(id: string): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
