import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MunicipalityTax {
  municipality: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class MunicipalityTaxService {

  private apiUrl = 'https://localhost:7289/api/tax/GetBtvReport'; // backend endpointin

  constructor(private http: HttpClient) { }

  getTaxes(date: string): Observable<MunicipalityTax[]> {
    return this.http.post<MunicipalityTax[]>(this.apiUrl, {
      month: date  // ⬅️ .toISOString() değil, zaten yyyy-MM-dd geliyor
    });
  }
}
