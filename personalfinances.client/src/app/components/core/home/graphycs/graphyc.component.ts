import { Component, OnInit } from '@angular/core';
import { Transaction } from '../transaction.model';
import { TransactionService } from '../transaction.service';

@Component({
  selector: 'app-graphyc',
  templateUrl: './graphyc.component.html',
  styleUrls: ['./graphyc.component.scss']
})
export class GraphycComponent implements OnInit {
  transactions: Transaction[] = [];
  totalBalance: number = 0;
  totalIncome: number = 0;
  totalExpenses: number = 0;
  lastMonthBalance: number = 0;
  balanceVariation: number = 0;
  savings: number = 0;
  savingVariation: number = 0;
  chartData: any[] = [];
  recentTransactions: Transaction[] = [];

  constructor(private transactionService: TransactionService) {}

  ngOnInit(): void {
    this.transactionService.transactions$.subscribe(transactions => {
      this.transactions = transactions;
      this.calculateValues();
      this.updateChart();
      this.loadRecentTransactions();
    });
  }

  calculateValues() {
    // Filtrar transações do mês atual
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    const thisMonthTransactions = this.transactions.filter(t => {
      const transactionDate = new Date(t.date);
      return transactionDate.getMonth() === currentMonth && transactionDate.getFullYear() === currentYear;
    });

    // Filtrar transações do mês passado
    const lastMonth = currentMonth === 0 ? 11 : currentMonth - 1;
    const lastMonthYear = currentMonth === 0 ? currentYear - 1 : currentYear;
    const lastMonthTransactions = this.transactions.filter(t => {
      const transactionDate = new Date(t.date);
      return transactionDate.getMonth() === lastMonth && transactionDate.getFullYear() === lastMonthYear;
    });

    // Calcular saldo do mês atual
    this.totalIncome = thisMonthTransactions
      .filter(t => t.category === 'income')
      .reduce((acc, t) => acc + t.amount, 0);

    this.totalExpenses = thisMonthTransactions
      .filter(t => t.category === 'expense')
      .reduce((acc, t) => acc + t.amount, 0);

    this.totalBalance = this.totalIncome - this.totalExpenses;

    // Calcular saldo do mês passado
    const lastMonthIncome = lastMonthTransactions
      .filter(t => t.category === 'income')
      .reduce((acc, t) => acc + t.amount, 0);

    const lastMonthExpenses = lastMonthTransactions
      .filter(t => t.category === 'expense')
      .reduce((acc, t) => acc + t.amount, 0);

    this.lastMonthBalance = lastMonthIncome - lastMonthExpenses;

    // Calcular variação percentual entre os saldos
    if (this.lastMonthBalance !== 0) {
      this.balanceVariation = ((this.totalBalance - this.lastMonthBalance) / Math.abs(this.lastMonthBalance)) * 100;
    } else {
      this.balanceVariation = 0;
    }

    // Calcular savings como uma porcentagem da receita
    this.savings = this.totalIncome * 0.1;
    this.savingVariation = this.savings * 0.04;
  }

  updateChart() {
    this.chartData = [
      { name: 'Income', data: [this.totalIncome] },
      { name: 'Expenses', data: [this.totalExpenses] }
    ];
  }

  loadRecentTransactions() {
    // Pegar nas 4 transações mais recentes
    this.recentTransactions = [...this.transactions]
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 4);
  }
}
