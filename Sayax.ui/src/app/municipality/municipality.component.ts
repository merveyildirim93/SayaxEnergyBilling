import { Component } from '@angular/core';
import { MunicipalityTax, MunicipalityTaxService } from './municipality.service';

@Component({
  selector: 'app-municipality-tax',
  templateUrl: './municipality.component.html',
  standalone:false
})
export class MunicipalityComponent {
  date: string = ''; // ⬅️ Date değil, string
  report: MunicipalityTax[] = [];

  constructor(private taxService: MunicipalityTaxService) { }

  calculateTaxes() {
    this.taxService.getTaxes(this.date).subscribe({
      next: (data: MunicipalityTax[]) => {
        this.report = data;
      },
      error: (err: any) => console.error('Fatura hesaplanamadı', err)
    });
  }
}

