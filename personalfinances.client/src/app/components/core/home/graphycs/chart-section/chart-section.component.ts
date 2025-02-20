import { Component, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { ChartSeries } from '../../models/chart-series.model';

@Component({
  selector: 'app-chart-section',
  templateUrl: './chart-section.component.html',
  styleUrls: ['./chart-section.component.scss']
})
export class ChartSectionComponent implements OnDestroy {
  @Input() chartData: ChartSeries[] = [];
  @Input() categories: string[] = [];
  @Input() selectedInterval: string = 'daily';
  @Output() intervalChanged = new EventEmitter<string>();

  @ViewChild('chart') chart: any;

  onSelectInterval(interval: string): void {
    this.intervalChanged.emit(interval);
  }

  ngOnDestroy(): void {
    console.log('ChartSectionComponent destruído');
    if (this.chart) {
      this.chart.destroy();
    }
  }
}
