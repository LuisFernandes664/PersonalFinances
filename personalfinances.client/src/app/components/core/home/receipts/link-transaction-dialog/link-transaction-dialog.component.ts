import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TransactionService } from '../../transaction.service';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Transaction } from '../../models/transaction.model';
import { Receipt } from '../../models/receipt.model';

@Component({
  selector: 'app-link-transaction-dialog',
  templateUrl: './link-transaction-dialog.component.html',
  styleUrls: ['./link-transaction-dialog.component.scss']
})
export class LinkTransactionDialogComponent implements OnInit {
  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  isLoading: boolean = false;
  selectedTransaction: Transaction | null = null;
  searchControl = new FormControl('');

  constructor(
    public dialogRef: MatDialogRef<LinkTransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { receipt: Receipt },
    private transactionService: TransactionService
  ) {}

  ngOnInit(): void {
    this.loadTransactions();

    // Configurar filtro de pesquisa
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(searchTerm => {
        this.filterTransactions(searchTerm);
      });
  }

  loadTransactions(): void {
    this.isLoading = true;
    this.transactions = this.transactionService.getCurrentTransactions()
    this.filterTransactions('');
    this.isLoading = false;
  }

  filterTransactions(searchTerm: string): void {
    if (!searchTerm) {
      // Sem termo de busca, mostramos as transações com valor próximo ao do recibo
      // e preferencialmente da mesma data
      this.filteredTransactions = this.transactions
        .filter(transaction =>
          // Apenas despesas (valores negativos)
          transaction.amount < 0 &&
          // Valor próximo ao do recibo (com margem de 10%)
          Math.abs(Math.abs(transaction.amount) - this.data.receipt.totalAmount) <= this.data.receipt.totalAmount * 0.1
        )
        .sort((a, b) => {
          // Ordenar por proximidade de data
          const receiptDate = new Date(this.data.receipt.receiptDate).getTime();
          const dateA = new Date(a.date).getTime();
          const dateB = new Date(b.date).getTime();

          return Math.abs(dateA - receiptDate) - Math.abs(dateB - receiptDate);
        });
    } else {
      // Com termo de busca, filtramos por descrição/recebedor
      const term = searchTerm.toLowerCase();
      this.filteredTransactions = this.transactions
        .filter(transaction =>
          (transaction.description.toLowerCase().includes(term) ||
           transaction.recipient.toLowerCase().includes(term))
        );
    }
  }

  selectTransaction(transaction: Transaction): void {
    this.selectedTransaction = transaction;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onLink(): void {
    if (this.selectedTransaction) {
      this.dialogRef.close({ transactionId: this.selectedTransaction.stampEntity });
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-PT');
  }

  formatAmount(amount: number): string {
    return Math.abs(amount).toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
  }
}
