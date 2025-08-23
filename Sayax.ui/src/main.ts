import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { importProvidersFrom } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter([
      { path: '', loadComponent: () => import('./app/home/home.component').then(m => m.HomeComponent) },
      { path: 'invoice', loadComponent: () => import('./app/invoice/invoice.component').then(m => m.InvoiceComponent) },
      { path: 'invoice-all', loadComponent: () => import('./app/invoice/invoice-all.component').then(m => m.InvoiceAllComponent) },
      { path: 'municipality', loadComponent: () => import('./app/municipality/municipality.component').then(m => m.MunicipalityComponent) },
    ]),
    importProvidersFrom(FormsModule),
    importProvidersFrom(HttpClientModule)
  ]
}).catch(err => console.error(err));
