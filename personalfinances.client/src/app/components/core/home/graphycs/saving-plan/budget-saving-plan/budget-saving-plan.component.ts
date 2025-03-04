import { Component } from '@angular/core';

@Component({
  selector: 'app-budget-saving-plan',
  template: `<app-base-saving-plan [title]="'Orçamentos'" [isBudget]="true"></app-base-saving-plan>`
})
export class BudgetSavingPlanComponent {}
