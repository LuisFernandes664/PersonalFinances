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
import { RecentTransactionsComponent } from './home/graphycs/recent-transactions/recent-transactions.component';
import { GraphycComponent } from './home/graphycs/graphyc.component';
import { FormControl, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule, MatOption, MatOptionModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { SavingPlanDialogComponent } from './home/graphycs/saving-plan/saving-plan-dialog/saving-plan-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select'
import { GoalComponent } from './home/graphycs/goal/goal.component';
import { BudgetComponent } from './home/graphycs/budget/budget.component';
import { BaseSavingPlanComponent } from './home/graphycs/saving-plan/base-saving-plan/base-saving-plan.component';
import { BudgetSavingPlanComponent } from './home/graphycs/saving-plan/budget-saving-plan/budget-saving-plan.component';
import { GoalSavingPlanComponent } from './home/graphycs/saving-plan/goal-saving-plan/goal-saving-plan.component';

@NgModule({
  declarations: [
    HomeComponent,
    OverviewComponent,
    CardComponent,
    ChartSectionComponent,
    RecentTransactionsComponent,
    GraphycComponent,
    SavingPlanDialogComponent,
    GoalComponent,
    BudgetComponent,
    BaseSavingPlanComponent,
    BudgetSavingPlanComponent,
    GoalSavingPlanComponent
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
    MatIconModule,
    MatOptionModule,
    MatSelectModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
