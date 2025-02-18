import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent {
  @Input() totalBalance: number = 0;
  @Input() balanceVariation: number = 0;
  @Input() lastMonthBalance: number = 0;
  @Input() savings: number = 0;
  @Input() savingVariation: number = 0;
  @Input() totalExpenses: number = 0;
  @Input() totalIncome: number = 0;
}
