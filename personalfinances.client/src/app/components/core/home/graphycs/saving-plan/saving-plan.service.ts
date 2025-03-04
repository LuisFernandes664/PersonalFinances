// saving-plan.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { SavingPlanModel } from './abstract-saving-plan.model';
import { environment } from '../../../../../../environments/environment';
import { BudgetService } from '../budget/budget.service';
import { GoalService } from '../goal/goal.service';
import { mapBudgetToSavingPlan, mapGoalToSavingPlan } from './saving-plan.mapper';

@Injectable({
  providedIn: 'root'
})
export class SavingPlanService {
  private goalApiUrl = `${environment.apiUrl}/goals`;
  private budgetApiUrl = `${environment.apiUrl}/budgets`;

  constructor(
    private http: HttpClient,
    private budgetService: BudgetService,
    private goalService: GoalService
  ) { }

  getSavingPlans(): Observable<SavingPlanModel[]> {
    return forkJoin([
      this.budgetService.getBudgets().pipe(
        map(response => response.data.map(mapBudgetToSavingPlan))
      ),
      this.goalService.getSavingPlans().pipe(
        map(response => response.data.map(mapGoalToSavingPlan))
      )
    ]).pipe(
      map(([budgetPlans, goalPlans]) => [...budgetPlans, ...goalPlans])
    );
  }

  createSavingPlan(plan: SavingPlanModel): Observable<SavingPlanModel> {
    const apiUrl = plan.tipo === 'goal' ? this.goalApiUrl : this.budgetApiUrl;
    return this.http.post<SavingPlanModel>(apiUrl, plan);
  }
}
