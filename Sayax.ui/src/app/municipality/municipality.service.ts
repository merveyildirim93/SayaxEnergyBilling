import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MunicipalityTax {
  customerId: number;
  customerName: string;
  btvAmount: number;
  municipality: string;
}

@Injectable({
  providedIn: 'root'
})
export class MunicipalityTaxService {

  private apiUrl = 'https://localhost:7289/api/tax/btv-report'; 

  constructor(private http: HttpClient) { }

  getTaxes(date: string): Observable<MunicipalityTax[]> {
    return this.http.post<MunicipalityTax[]>(
      `${this.apiUrl}/?month=${date}`,
      {}
    );
  }
}
