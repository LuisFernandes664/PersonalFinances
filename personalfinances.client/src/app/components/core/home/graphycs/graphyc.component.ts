import { Component, OnInit } from '@angular/core';
import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexStroke,
  ApexMarkers,
  ApexYAxis,
  ApexGrid
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers;
  yaxis: ApexYAxis;
  grid: ApexGrid;
};

@Component({
  selector: 'app-graphyc',
  templateUrl: './graphyc.component.html',
  styleUrls: ['./graphyc.component.scss']
})
export class GraphycsComponent implements OnInit {
  public chartOptions!: Partial<ChartOptions>;

  constructor() { }

  ngOnInit(): void {
    this.chartOptions = {
      series: [
        {
          name: "Series 1",
          data: [30, 40, 25, 50, 49, 21, 70]
        },
        {
          name: "Series 2",
          data: [20, 30, 45, 30, 29, 55, 35]
        }
      ],
      chart: {
        type: "line",
        height: 300
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: "smooth"
      },
      markers: {
        size: 0
      },
      xaxis: {
        categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
      },
      yaxis: {
        labels: {
          formatter: (val: number) => {
            return "$" + val.toFixed(0);
          }
        }
      },
      grid: {
        borderColor: "#e0e0e0",
        strokeDashArray: 4
      }
    };
  }
}
