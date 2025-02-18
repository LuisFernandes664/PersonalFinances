import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { APIResponse } from '../../../../../models/api-response.model';
import { ChartSeries } from '../../models/chart-series.model';

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private apiUrl = `${environment.apiUrl}/chartdata`;

  constructor(private http: HttpClient) {}

  getChartData(interval: string): Observable<APIResponse<ChartSeries[]>> {
    return this.http.get<APIResponse<ChartSeries[]>>(`${this.apiUrl}?interval=${interval}`);
  }
}
