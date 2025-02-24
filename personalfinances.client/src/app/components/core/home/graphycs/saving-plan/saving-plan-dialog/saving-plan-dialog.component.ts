import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SavingPlan } from '../../../goal/goal.service';

@Component({
  selector: 'app-saving-plan-dialog',
  templateUrl: './saving-plan-dialog.component.html',
  styleUrls: ['./saving-plan-dialog.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class SavingPlanDialogComponent {

  newPlan: SavingPlan = {
    stamp_entity: '',
    UserId: '',
    Descricao: '',
    ValorAlvo: 0,
    ValorAtual: 0,
    DataLimite: new Date(),
    CreatedAt: new Date()
  };

  constructor(
    public dialogRef: MatDialogRef<SavingPlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onCreate(): void {
    // Fecha o diálogo retornando o objeto do novo saving plan
    this.dialogRef.close(this.newPlan);
  }
}
