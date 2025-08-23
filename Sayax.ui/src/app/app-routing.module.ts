import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component'; 
import { InvoiceComponent } from './invoice/invoice.component';
import { InvoiceAllComponent } from './invoice/invoice-all.component';
import { MunicipalityComponent } from './municipality/municipality.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'invoice', component: InvoiceComponent },
  { path: 'invoice-all', component: InvoiceAllComponent },
  { path: 'municipality', component: MunicipalityComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
