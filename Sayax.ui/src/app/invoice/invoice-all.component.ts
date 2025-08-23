import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService, InvoiceResponse } from './invoice.service';
import { InvoiceCardComponent } from './invoice-card.component';

@Component({
  selector: 'app-invoice-all',
  templateUrl: './invoice-all.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule, InvoiceCardComponent]
})
export class InvoiceAllComponent {
  month!: string;
  results: InvoiceResponse[] = [];

  constructor(private invoiceService: InvoiceService) { }

  calculateAll() {
    if (!this.month) return;
    this.invoiceService.calculateAllInvoices(this.month).subscribe({
      next: (data) => this.results = data,
      error: (err) => console.error('Toplu fatura hesaplama başarısız', err)
    });
  }

  recalculateSingle(customerId: number, month: string) {
    this.invoiceService.calculateInvoice({ customerId, month }).subscribe({
      next: (res) => {
        const index = this.results.findIndex(r => r.customerId === customerId);
        if (index >= 0) this.results[index] = res;
      },
      error: (err) => console.error('Fatura yeniden hesaplanamadı', err)
    });
  }
}
