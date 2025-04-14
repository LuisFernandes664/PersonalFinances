import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TransactionService } from '../../transaction.service';
import { Transaction } from '../../models/transaction.model';
import { Receipt } from '../../models/receipt.model';

@Component({
  selector: 'app-receipt-detail-dialog',
  templateUrl: './receipt-detail-dialog.component.html',
  styleUrls: ['./receipt-detail-dialog.component.scss']
})
export class ReceiptDetailDialogComponent implements OnInit {
  linkedTransaction: Transaction | null = null;
  isLoading: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<ReceiptDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { receipt: Receipt },
    private transactionService: TransactionService
  ) {}

  ngOnInit(): void {
    this.loadLinkedTransaction();
  }

  loadLinkedTransaction(): void {
    if (this.data.receipt.transactionId) {
      this.isLoading = true;
      this.transactionService.getTransactionByStampEntityAsync(this.data.receipt.transactionId)
        .subscribe(
          transaction => {
            this.linkedTransaction = transaction;
            this.isLoading = false;
          },
          error => {
            console.error('Erro ao carregar transação vinculada:', error);
            this.isLoading = false;
          }
        );
    }
  }

  onClose(): void {
    this.dialogRef.close();
  }

  formatDate(date: string | Date): string {
    if (!date) return '';
    return new Date(date).toLocaleDateString('pt-PT');
  }

  formatAmount(amount: number): string {
    return amount.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
  }

  // Obtém o caminho de exibição de imagem apropriado
  getImagePath(path: string): string {
    // Na implementação real, isso pode precisar ser ajustado dependendo de como as imagens são servidas
    return path;
  }

  getImageUrl(receipt: Receipt): string {
    if (receipt.imageBase64 && receipt.contentType) {
      return `url(data:${receipt.contentType};base64,${receipt.imageBase64})`;
    } else if (receipt.imagePath) {
      return `url(/assets/receipt-previews/${receipt.imagePath})`;
    } else {
      return 'url(/assets/images/receipt-placeholder.jpg)';
    }
  }
}
