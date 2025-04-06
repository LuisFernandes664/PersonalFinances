import { Component, OnInit } from '@angular/core';
import { FinancialHealthService } from '../services/financial-health.service';
import { FinancialHealth } from '../models/financial-health.model';
import { NotificationService } from '../../../shared/notifications/notification.service';

@Component({
  selector: 'app-financial-health',
  templateUrl: './financial-health.component.html',
  styleUrls: ['./financial-health.component.scss']
})
export class FinancialHealthComponent implements OnInit {
  financialHealth: FinancialHealth | null = null;
  historyData: FinancialHealth[] = [];
  isLoading: boolean = false;
  showHistory: boolean = false;

  // Dados para o gráfico
  chartOptions: any;
  chartData: any[] = [];

  // Define o número de meses para exibir no histórico
  historyMonths: number = 6;

  constructor(
    private financialHealthService: FinancialHealthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadFinancialHealth();
  }

  loadFinancialHealth(): void {
    this.isLoading = true;
    this.financialHealthService.getFinancialHealth().subscribe(
      response => {
        this.financialHealth = response.data;
        this.isLoading = false;

        // Após carregar a saúde financeira atual, carregue o histórico para o gráfico
        this.loadHistoryData();
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao carregar dados de saúde financeira. Por favor, tente novamente.');
        console.error('Erro ao carregar saúde financeira:', error);
      }
    );
  }

  loadHistoryData(): void {
    this.isLoading = true;
    this.financialHealthService.getFinancialHealthHistory(this.historyMonths).subscribe(
      response => {
        this.historyData = response.data;
        this.prepareChartData();
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao carregar histórico de saúde financeira. Por favor, tente novamente.');
        console.error('Erro ao carregar histórico de saúde financeira:', error);
      }
    );
  }

  prepareChartData(): void {
    if (!this.historyData || this.historyData.length === 0) {
      return;
    }

    // Ordenar dados por data
    const sortedData = [...this.historyData].sort((a, b) => {
      return new Date(a.calculatedAt).getTime() - new Date(b.calculatedAt).getTime();
    });

    // Preparar dados para o gráfico
    const labels = sortedData.map(item => this.formatDate(item.calculatedAt));

    const overallScores = sortedData.map(item => item.overallScore);
    const savingsScores = sortedData.map(item => item.savingsScore);
    const spendingScores = sortedData.map(item => item.spendingScore);
    const debtScores = sortedData.map(item => item.debtScore);
    const budgetScores = sortedData.map(item => item.budgetAdherenceScore);

    this.chartData = [
      { name: 'Geral', data: overallScores },
      { name: 'Poupança', data: savingsScores },
      { name: 'Despesas', data: spendingScores },
      { name: 'Dívidas', data: debtScores },
      { name: 'Orçamento', data: budgetScores }
    ];

    this.chartOptions = {
      chart: {
        height: 350,
        type: 'line',
        toolbar: {
          show: false
        }
      },
      colors: ['#4caf50', '#2196f3', '#ff9800', '#f44336', '#9c27b0'],
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: 3,
        curve: 'smooth'
      },
      xaxis: {
        categories: labels
      },
      yaxis: {
        min: 0,
        max: 100,
        title: {
          text: 'Pontuação'
        }
      },
      tooltip: {
        y: {
          formatter: (value: number) => `${value}%`
        }
      },
      legend: {
        position: 'top'
      }
    };
  }

  recalculateFinancialHealth(): void {
    this.isLoading = true;
    this.financialHealthService.recalculateFinancialHealth().subscribe(
      response => {
        this.financialHealth = response.data;
        this.notificationService.showToast('success', 'Saúde financeira recalculada com sucesso!');
        this.loadHistoryData(); // Recarregar o histórico para atualizar o gráfico
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao recalcular saúde financeira. Por favor, tente novamente.');
        console.error('Erro ao recalcular saúde financeira:', error);
      }
    );
  }

  toggleHistory(): void {
    this.showHistory = !this.showHistory;
  }

  // Helpers
  getScoreColor(score: number): string {
    if (score >= 80) return '#4caf50'; // Verde
    if (score >= 60) return '#8bc34a'; // Verde claro
    if (score >= 40) return '#ff9800'; // Laranja
    if (score >= 20) return '#ff5722'; // Laranja escuro
    return '#f44336'; // Vermelho
  }

  getScoreLabel(score: number): string {
    if (score >= 80) return 'Excelente';
    if (score >= 60) return 'Bom';
    if (score >= 40) return 'Razoável';
    if (score >= 20) return 'Insuficiente';
    return 'Crítico';
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 1: return 'Alta';
      case 2: return 'Média';
      case 3: return 'Baixa';
      default: return 'Nenhuma';
    }
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 1: return '#f44336'; // Vermelho
      case 2: return '#ff9800'; // Laranja
      case 3: return '#8bc34a'; // Verde claro
      default: return '#9e9e9e'; // Cinza
    }
  }

  formatDate(date: string | Date): string {
    if (!date) return '';
    const dateObj = new Date(date);
    return dateObj.toLocaleDateString('pt-PT', { month: 'short', year: 'numeric' });
  }
}
