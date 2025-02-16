import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TransactionService } from '../../transaction.service';

@Component({
  selector: 'app-transaction-add',
  templateUrl: './transaction-add.component.html',
  styleUrls: ['./transaction-add.component.scss']
})
export class TransactionAddComponent {
  @Output() close = new EventEmitter<void>();
  @Input() editingIndex: number | null = null;
  transactionForm: FormGroup;

  constructor(private fb: FormBuilder, private transactionService: TransactionService) {
    this.transactionForm = this.fb.group({
      id: [''],
      description: ['', Validators.required],
      amount: ['', Validators.required],
      date: ['', Validators.required],
      category: ['expense', Validators.required],
      paymentMethod: ['cash', Validators.required],
      recipient: [''],
      status: ['pending', Validators.required]
    });
  }

  ngOnInit() {
    if (this.editingIndex !== null) {
      const transactions = this.transactionService.getTransactions();

      if (this.editingIndex >= 0 && this.editingIndex < transactions.length) {
        const transaction = transactions[this.editingIndex];

        if (transaction) {
          this.transactionForm.setValue({ ...transaction });
        }
      }
    }
  }

  onSubmit() {
    if (this.transactionForm.valid) {
      const newTransaction = this.transactionForm.value;

      if (this.editingIndex !== null) {
        this.transactionService.updateTransaction(this.editingIndex, newTransaction);
      } else {
        this.transactionService.addTransaction(newTransaction);
      }
      this.transactionForm.reset();
      this.close.emit();
    }
  }

  onCancel() {
    this.close.emit();
  }
}
