import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface InvoiceRequest {
  customerId: number;
  month: string; // YYYY-MM-DD
}

export interface InvoiceResponse {
  customerId: number;
  customerName: string;
  energyCost: number;
  distributionCost: number;
  btv: number;
  kdv: number;
  totalInvoice: number;
}

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private baseUrl = 'https://localhost:7289/api/invoice'; 

  constructor(private http: HttpClient) {}

  calculateInvoice(request: InvoiceRequest): Observable<InvoiceResponse> {
    return this.http.post<InvoiceResponse>(`${this.baseUrl}/calculate`, request);
  }

  calculateAllInvoices(month: string): Observable<InvoiceResponse[]> {
    return this.http.post<InvoiceResponse[]>(
      `${this.baseUrl}/calculate-all?month=${month}`,
      {}
    );
  }

}
