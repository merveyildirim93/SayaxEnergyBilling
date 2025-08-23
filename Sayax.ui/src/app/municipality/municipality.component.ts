import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';
import { MunicipalityTaxService, MunicipalityTax } from './municipality.service';

@Component({
  selector: 'app-municipality-tax',
  templateUrl: './municipality.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class MunicipalityComponent {
  date: string = '';
  report: MunicipalityTax[] = [];

  constructor(private taxService: MunicipalityTaxService) { }

  calculateTaxes() {
    if (!this.date) return;

    this.taxService.getTaxes(this.date).subscribe({
      next: (data: MunicipalityTax[]) => {
        this.report = data;
      },
      error: (err: any) => console.error('Vergi raporu alınamadı', err)
    });
  }
}
