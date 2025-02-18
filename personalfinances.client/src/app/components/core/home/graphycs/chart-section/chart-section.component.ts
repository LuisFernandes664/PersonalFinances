import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-chart-section',
  templateUrl: './chart-section.component.html',
  styleUrls: ['./chart-section.component.scss']
})
export class ChartSectionComponent {
  @Input() chartData: any[] = [];
  @Input() selectedInterval: string = 'daily';
  @Output() intervalChanged = new EventEmitter<string>();

  onSelectInterval(interval: string): void {
    this.intervalChanged.emit(interval);
  }
}
