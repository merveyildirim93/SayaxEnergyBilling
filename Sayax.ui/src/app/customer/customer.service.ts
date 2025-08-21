import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CustomerDto {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiUrl = 'https://localhost:7289/api/customer/getAll';

  constructor(private http: HttpClient) { }

  getCustomers(): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(this.apiUrl);
  }
}
