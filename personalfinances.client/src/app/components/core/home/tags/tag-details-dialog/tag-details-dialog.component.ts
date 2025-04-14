import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Transaction } from '../../models/transaction.model';
import { TagService } from '../tag.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { Tag } from '../tag.model';

@Component({
  selector: 'app-tag-details-dialog',
  templateUrl: './tag-details-dialog.component.html',
  styleUrls: ['./tag-details-dialog.component.scss']
})
export class TagDetailsDialogComponent implements OnInit {
  tagForm: FormGroup;
  taggedTransactions: Transaction[] = [];
  isLoading: boolean = false;
  isEditMode: boolean = false;

  // Cores predefinidas para seleção no formulário
  predefinedColors: string[] = [
    '#f44336', '#e91e63', '#9c27b0', '#673ab7',
    '#3f51b5', '#2196f3', '#03a9f4', '#00bcd4',
    '#009688', '#4caf50', '#8bc34a', '#cddc39',
    '#ffeb3b', '#ffc107', '#ff9800', '#ff5722'
  ];

  constructor(
    private fb: FormBuilder,
    private tagService: TagService,
    private notificationService: NotificationService,
    public dialogRef: MatDialogRef<TagDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { tag: Tag }
  ) {
    this.tagForm = this.fb.group({
      name: [data.tag.name, [Validators.required, Validators.maxLength(30)]],
      color: [data.tag.color, Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadTaggedTransactions();
  }

  loadTaggedTransactions(): void {
    this.isLoading = true;
    this.tagService.getTransactionsByTag(this.data.tag.stampEntity).subscribe(
      response => {
        this.taggedTransactions = response.data;
        this.isLoading = false;
      },
      error => {
        this.notificationService.showToast('error', 'Erro ao carregar transações. Por favor, tente novamente.');
        this.isLoading = false;
        console.error('Erro ao carregar transações da tag:', error);
      }
    );
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode) {
      // Reset form to original values when canceling edit
      this.tagForm.patchValue({
        name: this.data.tag.name,
        color: this.data.tag.color
      });
    }
  }

  saveChanges(): void {
    if (this.tagForm.valid) {
      const updatedTag: Tag = {
        ...this.data.tag,
        name: this.tagForm.value.name,
        color: this.tagForm.value.color
      };

      this.dialogRef.close({ action: 'edit', tag: updatedTag });
    }
  }

  deleteTag(): void {
    if (confirm('Tem certeza que deseja excluir esta tag? Esta ação não pode ser desfeita.')) {
      this.dialogRef.close({ action: 'delete', tag: this.data.tag });
    }
  }

  onClose(): void {
    this.dialogRef.close();
  }

  getTextColor(backgroundColor: string): string {
    // Convertendo o hex para RGB
    const hex = backgroundColor.replace('#', '');
    const r = parseInt(hex.substr(0, 2), 16);
    const g = parseInt(hex.substr(2, 2), 16);
    const b = parseInt(hex.substr(4, 2), 16);

    // Calculando a luminosidade
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    // Se luminância for > 0.5, usar texto preto, senão usar branco
    return luminance > 0.5 ? '#000000' : '#FFFFFF';
  }

  formatAmount(amount: number): string {
    return amount.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-PT');
  }

  getFormattedTotal(): string {
    const total = this.taggedTransactions.reduce((sum, transaction) => sum + transaction.amount, 0);
    return total.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
  }
}
