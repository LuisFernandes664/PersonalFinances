import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TransactionService, ChartSeries, ChartData } from '../transaction.service';
import { Transaction } from '../models/transaction.model';
import { DashboardTotals } from '../models/dashboard-totals.model';
import { APIResponse } from '../../../../models/api-response.model';

@Component({
  selector: 'app-graphyc',
  templateUrl: './graphyc.component.html',
  styleUrls: ['./graphyc.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class GraphycComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  totalBalance: number = 0;
  balanceVariation: number = 0;
  lastMonthBalance: number = 0;
  savings: number = 0;
  savingVariation: number = 0;
  totalExpenses: number = 0;
  totalIncome: number = 0;
  chartData: ChartSeries[] = [];
  chartCategories: string[] = [];
  recentTransactions: Transaction[] = [];
  selectedInterval: string = 'daily';

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.transactionService.getDashboardTotals()
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (totals: DashboardTotals) => {
          this.totalBalance = totals.totalBalance;
          this.balanceVariation = totals.balanceVariation;
          this.lastMonthBalance = totals.lastMonthBalance;
          this.savings = totals.savings;
          this.savingVariation = totals.savingVariation;
          this.totalExpenses = totals.totalExpenses;
          this.totalIncome = totals.totalIncome;

          this.onIntervalChanged('daily');
        },
        error => console.error('Erro ao obter os totais do dashboard:', error)
      );

    this.transactionService.transactions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (transactions: Transaction[]) => {
          this.recentTransactions = transactions
            .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
            .slice(0, 7);
        },
        error => console.error('Erro ao obter as transacções:', error)
      );
  }

  onIntervalChanged(interval: string): void {
    this.selectedInterval = interval;
    this.transactionService.getChartData(interval)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (response: APIResponse<any>) => {

          if (response && response.data) {
            if (Array.isArray(response.data) && response.data.length > 0) {
              const rawSeries = response.data[0];

              this.chartData = [
                {
                  name: 'Lucros',
                  data: rawSeries.data.map((value: number) => (value > 0 ? value : 0))
                },
                {
                  name: 'Perdas',
                  data: rawSeries.data.map((value: number) => (value < 0 ? Math.abs(value) : 0))
                }
              ];
              this.chartCategories = rawSeries.categories || this.generateCategories(interval, rawSeries.data.length);
            }

            else if (response.data.series && response.data.categories) {
              this.chartData = response.data.series;
              this.chartCategories = response.data.categories;
            }
          } else {
            this.chartData = [];
            this.chartCategories = [];
          }
        }
      );
  }

  private generateCategories(interval: string, length: number): string[] {
    const categories: string[] = [];
    const now = new Date();
    for (let i = length - 1; i >= 0; i--) {
      const date = new Date(now);
      if (interval === 'daily') {
        date.setDate(now.getDate() - i);
        categories.push(date.toLocaleDateString('pt-BR', { month: '2-digit', day: '2-digit' }));
      } else if (interval === 'weekly') {
        date.setDate(now.getDate() - i * 7);
        categories.push(`Semana ${this.getWeek(date)}`);
      } else if (interval === 'monthly') {
        date.setMonth(now.getMonth() - i);
        categories.push(date.toLocaleString('pt-BR', { month: '2-digit', year: '2-digit' }));
      }
    }
    return categories;
  }

  private getWeek(date: Date): number {
    const firstDay = new Date(date.getFullYear(), 0, 1);
    const dayOffset = (date.getDay() + 6) % 7;
    const diff = Math.floor((date.getTime() - firstDay.getTime()) / (1000 * 60 * 60 * 24));
    return Math.floor((diff + firstDay.getDay() + 6) / 7) + 1;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
