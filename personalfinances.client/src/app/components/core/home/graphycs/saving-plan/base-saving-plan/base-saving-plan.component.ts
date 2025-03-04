// base-saving-plan.component.ts
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SavingPlanModel } from '../abstract-saving-plan.model';
import { SavingPlanService } from '../saving-plan.service';
import { SavingPlanDialogComponent } from '../saving-plan-dialog/saving-plan-dialog.component';

@Component({
  selector: 'app-base-saving-plan',
  templateUrl: './base-saving-plan.component.html',
  styleUrls: ['./base-saving-plan.component.scss']
})
export class BaseSavingPlanComponent implements OnInit {
  savingPlans$: Observable<SavingPlanModel[]>;

  @Input() title: string = 'Planos de Poupança';
  @Input() isBudget: boolean = false;

  constructor(
    private savingPlanService: SavingPlanService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadSavingPlans();
  }

  loadSavingPlans(): void {
    this.savingPlans$ = this.savingPlanService.getSavingPlans().pipe(
      map(plans => plans.filter(plan => plan.tipo === (this.isBudget ? 'budget' : 'goal')))
    );
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(SavingPlanDialogComponent, {
      width: '100vw',
      panelClass: 'custom-dialog-container',
      disableClose: false,
      backdropClass: 'custom-backdrop',
      data: { isBudget: this.isBudget }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.savingPlanService.createSavingPlan(result).subscribe(
          savedPlan => {
            // Atualiza a lista após criar, por exemplo, recarregando os saving plans
            this.loadSavingPlans();
          },
          error => console.error('Erro ao criar saving plan:', error)
        );
      }
    });
  }

  trackByStampEntity(index: number, plan: SavingPlanModel): string {
    return plan.stampEntity;
  }
}
