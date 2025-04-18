import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { Transaction } from '../models/transaction.model';
import { MatDialog } from '@angular/material/dialog';
import { TransactionDialogComponent } from './transaction-dialog/transaction-dialog.component';
import { FormControl } from '@angular/forms';

interface ChartData {
  series: any[];
  categories: string[];
}

interface BudgetItem {
  category: string;
  current: number;
  target: number;
  percentage: number;
}

interface CategoryData {
  name: string;
  value: number;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardComponent implements OnInit {
  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  showModal: boolean = false;
  editingIndex: number | null = null;
  searchControl = new FormControl('');

  // Dados de totais para os cards
  totals = {
    totalIncome: 0,
    totalExpenses: 0,
    totalBalance: 0
  };

  // Dados para os orçamentos
  budgets: BudgetItem[] = [
    { category: 'Alimentação', current: 350, target: 500, percentage: 70 },
    { category: 'Transporte', current: 180, target: 200, percentage: 90 },
    { category: 'Entretenimento', current: 75, target: 150, percentage: 50 }
  ];

  // Filtros de data
  dateRange = {
    start: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
    end: new Date()
  };

  // Controle de visualização dos gráficos
  selectedChartPeriod = 'monthly';
  selectedTransactionType = 'all';

  // Dados para o gráfico de linha/área
  chartData: ChartData = {
    series: [],
    categories: []
  };

  // Dados para o gráfico de pizza
  categoryData: CategoryData[] = [];

  loading = false;

  constructor(
    private transactionService: TransactionService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loading = true;
    this.loadTransactions();
    this.setupSearch();
    this.loadChartData();
    this.loadCategoryData();
  }

  setupSearch() {
    this.searchControl.valueChanges.subscribe(value => {
      this.filterTransactions(value);
    });
  }

  filterTransactions(searchTerm: string | null) {
    if (!searchTerm) {
      this.filteredTransactions = [...this.transactions];
      return;
    }

    const term = searchTerm.toLowerCase();
    this.filteredTransactions = this.transactions.filter(tx =>
      tx.description.toLowerCase().includes(term) ||
      (tx.category && tx.category.toLowerCase().includes(term))
    );
  }

  loadChartData() {
    this.loading = true;

    // Simulação de chamada de API com delay
    setTimeout(() => {
      if (this.selectedChartPeriod === 'monthly') {
        this.chartData = {
          series: [
            {
              name: 'Receitas',
              data: [2350, 2100, 2400, 2550, 2300, 2450]
            },
            {
              name: 'Despesas',
              data: [1800, 1950, 2100, 1700, 1800, 2050]
            }
          ],
          categories: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun']
        };
      } else if (this.selectedChartPeriod === 'weekly') {
        this.chartData = {
          series: [
            {
              name: 'Receitas',
              data: [580, 620, 750, 500]
            },
            {
              name: 'Despesas',
              data: [450, 520, 490, 380]
            }
          ],
          categories: ['Semana 1', 'Semana 2', 'Semana 3', 'Semana 4']
        };
      } else {
        this.chartData = {
          series: [
            {
              name: 'Receitas',
              data: [25000, 27500, 29000, 32000]
            },
            {
              name: 'Despesas',
              data: [22000, 24000, 25500, 27000]
            }
          ],
          categories: ['2022', '2023', '2024', '2025']
        };
      }

      this.loading = false;
    }, 1000);
  }

  loadCategoryData() {
    // Simulação de dados para o gráfico de pizza
    this.categoryData = [
      { name: 'Alimentação', value: 400 },
      { name: 'Moradia', value: 700 },
      { name: 'Transporte', value: 200 },
      { name: 'Entretenimento', value: 150 },
      { name: 'Saúde', value: 180 },
      { name: 'Outros', value: 120 }
    ];
  }

  openModal() {
    this.showModal = true;

    const dialogRef = this.dialog.open(TransactionDialogComponent, {
      width: '600px',
      disableClose: true,
      data: { isEdit: false }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Adicionar categoria e tipo de transação (income/expense) com base no valor
        if (!result.category) {
          result.category = result.amount > 0 ? 'Receita' : 'Gasto Geral';
        }

        console.log('Nova transação:', result);
        this.transactionService.addTransaction(result).subscribe(() => {
          this.loadTransactions();
          // Recarregar dados dos gráficos após adicionar transação
          this.loadChartData();
          this.loadCategoryData();
        });
      }
    });
  }

  loadTransactions() {
    this.transactionService.transactions$.subscribe(data => {
      this.transactions = data.map(tx => ({
        ...tx,
        category: tx.category || (tx.amount > 0 ? 'Receita' : 'Gasto Geral')
      }));
      this.filteredTransactions = [...this.transactions];
    });

    this.transactionService.refreshTotals().subscribe(totals => {
      totals.totalBalance = Math.abs(totals.totalBalance);
      totals.totalExpenses = Math.abs(totals.totalExpenses);
      totals.totalIncome = Math.abs(totals.totalIncome);
      this.totals = totals;
    });
  }

  closeModal() {
    this.showModal = false;
    this.editingIndex = null;
  }

  editTransaction(transaction: Transaction) {
    const dialogRef = this.dialog.open(TransactionDialogComponent, {
      width: '500px',
      disableClose: true,
      data: {
        isEdit: true,
        transaction: transaction
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Manter a categoria se não foi alterada
        if (!result.category) {
          result.category = transaction.category;
        }

        this.transactionService.updateTransaction(transaction.stampEntity, result).subscribe(() => {
          this.loadTransactions();
          // Recarregar dados dos gráficos após editar transação
          this.loadChartData();
          this.loadCategoryData();
        });
      }
    });
  }

  deleteTransaction(index: number) {
    const transaction = this.transactions[index];
    if (transaction && transaction.stampEntity) {
      this.transactionService.deleteTransaction(transaction.stampEntity).subscribe(() => {
        this.loadTransactions();
        // Recarregar dados dos gráficos após excluir transação
        this.loadChartData();
        this.loadCategoryData();
      });
    }
  }

  // Métodos para filtros de gráficos
  setChartPeriod(period: string) {
    this.selectedChartPeriod = period;
    this.loadChartData();
  }

  setTransactionType(type: string) {
    this.selectedTransactionType = type;
    // Recarregar dados baseados no tipo selecionado
    this.loadCategoryData();
  }

  // Métodos para exportação
  exportData(format: string) {
    console.log(`Exportando dados em formato ${format}...`);
    // Aqui você implementaria a chamada ao serviço de exportação
  }

  // Métodos para paginação
  changePage(page: number) {
    console.log(`Mudando para página ${page}...`);
    // Aqui você implementaria a lógica de paginação
  }
}
