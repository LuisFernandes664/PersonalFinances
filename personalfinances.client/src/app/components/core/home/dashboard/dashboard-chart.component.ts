import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ChartComponent,
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexXAxis,
  ApexTooltip,
  ApexTheme,
  ApexGrid,
  ApexStroke
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  theme: ApexTheme;
  grid: ApexGrid;
  stroke: ApexStroke;
  colors: string[];
};

@Component({
  selector: 'app-dashboard-chart',
  template: `
    <div class="chart-container">
      <div *ngIf="loading" class="chart-loading">
        <mat-spinner diameter="40"></mat-spinner>
      </div>
      <apx-chart
        #chart
        [series]="chartOptions.series"
        [chart]="chartOptions.chart"
        [xaxis]="chartOptions.xaxis"
        [yaxis]="chartOptions.yaxis"
        [dataLabels]="chartOptions.dataLabels"
        [plotOptions]="chartOptions.plotOptions"
        [legend]="chartOptions.legend"
        [colors]="chartOptions.colors"
        [tooltip]="chartOptions.tooltip"
        [grid]="chartOptions.grid"
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
export class DashboardChartComponent implements OnInit, OnChanges {
  @ViewChild("chart") chart!: ChartComponent;
  @Input() type: 'bar' | 'line' | 'area' = 'line';
  @Input() series: any[] = [];
  @Input() categories: string[] = [];
  @Input() height: number = 350;
  @Input() loading: boolean = false;

  public chartOptions: Partial<ChartOptions> = {};

  constructor() {}

  ngOnInit(): void {
    this.initChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['series'] || changes['categories']) && this.chartOptions.series) {
      this.updateChartData();
    }

    if (changes['type'] && this.chartOptions.chart) {
      this.chartOptions.chart.type = this.type;
    }

    if (changes['height'] && this.chartOptions.chart) {
      this.chartOptions.chart.height = this.height;
    }
  }

  private initChart(): void {
    this.chartOptions = {
      series: this.formatSeries(),
      chart: {
        type: this.type,
        height: this.height,
        fontFamily: 'Poppins, sans-serif',
        toolbar: {
          show: true,
          tools: {
            download: true,
            selection: true,
            zoom: true,
            zoomin: true,
            zoomout: true,
            pan: true,
            reset: true
          }
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
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: 'smooth',
        width: 3,
      },
      colors: ['#4CAF50', '#F44336', '#2196F3', '#FF9800', '#9C27B0'],
      xaxis: {
        categories: this.categories,
        labels: {
          style: {
            colors: [],
            fontSize: '12px',
            fontFamily: 'Poppins, sans-serif',
            fontWeight: 400,
          }
        },
        axisBorder: {
          show: false
        },
        axisTicks: {
          show: false
        }
      },
      yaxis: {
        labels: {
          formatter: function (val) {
            return val.toFixed(0) + "€";
          },
          style: {
            colors: [],
            fontSize: '12px',
            fontFamily: 'Poppins, sans-serif',
            fontWeight: 400
          }
        }
      },
      legend: {
        position: 'top',
        horizontalAlign: 'right',
        floating: false,
        fontSize: '14px',
        fontFamily: 'Poppins, sans-serif',
        fontWeight: 400,
        offsetY: -8,
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
        marker: {
          show: true,
        },
        x: {
          show: true,
        },
        y: {
          formatter: function (val) {
            return val.toFixed(2) + "€";
          }
        }
      },
      grid: {
        borderColor: '#f1f1f1',
        strokeDashArray: 4,
        xaxis: {
          lines: {
            show: false
          }
        },
        yaxis: {
          lines: {
            show: true
          }
        },
        padding: {
          top: 0,
          right: 0,
          bottom: 0,
          left: 10
        }
      },
      plotOptions: {
        bar: {
          borderRadius: 4,
          columnWidth: '50%',
          dataLabels: {
            position: 'top',
          }
        }
      }
    };
  }

  private formatSeries(): ApexAxisChartSeries {
    return this.series.map(item => ({
      name: item.name,
      data: item.data
    }));
  }

  private updateChartData(): void {
    this.chartOptions.series = this.formatSeries();
    this.chartOptions.xaxis = {
      ...this.chartOptions.xaxis,
      categories: this.categories
    };
  }
}
