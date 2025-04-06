import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tag } from '../models/tag.model';
import { Transaction } from '../models/transaction.model';
import { APIResponse } from '../../../../models/api-response.model';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private apiUrl = `${environment.apiUrl}/Tags`;

  constructor(private http: HttpClient) { }

  getTags(): Observable<APIResponse<Tag[]>> {
    return this.http.get<APIResponse<Tag[]>>(this.apiUrl);
  }

  getTag(id: string): Observable<APIResponse<Tag>> {
    return this.http.get<APIResponse<Tag>>(`${this.apiUrl}/${id}`);
  }

  createTag(tag: Tag): Observable<APIResponse<Tag>> {
    return this.http.post<APIResponse<Tag>>(this.apiUrl, tag);
  }

  updateTag(id: string, tag: Tag): Observable<APIResponse<Tag>> {
    return this.http.put<APIResponse<Tag>>(`${this.apiUrl}/${id}`, tag);
  }

  deleteTag(id: string): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`);
  }

  addTagToTransaction(transactionId: string, tagId: string): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}/transaction/${transactionId}/tag/${tagId}`, {});
  }

  removeTagFromTransaction(transactionId: string, tagId: string): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/transaction/${transactionId}/tag/${tagId}`);
  }

  getTransactionsByTag(tagId: string): Observable<APIResponse<Transaction[]>> {
    return this.http.get<APIResponse<Transaction[]>>(`${this.apiUrl}/transaction/${tagId}`);
  }
}
