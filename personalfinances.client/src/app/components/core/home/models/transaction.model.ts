export interface Transaction {
  stampEntity: string;
  description: string;
  amount: number;
  date: Date;
  category?: string;
  paymentMethod?: string;
  recipient?: string;
  status?: 'pending' | 'confirmed' | 'cancelled';
  referenceId?: string;
  referenceType?: string;
  currency?: string;
  originalAmount?: number;
  exchangeRate?: number;
  tags?: string[];
  notes?: string;
  createdAt?: Date | string;
  updatedAt?: Date | string;
}

export const TRANSACTION_CATEGORIES = [
  { id: 'income', name: 'Receita', icon: 'arrow-down', color: '#4caf50' },
  { id: 'salary', name: 'Salário', icon: 'currency-eur', color: '#4caf50', parent: 'income' },
  { id: 'investment', name: 'Investimentos', icon: 'trending-up', color: '#4caf50', parent: 'income' },
  { id: 'freelance', name: 'Freelance', icon: 'briefcase', color: '#4caf50', parent: 'income' },
  { id: 'other_income', name: 'Outras Receitas', icon: 'plus-circle', color: '#4caf50', parent: 'income' },

  { id: 'expense', name: 'Despesa', icon: 'arrow-up', color: '#f44336' },
  { id: 'food', name: 'Alimentação', icon: 'shopping-cart', color: '#f44336', parent: 'expense' },
  { id: 'housing', name: 'Moradia', icon: 'home', color: '#f44336', parent: 'expense' },
  { id: 'transport', name: 'Transporte', icon: 'car', color: '#f44336', parent: 'expense' },
  { id: 'entertainment', name: 'Entretenimento', icon: 'film', color: '#f44336', parent: 'expense' },
  { id: 'health', name: 'Saúde', icon: 'heart', color: '#f44336', parent: 'expense' },
  { id: 'education', name: 'Educação', icon: 'book', color: '#f44336', parent: 'expense' },
  { id: 'utilities', name: 'Utilidades', icon: 'zap', color: '#f44336', parent: 'expense' },
  { id: 'shopping', name: 'Compras', icon: 'shopping-bag', color: '#f44336', parent: 'expense' },
  { id: 'subscriptions', name: 'Assinaturas', icon: 'repeat', color: '#f44336', parent: 'expense' },
  { id: 'other_expense', name: 'Outras Despesas', icon: 'more-horizontal', color: '#f44336', parent: 'expense' }
];

export const PAYMENT_METHODS = [
  { id: 'cash', name: 'Dinheiro', icon: 'cash' },
  { id: 'credit_card', name: 'Cartão de Crédito', icon: 'credit-card' },
  { id: 'debit_card', name: 'Cartão de Débito', icon: 'credit-card' },
  { id: 'bank_transfer', name: 'Transferência Bancária', icon: 'refresh-cw' },
  { id: 'pix', name: 'PIX', icon: 'zap' },
  { id: 'mobile_payment', name: 'Pagamento Móvel', icon: 'smartphone' },
  { id: 'other', name: 'Outros', icon: 'more-horizontal' }
];

export const TRANSACTION_STATUS = [
  { id: 'pending', name: 'Pendente', color: '#ff9800' },
  { id: 'confirmed', name: 'Confirmado', color: '#4caf50' },
  { id: 'cancelled', name: 'Cancelado', color: '#f44336' }
];

export const CURRENCIES = [
  { code: 'EUR', symbol: '€', name: 'Euro' },
  { code: 'USD', symbol: '$', name: 'Dólar Americano' },
  { code: 'GBP', symbol: '£', name: 'Libra Esterlina' },
  { code: 'JPY', symbol: '¥', name: 'Iene Japonês' },
  { code: 'BRL', symbol: 'R$', name: 'Real Brasileiro' }
];
