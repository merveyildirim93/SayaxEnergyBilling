import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService, InvoiceRequest, InvoiceResponse } from './invoice.service';
import { CustomerService, CustomerDto } from '../customer/customer.service';
import { InvoiceCardComponent } from './invoice-card.component';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule, InvoiceCardComponent]
})
export class InvoiceComponent implements OnInit {
  customers: CustomerDto[] = [];
  request: InvoiceRequest = { customerId: 0, month: '' };
  result?: InvoiceResponse;

  constructor(
    private invoiceService: InvoiceService,
    private customerService: CustomerService
  ) { }

  ngOnInit() {
    this.customerService.getCustomers().subscribe({
      next: (data) => this.customers = data,
      error: (err) => console.error('Müşteriler yüklenemedi', err)
    });
  }

  calculateInvoice() {
    if (!this.request.customerId || !this.request.month) return;

    this.invoiceService.calculateInvoice(this.request).subscribe({
      next: (res) => this.result = res,
      error: (err) => console.error('Fatura hesaplanamadı', err)
    });
  }

  recalculateSingle(customerId: number, month: string) {
    this.invoiceService.calculateInvoice({ customerId, month }).subscribe({
      next: (res) => this.result = res,
      error: (err) => console.error('Fatura yeniden hesaplanamadı', err)
    });
  }
}
