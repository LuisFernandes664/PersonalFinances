import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs/operators';
import { Transaction } from '../../models/transaction.model';
import { TransactionService } from '../../transaction.service';
import { ThemeService } from '../../../../../services/theme.service';

export interface TransactionDialogData {
  transaction?: Transaction;
  isEdit: boolean;
}

@Component({
  selector: 'app-transaction-dialog',
  templateUrl: './transaction-dialog.component.html',
  styleUrls: ['./transaction-dialog.component.scss']
})
export class TransactionDialogComponent implements OnInit {
  transactionForm: FormGroup;
  isSubmitting = false;
  formTitle: string;
  isDarkMode: boolean;

  // Categorias predefinidas
  expenseCategories: string[] = [
    'Alimentação', 'Transporte', 'Moradia', 'Saúde',
    'Educação', 'Lazer', 'Compras', 'Assinaturas',
    'Serviços', 'Impostos', 'Outros'
  ];

  incomeCategories: string[] = [
    'Salário', 'Freelance', 'Investimentos', 'Vendas',
    'Presentes', 'Reembolsos', 'Outros'
  ];

  // Opções para formulário
  paymentMethods = [
    { value: 'cash', label: 'Dinheiro' },
    { value: 'credit_card', label: 'Cartão de Crédito' },
    { value: 'debit_card', label: 'Cartão de Débito' },
    { value: 'bank_transfer', label: 'Transferência Bancária' },
    { value: 'other', label: 'Outro' }
  ];

  transactionStatuses = [
    { value: 'pending', label: 'Pendente' },
    { value: 'confirmed', label: 'Confirmada' },
    { value: 'cancelled', label: 'Cancelada' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<TransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TransactionDialogData,
    public transactionService: TransactionService,
    private snackBar: MatSnackBar,
    private themeService: ThemeService
  ) {
    this.formTitle = data.isEdit ? 'Editar Transação' : 'Nova Transação';
    this.isDarkMode = this.themeService.isDarkMode();

    // Inicialização do formulário com validadores
    this.transactionForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(100)]],
      amount: ['', [Validators.required, Validators.pattern(/^-?\d+(\.\d{1,2})?$/)]],
      date: [new Date(), Validators.required],
      category: ['expense', Validators.required],
      subcategory: ['', Validators.required],
      paymentMethod: ['bank_transfer', Validators.required],
      recipient: ['', Validators.maxLength(100)],
      status: ['confirmed', Validators.required],
      referenceId: [''],
      referenceType: [''],
      notes: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    // Aplica a classe dark ao overlay do dialog se estiver no modo escuro
    if (this.isDarkMode) {
      this.dialogRef.addPanelClass('dark-theme');
    }

    // Monitora alterações do tema
    if (this.themeService.isDarkMode()) {
      this.dialogRef.addPanelClass('dark-theme');
    } else {
      this.dialogRef.removePanelClass('dark-theme');
    }

    // Monitora alterações na categoria principal para atualizar subcategorias
    this.transactionForm.get('category')?.valueChanges.subscribe(category => {
      this.updateSubcategoryValidators(category);
    });

    // Configura o modo de edição se necessário
    if (this.data.isEdit && this.data.transaction) {
      this.setupEditMode(this.data.transaction);
    } else {
      // Define data atual para novas transações
      this.transactionForm.get('date')?.setValue(new Date());
      this.updateSubcategoryValidators('expense');
    }
  }

  // Configura o formulário para edição
  setupEditMode(transaction: Transaction): void {
    // Formatação da data para o formato correto
    const transactionDate = new Date(transaction.date);

    // Determinar a categoria com base no sinal do valor
    const isExpense = transaction.amount < 0;
    const category = isExpense ? 'expense' : 'income';

    // Preenche o formulário com os dados existentes
    this.transactionForm.patchValue({
      description: transaction.description,
      amount: Math.abs(transaction.amount), // Remover sinal negativo
      date: transactionDate,
      category: category,
      subcategory: transaction.subcategory || this.getDefaultSubcategory(category),
      paymentMethod: transaction.paymentMethod,
      recipient: transaction.recipient,
      status: transaction.status,
      referenceId: transaction.referenceId || '',
      referenceType: transaction.referenceType || '',
      notes: transaction.notes || ''
    });
  }

  // Atualiza validadores e opções de subcategoria com base na categoria principal
  updateSubcategoryValidators(category: string): void {
    const subcategoryControl = this.transactionForm.get('subcategory');
    if (subcategoryControl) {
      subcategoryControl.setValue(this.getDefaultSubcategory(category));
    }
  }

  // Obtém subcategoria padrão baseada na categoria principal
  getDefaultSubcategory(category: string): string {
    return category === 'income' ? this.incomeCategories[0] : this.expenseCategories[0];
  }

  // Retorna as subcategorias apropriadas com base na categoria principal
  getSubcategories(): string[] {
    const category = this.transactionForm.get('category')?.value;
    return category === 'income' ? this.incomeCategories : this.expenseCategories;
  }

  // Calcula classe CSS dinâmica para o seletor de valor
  getAmountClass(): string {
    const category = this.transactionForm.get('category')?.value;
    return category === 'income' ? 'income-amount' : 'expense-amount';
  }

  // Manipulação ao enviar o formulário
  onSubmit(): void {
    if (this.transactionForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      // Prepara os dados do formulário
      const formData = this.prepareFormData();

      // Usa o serviço apropriado baseado se é edição ou criação
      const request = this.data.isEdit
        ? this.transactionService.updateTransaction(this.data.transaction?.stampEntity || '', formData)
        : this.transactionService.addTransaction(formData);

      request.pipe(
        finalize(() => this.isSubmitting = false)
      ).subscribe({
        next: (result) => {
          const successMessage = this.data.isEdit
            ? 'Transação atualizada com sucesso'
            : 'Transação adicionada com sucesso';

          this.showSnackbar(successMessage, 'success');
          this.dialogRef.close(result);
        },
        error: (error) => {
          console.error('Erro ao processar transação:', error);
          this.showSnackbar('Erro ao processar a transação', 'error');
        }
      });
    } else {
      // Marca todos os campos como tocados para mostrar erros
      this.markFormGroupTouched(this.transactionForm);
    }
  }

  // Prepara os dados do formulário antes de enviar
  private prepareFormData(): any {
    const formValues = { ...this.transactionForm.value };

    // Ajusta o sinal do valor com base na categoria
    if (formValues.category === 'expense') {
      formValues.amount = -Math.abs(formValues.amount);
    } else {
      formValues.amount = Math.abs(formValues.amount);
    }

    // Verifica e configura referências a orçamentos/metas
    if (formValues.referenceId) {
      const isBudget = this.transactionService.budgets.some(
        b => b.id === formValues.referenceId
      );
      formValues.referenceType = isBudget ? 'Budget' : 'Goal';
    } else {
      formValues.referenceType = null;
    }

    return formValues;
  }

  // Marca todos os campos do formulário como tocados para exibir erros
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  // Exibe snackbar com mensagens ao utilizador
  private showSnackbar(message: string, type: 'success' | 'error' | 'warning'): void {
    this.snackBar.open(message, 'Fechar', {
      duration: 4000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: [`${type}-snackbar`]
    });
  }

  // Método para cancelar e fechar o diálogo
  onCancel(): void {
    this.dialogRef.close();
  }

}
