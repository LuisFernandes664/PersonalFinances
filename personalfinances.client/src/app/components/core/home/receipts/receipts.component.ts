import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { TransactionService } from '../transaction.service';
import { MatDialog } from '@angular/material/dialog';
import { Receipt } from '../models/receipt.model';
import { ReceiptService } from '../services/receipt.service';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { ReceiptDetailDialogComponent } from './receipt-detail-dialog/receipt-detail-dialog.component';
import { LinkTransactionDialogComponent } from './link-transaction-dialog/link-transaction-dialog.component';

@Component({
  selector: 'app-receipts',
  templateUrl: './receipts.component.html',
  styleUrls: ['./receipts.component.scss']
})
export class ReceiptsComponent implements OnInit {

  @ViewChild('receiptUpload', { static: false }) receiptUpload!: ElementRef;

  receipts: Receipt[] = [];
  filteredReceipts: Receipt[] = [];
  isLoading: boolean = false;
  searchTerm: string = '';

  // Filtros
  filterOptions = {
    processedOnly: false,
    linkedOnly: false,
    unlinkedOnly: false,
    sortBy: 'date' // 'date', 'merchant', 'amount'
  };

  constructor(
    private receiptService: ReceiptService,
    private transactionService: TransactionService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadReceipts();
  }

  loadReceipts(): void {
    this.isLoading = true;
    this.receiptService.getReceipts().subscribe(
      response => {
        this.receipts = response.data;
        this.applyFilters(); // Aplica os filtros iniciais
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao carregar recibos. Por favor, tente novamente.');
        console.error('Erro ao carregar recibos:', error);
      }
    );
  }

  applyFilters(): void {
    let filteredData = [...this.receipts];

    // Aplicar termo de busca
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filteredData = filteredData.filter(receipt =>
        receipt.merchantName.toLowerCase().includes(term)
      );
    }

    // Aplicar filtros de status
    if (this.filterOptions.processedOnly) {
      filteredData = filteredData.filter(receipt => receipt.isProcessed);
    }

    if (this.filterOptions.linkedOnly) {
      filteredData = filteredData.filter(receipt => !!receipt.transactionId);
    }

    if (this.filterOptions.unlinkedOnly) {
      filteredData = filteredData.filter(receipt => !receipt.transactionId);
    }

    // Aplicar ordenação
    switch (this.filterOptions.sortBy) {
      case 'date':
        filteredData.sort((a, b) => new Date(b.receiptDate).getTime() - new Date(a.receiptDate).getTime());
        break;
      case 'merchant':
        filteredData.sort((a, b) => a.merchantName.localeCompare(b.merchantName));
        break;
      case 'amount':
        filteredData.sort((a, b) => b.totalAmount - a.totalAmount);
        break;
    }

    this.filteredReceipts = filteredData;
  }

  onSearch(event: Event): void {
    this.searchTerm = (event.target as HTMLInputElement).value;
    this.applyFilters();
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.filterOptions = {
      processedOnly: false,
      linkedOnly: false,
      unlinkedOnly: false,
      sortBy: 'date'
    };
    this.applyFilters();
  }

  uploadReceipt(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf'];
    if (!allowedTypes.includes(file.type)) {
      this.notificationService.showToast('error', 'Formato de arquivo não suportado. Por favor, use JPEG, PNG, GIF ou PDF.');
      return;
    }

    this.isLoading = true;
    this.receiptService.uploadReceipt(file).subscribe(
      response => {
        this.notificationService.showToast('success', 'Recibo enviado com sucesso!');
        this.loadReceipts();
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao enviar recibo. Por favor, tente novamente.');
        console.error('Erro ao enviar recibo:', error);
      }
    );
  }

  processReceipt(receiptId: string): void {
    this.isLoading = true;
    this.receiptService.processReceipt(receiptId).subscribe(
      response => {
        this.notificationService.showToast('success', 'Recibo processado com sucesso!');
        this.loadReceipts();
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao processar recibo. Por favor, tente novamente.');
        console.error('Erro ao processar recibo:', error);
      }
    );
  }

  openReceiptDetail(receipt: Receipt): void {
    this.dialog.open(ReceiptDetailDialogComponent, {
      width: '800px',
      data: { receipt }
    });
  }

  openLinkTransactionDialog(receipt: Receipt): void {
    const dialogRef = this.dialog.open(LinkTransactionDialogComponent, {
      width: '600px',
      data: { receipt }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.transactionId) {
        this.linkReceiptToTransaction(receipt.stampEntity, result.transactionId);
      }
    });
  }

  linkReceiptToTransaction(receiptId: string, transactionId: string): void {
    this.isLoading = true;
    this.receiptService.linkToTransaction(receiptId, transactionId).subscribe(
      response => {
        this.notificationService.showToast('success', 'Recibo vinculado à transação com sucesso!');
        this.loadReceipts();
      },
      error => {
        this.isLoading = false;
        this.notificationService.showToast('error', 'Erro ao vincular recibo à transação. Por favor, tente novamente.');
        console.error('Erro ao vincular recibo:', error);
      }
    );
  }

  formatDate(date: string | Date): string {
    if (!date) return '';
    return new Date(date).toLocaleDateString('pt-PT');
  }

  formatCurrency(amount: number): string {
    return amount.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
  }

  triggerFileUpload(): void {
    this.receiptUpload.nativeElement.click();
  }

  triggerFileInputClick(): void {
    this.receiptUpload.nativeElement.click();
  }
}
