export interface FinancialHealth {
  stampEntity: string;
  userId: string;
  overallScore: number;
  savingsScore: number;
  spendingScore: number;
  debtScore: number;
  budgetAdherenceScore: number;
  recommendations: FinancialHealthRecommendation[];
  calculatedAt: Date | string;
}

export interface FinancialHealthRecommendation {
  category: string;
  description: string;
  actionItem: string;
  priorityLevel: number;
}
