import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InvoiceComponent } from './invoice/invoice.component';
import { InvoiceAllComponent } from './invoice/invoice-all.component';
import { MunicipalityComponent } from './municipality/municipality.component';

@NgModule({
  declarations: [
    AppComponent,
    InvoiceComponent,
    InvoiceAllComponent,
    MunicipalityComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
