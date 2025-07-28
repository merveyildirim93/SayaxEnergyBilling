import { Component } from '@angular/core';
import { InvoiceService, InvoiceRequest, InvoiceResponse } from './invoice.service';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  standalone: false
})
export class InvoiceComponent {
  request: {
    customerId: number;
    date: Date;
  } = {
      customerId: 1,
      date: new Date()
    };

  customers = [
    { id: 1, name: "Test Müşterisi" }
  ];

  result?: InvoiceResponse;

  constructor(private invoiceService: InvoiceService) { }

  calculateInvoice() {
    const selectedDate = new Date(this.request.date); // string → Date
    const formattedDate = selectedDate.toISOString().slice(0, 10);

    const requestPayload: InvoiceRequest = {
      customerId: this.request.customerId,
      month: formattedDate
    };

    this.invoiceService.calculateInvoice(requestPayload).subscribe({
      next: (res) => this.result = res,
      error: (err) => console.error('Fatura hesaplanamadı', err)
    });
  }
}
