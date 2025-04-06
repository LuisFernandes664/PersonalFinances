import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Receipt } from '../models/receipt.model';
import { APIResponse } from '../../../../models/api-response.model';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReceiptService {
  private apiUrl = `${environment.apiUrl}/Receipts`;

  constructor(private http: HttpClient) { }

  getReceipts(): Observable<APIResponse<Receipt[]>> {
    return this.http.get<APIResponse<Receipt[]>>(this.apiUrl);
  }

  getReceipt(id: string): Observable<APIResponse<Receipt>> {
    return this.http.get<APIResponse<Receipt>>(`${this.apiUrl}/${id}`);
  }

  uploadReceipt(file: File): Observable<APIResponse<Receipt>> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<APIResponse<Receipt>>(`${this.apiUrl}/upload`, formData);
  }

  processReceipt(id: string): Observable<APIResponse<Receipt>> {
    return this.http.post<APIResponse<Receipt>>(`${this.apiUrl}/${id}/process`, {});
  }

  linkToTransaction(receiptId: string, transactionId: string): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}/${receiptId}/link/${transactionId}`, {});
  }
}
