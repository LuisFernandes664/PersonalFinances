import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { APIResponse } from '../../../../models/api-response.model';


export interface SavingPlan {
  stamp_entity: string;
  UserId: string;
  Descricao: string;
  ValorAlvo: number;
  ValorAtual: number;
  DataLimite: Date;
  CreatedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class GoalService {
  private apiUrl = `${environment.apiUrl}/goals`;

  constructor(private http: HttpClient) {}

  getSavingPlans(userId: string): Observable<APIResponse<SavingPlan[]>> {
    return this.http.get<APIResponse<SavingPlan[]>>(`${this.apiUrl}/${userId}`);
  }

  createSavingPlan(plan: SavingPlan): Observable<APIResponse<SavingPlan>> {
    return this.http.post<APIResponse<SavingPlan>>(this.apiUrl, plan);
  }

  updateSavingPlan(plan: SavingPlan): Observable<APIResponse<SavingPlan>> {
    return this.http.put<APIResponse<SavingPlan>>(`${this.apiUrl}/${plan.stamp_entity}`, plan);
  }

  deleteSavingPlan(stampEntity: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${stampEntity}`);
  }

  getCategories(): Observable<APIResponse<{ stamp_entity: string, name: string }[]>> {
    return this.http.get<APIResponse<{ stamp_entity: string, name: string }[]>>(`${this.apiUrl}/categories`);
  }

}
