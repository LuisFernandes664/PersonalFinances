export interface Transaction {
  stampEntity: string;           // Identificador único (gerado pelo backend)
  description: string;           // Descrição da transacção
  amount: number;                // Valor da transacção
  date: string;                  // Data (formatada como 'yyyy-MM-dd')
  category: 'income' | 'expense';// Categoria: "income" ou "expense"
  paymentMethod: 'cash' | 'credit_card' | 'debit_card' | 'bank_transfer'; // Método de pagamento
  recipient: string;             // Destinatário da transacção
  status: 'pending' | 'confirmed' | 'cancelled'; // Estado da transacção
  created_at?: string;           // Data de criação (opcional, se pretender exibi-la)
  updated_at?: string;           // Data de actualização (opcional)
}
