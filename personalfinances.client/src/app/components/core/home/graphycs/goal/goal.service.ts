import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { APIResponse } from '../../../../../models/api-response.model';
import { SavingPlan } from '../saving-plan/saving.plan.model';
import { SelectListItem } from '../../../../../models/select-list-item.model';
import { GoalModel } from './goal.model';


@Injectable({
  providedIn: 'root'
})
export class GoalService {
  private apiUrl = `${environment.apiUrl}/goals`;

  constructor(private http: HttpClient) {}

  getSavingPlans(): Observable<APIResponse<GoalModel[]>> {
    return this.http.get<APIResponse<GoalModel[]>>(`${this.apiUrl}`);
  }

  getCategories(): Observable<APIResponse<SelectListItem[]>> {
    return this.http.get<APIResponse<SelectListItem[]>>(`${this.apiUrl}/categories`);
  }

  createSavingPlan(plan: GoalModel): Observable<APIResponse<GoalModel>> {
    return this.http.post<APIResponse<GoalModel>>(this.apiUrl, plan);
  }

  updateSavingPlan(plan: SavingPlan): Observable<APIResponse<SavingPlan>> {
    return this.http.put<APIResponse<SavingPlan>>(`${this.apiUrl}/${plan.stamp_entity}`, plan);
  }

  deleteSavingPlan(stampEntity: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${stampEntity}`);
  }

}
