import { Component, Input, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GoalService, SavingPlan } from '../../goal/goal.service';
import { SavingPlanDialogComponent } from './saving-plan-dialog/saving-plan-dialog.component';

@Component({
  selector: 'app-saving-plan',
  templateUrl: './saving-plan.component.html',
  styleUrls: ['./saving-plan.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SavingPlanComponent {
  @Input() plans: SavingPlan[] = [];

  constructor(
    private savingPlanService: GoalService,
    private dialog: MatDialog
  ) {}

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(SavingPlanDialogComponent, {
      width: '90%',
      maxWidth: '800px',
      maxHeight: '80vh',
      panelClass: 'custom-dialog-container',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Aqui definimos o UserId se necessário; no exemplo, usamos um valor fixo
        result.UserId = 'user-id-exemplo';
        this.savingPlanService.createSavingPlan(result).subscribe(
          (plan) => {
            this.plans.push(plan.data);
          },
          error => console.error('Erro ao criar saving plan:', error)
        );
      }
    });
  }
}
