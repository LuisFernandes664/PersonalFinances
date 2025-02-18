import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { LayoutModule } from './shared/layout/layout.module';
import { HomeComponent } from './home/home.component';
import { RouterModule } from '@angular/router';
import { DashboardModule } from './home/dashboard/dashboard.module';
import { OverviewComponent } from './home/graphycs/overview/overview.component';
import { CardComponent } from './home/graphycs/card/card.component';
import { ChartSectionComponent } from './home/graphycs/chart-section/chart-section.component';
import { SavingPlanComponent } from './home/graphycs/saving-plan/saving-plan.component';
import { RecentTransactionsComponent } from './home/graphycs/recent-transactions/recent-transactions.component';
import { GraphycComponent } from './home/graphycs/graphyc.component';


@NgModule({
  declarations: [
    HomeComponent,
    OverviewComponent,
    CardComponent,
    ChartSectionComponent,
    SavingPlanComponent,
    RecentTransactionsComponent,
    GraphycComponent
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
