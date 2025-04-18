import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import {
  ApexChart,
  ChartComponent,
  ApexDataLabels,
  ApexLegend,
  ApexTheme,
  ApexTooltip,
  ApexNonAxisChartSeries,
  ApexResponsive,
  ApexStroke
} from "ng-apexcharts";

export type PieChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  theme: ApexTheme;
  colors: string[];
  responsive: ApexResponsive[];
  labels: string[];
  stroke: ApexStroke;
};

@Component({
  selector: 'app-dashboard-pie-chart',
  template: `
    <div class="chart-container">
      <div *ngIf="loading" class="chart-loading">
        <mat-spinner diameter="40"></mat-spinner>
      </div>
      <apx-chart
        #chart
        [series]="chartOptions.series"
        [chart]="chartOptions.chart"
        [labels]="chartOptions.labels"
        [dataLabels]="chartOptions.dataLabels"
        [legend]="chartOptions.legend"
        [colors]="chartOptions.colors"
        [tooltip]="chartOptions.tooltip"
        [responsive]="chartOptions.responsive"
        [stroke]="chartOptions.stroke"
      ></apx-chart>
    </div>
  `,
  styles: [`
    .chart-container {
      position: relative;
      min-height: 300px;
    }

    .chart-loading {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      display: flex;
      justify-content: center;
      align-items: center;
      background-color: rgba(255, 255, 255, 0.7);
      z-index: 10;
    }
  `]
})
export class DashboardPieChartComponent implements OnInit, OnChanges {
  @ViewChild("chart") chart!: ChartComponent;
  @Input() data: {name: string, value: number}[] = [];
  @Input() height: number = 350;
  @Input() donut: boolean = true;
  @Input() loading: boolean = false;

  public chartOptions: Partial<PieChartOptions> = {};

  constructor() {}

  ngOnInit(): void {
    this.initChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data'] && this.chartOptions.series) {
      this.updateChartData();
    }

    if (changes['donut'] && this.chartOptions.chart) {
      this.chartOptions.chart.type = this.donut ? 'donut' : 'pie';
    }

    if (changes['height'] && this.chartOptions.chart) {
      this.chartOptions.chart.height = this.height;
    }
  }

  private initChart(): void {
    const series = this.data.map(item => item.value);
    const labels = this.data.map(item => item.name);

    this.chartOptions = {
      series: series,
      chart: {
        type: 'pie',
        height: this.height,
        fontFamily: 'Poppins, sans-serif',
        toolbar: {
          show: false
        },
        animations: {
          enabled: true,
          // easing: 'easeinout',
          speed: 800,
          animateGradually: {
            enabled: true,
            delay: 150
          },
          dynamicAnimation: {
            enabled: true,
            speed: 350
          }
        }
      },
      labels: labels,
      dataLabels: {
        enabled: true,
        formatter: function(val, opts) {
          return `${opts.w.globals.labels[opts.seriesIndex]}: ${Number(val).toFixed(1)}%`;
        },
        style: {
          fontSize: '14px',
          fontFamily: 'Poppins, sans-serif',
          fontWeight: 500
        },
        dropShadow: {
          enabled: false
        }
      },
      stroke: {
        width: 2,
        colors: ['rgba(255, 255, 255, 0.8)']
      },
      colors: ['#4CAF50', '#F44336', '#2196F3', '#FF9800', '#9C27B0', '#00BCD4', '#607D8B', '#E91E63'],
      legend: {
        position: 'bottom',
        horizontalAlign: 'center',
        fontSize: '14px',
        fontFamily: 'Poppins, sans-serif',
        fontWeight: 400,
        offsetY: 8,
        markers: {
          // width: 12,
          // height: 12,
          strokeWidth: 0,
          // radius: 12,
          offsetX: -3
        }
      },
      tooltip: {
        enabled: true,
        theme: 'light',
        y: {
          formatter: function (val) {
            return val.toFixed(2) + "€";
          }
        }
      },
      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              height: 300
            },
            legend: {
              position: 'bottom'
            }
          }
        }
      ]
    };
  }

  private updateChartData(): void {
    this.chartOptions.series = this.data.map(item => item.value);
    this.chartOptions.labels = this.data.map(item => item.name);
  }
}
