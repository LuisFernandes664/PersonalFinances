export interface Transaction {
  id: number;
  description: string;
  amount: number;
  date: string; // Formatado como 'yyyy-MM-dd'
  category: 'income' | 'expense'; // Tipo da transação
  paymentMethod: 'cash' | 'credit_card' | 'debit_card' | 'bank_transfer'; // Método de pagamento
  recipient: string; // Para quem foi o pagamento
  status: 'pending' | 'confirmed' | 'cancelled'; // Status da transação
}
