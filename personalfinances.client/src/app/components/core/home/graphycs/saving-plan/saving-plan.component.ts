import { Component, Input, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GoalService, SavingPlan } from '../../goal/goal.service';
import { SavingPlanDialogComponent } from './saving-plan-dialog/saving-plan-dialog.component';
import { AuthService } from '../../../../public/auth/auth.service';

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
    private dialog: MatDialog,
    private authService: AuthService
  ) {}

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(SavingPlanDialogComponent, {
      width: '400px',
      panelClass: 'custom-dialog-container',
      disableClose: false,
      backdropClass: 'custom-backdrop',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        result.UserId = this.authService.getDecodedToken().nameid;
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
