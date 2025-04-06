import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import { LayoutModule } from './shared/layout/layout.module';
import { HomeComponent } from './home/home.component';
import { RouterModule, Routes } from '@angular/router';
import { DashboardModule } from './home/dashboard/dashboard.module';
import { OverviewComponent } from './home/graphycs/overview/overview.component';
import { CardComponent } from './home/graphycs/card/card.component';
import { ChartSectionComponent } from './home/graphycs/chart-section/chart-section.component';
import { RecentTransactionsComponent } from './home/graphycs/recent-transactions/recent-transactions.component';
import { GraphycComponent } from './home/graphycs/graphyc.component';
import { FormControl, FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { SavingPlanDialogComponent } from './home/graphycs/saving-plan/saving-plan-dialog/saving-plan-dialog.component';
import { GoalComponent } from './home/graphycs/goal/goal.component';
import { BudgetComponent } from './home/graphycs/budget/budget.component';
import { BaseSavingPlanComponent } from './home/graphycs/saving-plan/base-saving-plan/base-saving-plan.component';
import { BudgetSavingPlanComponent } from './home/graphycs/saving-plan/budget-saving-plan/budget-saving-plan.component';
import { GoalSavingPlanComponent } from './home/graphycs/saving-plan/goal-saving-plan/goal-saving-plan.component';
import { AuthGuard } from '../../guards/auth.guard';
import { DashboardComponent } from './home/dashboard/dashboard.component';
import { RecurringTransactionsComponent } from './home/recurring-transactions/recurring-transactions.component';
import { TagsComponent } from './home/tags/tags.component';
import { FinancialHealthComponent } from './home/financial-health/financial-health.component';
import { ReceiptsComponent } from './home/receipts/receipts.component';
import { CurrencyComponent } from './home/currency/currency.component';
import { CalendarComponent } from './home/calendar/calendar.component';
import { DataExportComponent } from './home/data-export/data-export.component';
import { ReactiveFormsModule } from '@angular/forms';

// FullCalendar
import { FullCalendarModule } from '@fullcalendar/angular';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';

// Dialog Components
import { RecurringTransactionDialogComponent } from './home/recurring-transactions/recurring-transaction-dialog/recurring-transaction-dialog.component';
import { TagDetailsDialogComponent } from './home/tags/tag-details-dialog/tag-details-dialog.component';
import { ReceiptDetailDialogComponent } from './home/receipts/receipt-detail-dialog/receipt-detail-dialog.component';
import { LinkTransactionDialogComponent } from './home/receipts/link-transaction-dialog/link-transaction-dialog.component';
import { CreateEventDialogComponent } from './home/calendar/create-event-dialog/create-event-dialog.component';
import { EventDetailDialogComponent } from './home/calendar/event-detail-dialog/event-detail-dialog.component';

import { PipeModule } from "../../pipes/pipe.module";

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: HomeComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'graphyc', component: GraphycComponent },
      { path: 'recurring-transactions', component: RecurringTransactionsComponent },
      { path: 'tags', component: TagsComponent },
      { path: 'financial-health', component: FinancialHealthComponent },
      { path: 'receipts', component: ReceiptsComponent },
      { path: 'calendar', component: CalendarComponent },
      { path: 'export', component: DataExportComponent }
    ]
  }
];

@NgModule({
  declarations: [
    HomeComponent,
    RecurringTransactionDialogComponent,
    TagDetailsDialogComponent,
    ReceiptDetailDialogComponent,
    LinkTransactionDialogComponent,
    CreateEventDialogComponent,
    EventDetailDialogComponent,
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
    GoalSavingPlanComponent,
    RecurringTransactionsComponent,
    TagsComponent,
    FinancialHealthComponent,
    ReceiptsComponent,
    CurrencyComponent,
    CalendarComponent,
    DataExportComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LayoutModule,
    RouterModule,
    DashboardModule,
    FormsModule,
    // Material
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatListModule,
    MatExpansionModule,
    MatTooltipModule,
    MatTabsModule,
    // Third Party
    FullCalendarModule,
    NgApexchartsModule,
    RouterModule.forChild(routes),
    PipeModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
