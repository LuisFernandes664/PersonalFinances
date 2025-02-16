import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Transaction } from './transaction.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private transactions = new BehaviorSubject<Transaction[]>([]);
  transactions$ = this.transactions.asObservable();

  constructor() {}

  addTransaction(transaction: Transaction) {
    const currentTransactions = this.transactions.getValue();
    transaction.id = currentTransactions.length + 1; // Criando um ID único

    // Garantir que os valores sejam sempre positivos ou negativos corretamente
    if (transaction.category === 'expense') {
      transaction.amount = -Math.abs(transaction.amount);
    } else {
      transaction.amount = Math.abs(transaction.amount);
    }

    this.transactions.next([...currentTransactions, transaction]);
  }


  updateTransaction(index: number, updatedTransaction: Transaction) {
    const currentTransactions = this.transactions.getValue();

    if (index >= 0 && index < currentTransactions.length) {
      currentTransactions[index] = { ...updatedTransaction, id: currentTransactions[index].id };
      console.log(currentTransactions[index])
      console.log([...currentTransactions]);
      this.transactions.next([...currentTransactions]);
    }
  }


  deleteTransaction(id: number) {
    const filteredTransactions = this.transactions.getValue().filter(transaction => transaction.id !== id);
    this.transactions.next(filteredTransactions);
  }

  getTransactions() {
    return this.transactions.getValue();
  }
}
