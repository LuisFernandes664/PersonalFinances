export enum RecurrenceType {
  Daily = 0,
  Weekly = 1,
  Monthly = 2,
  Yearly = 3
}

export interface RecurringTransaction {
  stampEntity: string;
  userId: string;
  description: string;
  amount: number;
  category: 'income' | 'expense';
  paymentMethod: string;
  recipient: string;
  recurrenceType: RecurrenceType;
  recurrenceInterval: number;
  startDate: Date | string;
  endDate?: Date | string;
  isActive: boolean;
  lastProcessedDate?: Date | string;
}
