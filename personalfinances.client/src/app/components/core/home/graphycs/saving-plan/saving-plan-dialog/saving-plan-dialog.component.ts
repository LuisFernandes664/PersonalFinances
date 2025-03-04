import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BudgetService } from '../../budget/budget.service';
import { GoalService } from '../../goal/goal.service';
import { BudgetModel } from '../../budget/budget.model';
import { GoalModel } from '../../goal/goal.model';
import { SelectListItem } from '../../../../../../models/select-list-item.model';
import { SavingPlanService } from '../saving-plan.service';
import { SavingPlanModel } from '../abstract-saving-plan.model';

@Component({
  selector: 'app-saving-plan-dialog',
  templateUrl: './saving-plan-dialog.component.html',
  styleUrls: ['./saving-plan-dialog.component.scss']
})
export class SavingPlanDialogComponent implements OnInit {

  newPlan: Partial<SavingPlanModel> = {};
  categories: SelectListItem[] = [];
  formSubmitted: boolean = false;
  isBudget: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<SavingPlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private savingPlanService: SavingPlanService,
    private goalService: GoalService,
  ) {}


  ngOnInit(): void {
    this.isBudget = this.data.isBudget; // Define se é orçamento ou meta
    this.goalService.getCategories().subscribe(categories => {
      this.categories = categories.data;
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onCreate(): void {
    this.formSubmitted = true;
    if (!this.newPlan.descricao || !this.newPlan.valorAlvo || !this.newPlan.categoryId) {
      return;
    }

    this.savingPlanService.createSavingPlan(this.newPlan as SavingPlanModel).subscribe(
      (savedPlan) => this.dialogRef.close(savedPlan),
      (error) => console.error('Erro ao criar Saving Plan:', error)
    );
  }

}
