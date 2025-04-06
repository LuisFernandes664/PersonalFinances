import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FinancialHealth } from '../models/financial-health.model';
import { environment } from '../../../../../environments/environment';
import { APIResponse } from '../../../../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class FinancialHealthService {
  private apiUrl = `${environment.apiUrl}/FinancialHealth`;

  constructor(private http: HttpClient) { }

  getFinancialHealth(): Observable<APIResponse<FinancialHealth>> {
    return this.http.get<APIResponse<FinancialHealth>>(this.apiUrl);
  }

  recalculateFinancialHealth(): Observable<APIResponse<FinancialHealth>> {
    return this.http.get<APIResponse<FinancialHealth>>(`${this.apiUrl}/recalculate`);
  }

  getFinancialHealthHistory(months: number = 6): Observable<APIResponse<FinancialHealth[]>> {
    return this.http.get<APIResponse<FinancialHealth[]>>(`${this.apiUrl}/history?months=${months}`);
  }
}
