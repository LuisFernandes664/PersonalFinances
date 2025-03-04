import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TransactionService } from '../../transaction.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transaction-add',
  templateUrl: './transaction-add.component.html',
  styleUrls: ['./transaction-add.component.scss']
})
export class TransactionAddComponent {
  @Output() close = new EventEmitter<void>();
  @Input() editingIndex: number | null = null;
  transactionForm: FormGroup;
  transactions: Transaction[] = [];

  constructor(private fb: FormBuilder, protected transactionService: TransactionService) {
    this.transactionForm = this.fb.group({
      // Não incluímos o id, pois o identificador é o stampEntity (gerado no backend)
      description: ['', Validators.required],
      amount: ['', Validators.required],
      date: ['', Validators.required],
      category: ['expense', Validators.required],
      paymentMethod: ['cash', Validators.required],
      recipient: [''],
      status: ['pending', Validators.required],
      referenceId: [''],
      referenceType: ['']
    });
  }

  ngOnInit() {

    // Obtem a lista actual de transacções
    this.transactions = this.transactionService.getCurrentTransactions();

    // Se estiver em modo de edição, preenche o formulário com os dados da transacção seleccionada
    if (this.editingIndex !== null && this.editingIndex >= 0 && this.editingIndex < this.transactions.length) {
      const transaction = this.transactions[this.editingIndex];
      if (transaction) {
        this.transactionForm.patchValue({
          description: transaction.description,
          amount: transaction.amount,
          date: transaction.date,
          category: transaction.category,
          paymentMethod: transaction.paymentMethod,
          recipient: transaction.recipient,
          status: transaction.status
        });
      }
    }
  }

  onSubmit() {
    if (this.transactionForm.valid) {

      const transactionData = this.transactionForm.value;
      if (transactionData.referenceId) {
        const isBudget = this.transactionService.budgets.some(b => b.stampEntity === transactionData.referenceId);
        transactionData.referenceType = isBudget ? 'Budget' : 'Goal';
      } else {
        transactionData.referenceType = null;
      }

      const newTransaction = this.transactionForm.value;
      if (this.editingIndex !== null) {
        // Modo de edição: obter o stampEntity da transacção a editar
        const transactionToEdit = this.transactions[this.editingIndex];
        if (transactionToEdit && transactionToEdit.stampEntity) {
          this.transactionService.updateTransaction(transactionToEdit.stampEntity, newTransaction).subscribe(() => {
            this.close.emit();
          });
        }
      } else {
        // Modo de adição
        this.transactionService.addTransaction(newTransaction).subscribe(() => {
          this.close.emit();
        });
      }
      this.transactionForm.reset();
    }
  }

  onCancel() {
    this.close.emit();
  }
}
