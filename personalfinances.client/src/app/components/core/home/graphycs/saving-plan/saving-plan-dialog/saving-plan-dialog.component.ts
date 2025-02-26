import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GoalService } from '../../../goal/goal.service';
import { SavingPlan } from '../saving.plan.model';

@Component({
  selector: 'app-saving-plan-dialog',
  templateUrl: './saving-plan-dialog.component.html',
  styleUrls: ['./saving-plan-dialog.component.scss']
})
export class SavingPlanDialogComponent implements OnInit {

  newPlan: SavingPlan = {
    stamp_entity: '',
    UserId: '',
    CategoryId: '',
    Descricao: '',
    ValorAlvo: 0,
    ValorAtual: 0,
    DataLimite: new Date(),
    CreatedAt: new Date()
  };

  categories: { stamp_entity: string, name: string }[] = [];

  constructor(
    public dialogRef: MatDialogRef<SavingPlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private savingPlanService: GoalService
  ) {}

  ngOnInit(): void {
    this.savingPlanService.getCategories().subscribe(categories => {
      this.categories = categories.data;
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onCreate(): void {
    if (this.newPlan.Descricao && this.newPlan.ValorAlvo > 0 && this.newPlan.DataLimite && this.newPlan.CategoryId) {
      this.savingPlanService.createSavingPlan(this.newPlan).subscribe(
        (savedPlan) => {
          this.dialogRef.close(savedPlan.data);
        },
        error => console.error('Erro ao criar Saving Plan:', error)
      );
    }
  }
}
