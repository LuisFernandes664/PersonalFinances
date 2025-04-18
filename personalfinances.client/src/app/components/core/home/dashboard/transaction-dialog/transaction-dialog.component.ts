import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
      notes: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    // Aplica a classe dark ao overlay do dialog se estiver no modo escuro
    if (this.isDarkMode) {
      // Remove bordas coloridas no modo escuro e aplica estilos adequados
      this.dialogRef.addPanelClass('dark-theme-dialog');
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
      // subcategory: transaction.subcategory || this.getDefaultSubcategory(category),
      paymentMethod: transaction.paymentMethod,
      recipient: transaction.recipient,
      status: transaction.status,
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

      // Fecha o diálogo e retorna os dados
      this.dialogRef.close(formData);
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

  // Método para cancelar e fechar o diálogo
  onCancel(): void {
    this.dialogRef.close();
  }
}
