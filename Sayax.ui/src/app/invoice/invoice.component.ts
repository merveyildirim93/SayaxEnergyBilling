import { Component, OnInit } from '@angular/core';
import { InvoiceService, InvoiceRequest, InvoiceResponse } from './invoice.service';
import { CustomerService, CustomerDto } from '../customer/customer.service';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  standalone: false
})
export class InvoiceComponent implements OnInit {
  customers: CustomerDto[] = [];

  request: InvoiceRequest = {
    customerId: 0,
    month: ''
  };

  result?: InvoiceResponse;

  constructor(
    private invoiceService: InvoiceService,
    private customerService: CustomerService
  ) { }

  ngOnInit() {
    this.customerService.getCustomers().subscribe({
      next: (data: CustomerDto[]) => this.customers = data,
      error: (err: any) => console.error('Müşteriler yüklenemedi', err)
    });
  }

  calculateInvoice() {
    if (!this.request.customerId || !this.request.month) return;

    this.invoiceService.calculateInvoice(this.request).subscribe({
      next: (res) => this.result = res,
      error: (err) => console.error('Fatura hesaplanamadı', err)
    });
  }
}
