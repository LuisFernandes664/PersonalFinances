import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { APIResponse } from '../../../../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class DataExportService {
  private apiUrl = `${environment.apiUrl}/DataExport`;

  constructor(private http: HttpClient) { }

  importFromCsv(file: File): Observable<APIResponse<any>> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<APIResponse<any>>(`${this.apiUrl}/import/csv`, formData);
  }

  exportToCsv(startDate?: Date, endDate?: Date): Observable<Blob> {
    let url = `${this.apiUrl}/export/csv`;

    if (startDate && endDate) {
      const start = this.formatDate(startDate);
      const end = this.formatDate(endDate);
      url += `?startDate=${start}&endDate=${end}`;
    }

    return this.http.get(url, { responseType: 'blob' });
  }

  exportToExcel(startDate?: Date, endDate?: Date): Observable<Blob> {
    let url = `${this.apiUrl}/export/excel`;

    if (startDate && endDate) {
      const start = this.formatDate(startDate);
      const end = this.formatDate(endDate);
      url += `?startDate=${start}&endDate=${end}`;
    }

    return this.http.get(url, { responseType: 'blob' });
  }

  generateReport(reportType: string, startDate: Date, endDate: Date): Observable<Blob> {
    const start = this.formatDate(startDate);
    const end = this.formatDate(endDate);
    const url = `${this.apiUrl}/report/${reportType}?startDate=${start}&endDate=${end}`;

    return this.http.get(url, { responseType: 'blob' });
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
