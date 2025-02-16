import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { LayoutModule } from './shared/layout/layout.module';
import { HomeComponent } from './home/home.component';
import { RouterModule } from '@angular/router';
import { DashboardModule } from './home/dashboard/dashboard.module';


@NgModule({
  declarations: [
    HomeComponent,
  ],
  imports: [
    CommonModule,
    NgApexchartsModule,
    LayoutModule,
    RouterModule,
    DashboardModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
