import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { TransactionAddComponent } from './transaction-add/transaction-add.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LayoutModule } from '../../shared/layout/layout.module';
import { PipeModule } from '../../../../pipes/pipe.module';

import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { GraphycComponent } from '../graphycs/graphyc.component';

@NgModule({
  declarations: [
    DashboardComponent,
    TransactionAddComponent,
    GraphycComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutModule,
    PipeModule,
    NgApexchartsModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule
  ],
  exports: [
    DashboardComponent,
    GraphycComponent
  ],
  providers: [
    MatDatepickerModule,
    MatNativeDateModule
  ]
})
export class DashboardModule { }
