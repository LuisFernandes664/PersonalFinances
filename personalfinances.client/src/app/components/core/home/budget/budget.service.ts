import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { AuthService } from '../../../public/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = `${environment.apiUrl}/budgets`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Obtém os orçamentos do utilizador
  getBudgets(): Observable<any> {
    return this.http.get(`${this.apiUrl}/${this.authService.getDecodedToken().nameid}`);
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
