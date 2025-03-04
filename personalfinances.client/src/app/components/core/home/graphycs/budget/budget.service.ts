import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../../../public/auth/auth.service';
import { environment } from '../../../../../../environments/environment';
import { BudgetModel } from './budget.model';
import { APIResponse } from '../../../../../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = `${environment.apiUrl}/budgets`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Obtém os orçamentos do utilizador
  getBudgets(): Observable<APIResponse<BudgetModel[]>> {
    return this.http.get<APIResponse<BudgetModel[]>>(`${this.apiUrl}/${this.authService.getDecodedToken().nameid}`);
  }

  createBudget(budget: any): Observable<any> {
    return this.http.post(this.apiUrl, budget);
  }

  updateBudget(budget: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${budget.StampEntity}`, budget);
  }

  deleteBudget(stampEntity: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${stampEntity}`);
  }
}
