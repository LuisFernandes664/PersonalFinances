import { Component, OnInit } from '@angular/core';
import { BudgetService } from './budget.service';
import { AuthService } from '../../../../public/auth/auth.service';

@Component({
  selector: 'app-budget',
  templateUrl: './budget.component.html',
  styleUrls: ['./budget.component.scss']
})
export class BudgetComponent implements OnInit {
  budgets: any[] = [];
  newBudget: any = {
    Categoria: '',
    ValorOrcado: 0,
    DataInicio: '',
    DataFim: ''
  };

  constructor(private budgetService: BudgetService, private authService: AuthService) { }

  ngOnInit(): void {
    this.loadBudgets();
  }

  loadBudgets() {
    this.budgetService.getBudgets().subscribe((res) => {
      // Considerando que a resposta segue o padrão APIResponse com propriedade "data"
      this.budgets = res.data;
    });
  }

  createBudget() {
    this.newBudget.UserId = this.authService.getDecodedToken().nameid;
    // Gere um ID único ou deixe o back-end fazer isso
    this.newBudget.StampEntity = this.generateUniqueId();
    this.budgetService.createBudget(this.newBudget).subscribe((res) => {
      this.loadBudgets();
      this.newBudget = { Categoria: '', ValorOrcado: 0, DataInicio: '', DataFim: '' };
    });
  }

  generateUniqueId(): string {
    return Math.random().toString(36).substring(2, 15);
  }
}
