import { GoalModel } from '../goal/goal.model';
import { BudgetModel } from '../budget/budget.model';
import { SavingPlanModel } from './abstract-saving-plan.model';

export function mapGoalToSavingPlan(goal: GoalModel): SavingPlanModel {
  return {
    stampEntity: goal.stampEntity,
    userId: goal.userId,
    categoryId: goal.categoryId,
    descricao: goal.descricao,
    valorAtual: 0,
    valorAlvo: goal.valorAlvo,
    dataLimite: new Date(goal.dataLimite),
    tipo: 'goal'
  };
}

export function mapBudgetToSavingPlan(budget: BudgetModel): SavingPlanModel {
  return {
    stampEntity: budget.stampEntity,
    userId: budget.userId,
    categoryId: budget.categoryId,
    descricao: 'Orçamento',
    valorAtual: 0,
    valorAlvo: budget.valorOrcado,
    dataLimite: new Date(budget.dataFim),
    tipo: 'budget'
  };
}
