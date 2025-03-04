import { BudgetModel } from "../budget/budget.model";
import { GoalModel } from "../goal/goal.model";

export interface SavingPlanModel {
  userId: string;
  stampEntity: string;
  descricao: string;
  categoryId?: string;
  valorAtual: number;
  valorAlvo: number;
  dataLimite?: Date;
  tipo: 'goal' | 'budget'; // Indica se é uma meta ou um orçamento
}

export function mapToSavingPlan(plan: GoalModel | BudgetModel): SavingPlanModel {
  if ('valorAlvo' in plan) {
    return {
      stampEntity: plan.stampEntity,
      userId: plan.userId,
      categoryId: plan.categoryId,
      descricao: plan.descricao,
      valorAtual: 0, // ou valor acumulado se existir
      valorAlvo: plan.valorAlvo,
      tipo: 'goal'
    };
  } else {
    // Para o orçamento, mapear os valores corretos
    return {
      stampEntity: plan.stampEntity,
      userId: plan.userId,
      categoryId: plan.categoryId,
      descricao: '', // caso tenha uma descrição ou definir um valor padrão
      valorAtual: 0,  // valor utilizado no orçamento
      valorAlvo: plan.valorOrcado,
      tipo: 'budget'
    };
  }
}

