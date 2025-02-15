import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { LayoutModule } from './shared/layout/layout.module';
import { HomeComponent } from './home/home.component';
import { DashboardComponent } from './home/dashboard/dashboard.component';
import { RouterModule } from '@angular/router';
import { GraphycsComponent } from './home/graphycs/graphyc.component';


@NgModule({
  declarations: [
    HomeComponent,
    DashboardComponent,
    GraphycsComponent
  ],
  imports: [
    CommonModule,
    NgApexchartsModule,
    LayoutModule,
    RouterModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
