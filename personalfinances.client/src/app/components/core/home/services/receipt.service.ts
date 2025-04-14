import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
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

  uploadReceipt(file: File): Observable<any> {
    // Criar um objeto FormData para enviar o arquivo
    const formData = new FormData();
    formData.append('file', file, file.name);

    // Configurar os headers corretamente
    const headers = new HttpHeaders();
    // Não defina o Content-Type aqui - o navegador vai definir automaticamente com o boundary correto

    return this.http.post<any>(`${this.apiUrl}/upload`, formData, { headers })
      .pipe(
        catchError(error => {
          console.error('Erro ao fazer upload do arquivo:', error);
          throw error;
        })
      )
  }
  processReceipt(id: string): Observable<APIResponse<Receipt>> {
    return this.http.post<APIResponse<Receipt>>(`${this.apiUrl}/${id}/process`, {});
  }

  linkToTransaction(receiptId: string, transactionId: string): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}/${receiptId}/link/${transactionId}`, {});
  }
}
