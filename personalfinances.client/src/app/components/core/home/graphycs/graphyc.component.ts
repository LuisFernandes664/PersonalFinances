import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { Transaction } from '../models/transaction.model';
import { DashboardTotals } from '../models/dashboard-totals.model';
import { ChartService } from './chart-section/chart-series.service';

@Component({
  selector: 'app-graphyc',
  templateUrl: './graphyc.component.html',
  styleUrls: ['./graphyc.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class GraphycComponent implements OnInit {
  totalBalance: number = 0;
  balanceVariation: number = 0;
  lastMonthBalance: number = 0;
  savings: number = 0;
  savingVariation: number = 0;
  totalExpenses: number = 0;
  totalIncome: number = 0;
  chartData: any[] = [];
  recentTransactions: Transaction[] = [];
  selectedInterval: string = 'weekly';

  constructor(private transactionService: TransactionService, private chartService: ChartService) {}

  ngOnInit(): void {
    // Obter os totais do dashboard a partir do backend
    this.transactionService.getDashboardTotals().subscribe((totals: DashboardTotals) => {
      this.totalBalance = totals.totalBalance;
      this.balanceVariation = totals.balanceVariation;
      this.lastMonthBalance = totals.lastMonthBalance;
      this.savings = totals.savings;
      this.savingVariation = totals.savingVariation;
      this.totalExpenses = totals.totalExpenses;
      this.totalIncome = totals.totalIncome;

      // Por exemplo, definir os dados do gráfico com base nos totais (isto é apenas um exemplo)
      this.chartData = [
        { name: 'Income', data: [this.totalIncome], categories: ['Total'] },
        { name: 'Expenses', data: [this.totalExpenses], categories: ['Total'] }
      ];
    });

    // Obter as transacções recentes
    this.transactionService.transactions$.subscribe((transactions: Transaction[]) => {
      this.recentTransactions = transactions
        .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
        .slice(0, 4);
    });
  }

  onIntervalChanged(interval: string): void {
    this.selectedInterval = interval;
    this.chartService.getChartData(interval).subscribe(response => { this.chartData = response.data; });
  }
}
