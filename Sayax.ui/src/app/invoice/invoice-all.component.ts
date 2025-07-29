import { Component } from '@angular/core';
import { InvoiceService, InvoiceResponse } from '../invoice/invoice.service';

@Component({
  selector: 'app-invoice-all',
  templateUrl: './invoice-all.component.html',
  standalone: false
})
export class InvoiceAllComponent {
  month!: string;
  results: InvoiceResponse[] = [];

  constructor(private invoiceService: InvoiceService) { }

  calculateAll() {
    if (!this.month) return;
    console.log(this.month);
    this.invoiceService.calculateAllInvoices(this.month).subscribe({
      next: (data) => this.results = data,
      error: (err) => console.error('Toplu fatura hesaplama başarısız', err)
    });
  }
}
