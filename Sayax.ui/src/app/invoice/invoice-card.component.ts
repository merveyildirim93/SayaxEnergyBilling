import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InvoiceResponse } from './invoice.service';

@Component({
  selector: 'app-invoice-card',
  templateUrl: './invoice-card.component.html',
  standalone: true,
  imports: [CommonModule]
})
export class InvoiceCardComponent {
  @Input() invoice!: InvoiceResponse;
  @Input() month!: string;
  @Output() recalculate = new EventEmitter<{ customerId: number; month: string }>();

  recalc() {
    this.recalculate.emit({ customerId: this.invoice.customerId, month: this.month });
  }
}
