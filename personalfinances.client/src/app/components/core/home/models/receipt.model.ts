export interface Receipt {
  stampEntity: string;
  userId: string;
  imagePath: string;
  merchantName: string;
  totalAmount: number;
  receiptDate: Date | string;
  transactionId?: string;
  isProcessed: boolean;
  processingStatus: string;
  errorMessage?: string;
  imageBase64?: string;
  contentType?: string;

}
