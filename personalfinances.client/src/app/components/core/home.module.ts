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
import { BudgetComponent } from './home/budget/budget.component';
import { GoalComponent } from './home/goal/goal.component';
import { FormControl, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { SavingPlanDialogComponent } from './home/graphycs/saving-plan/saving-plan-dialog/saving-plan-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
  declarations: [
    HomeComponent,
    OverviewComponent,
    CardComponent,
    ChartSectionComponent,
    SavingPlanComponent,
    RecentTransactionsComponent,
    GraphycComponent,
    BudgetComponent,
    GoalComponent,
    SavingPlanDialogComponent
  ],
  imports: [
    CommonModule,
    NgApexchartsModule,
    LayoutModule,
    RouterModule,
    DashboardModule,
    BrowserModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
    MatIconModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
