import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-saving-plan',
  templateUrl: './saving-plan.component.html',
  styleUrls: ['./saving-plan.component.scss']
})
export class SavingPlanComponent {
  @Input() savings: number = 0;
}
