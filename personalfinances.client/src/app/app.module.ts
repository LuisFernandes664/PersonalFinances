import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './components/core/home/dashboard/dashboard.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { AuthInterceptor } from './interceptors/auth-interceptor.service';
import { PreferencesService } from './services/preferences.service';
import { RouterModule } from '@angular/router';
import { NotificationsComponent } from './components/shared/notifications/notifications.component';
import { LoaderComponent } from './components/shared/loader/loader.component';
import { AuthModule } from './components/public/auth/auth.module';
import { PublicModule } from './components/public/public.module';
import { MatDialogModule } from '@angular/material/dialog';
import { HomeModule } from './components/core/home.module';
import { DashboardModule } from './components/core/home/dashboard/dashboard.module';

export const CUSTOM_DATE_FORMATS = {
  parse: {
    dateInput: 'DD/MM/YYYY',
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export function loadPreferencesFactory(preferencesService: PreferencesService) {
  return (): Promise<void> => preferencesService.loadPreferences();
}

@NgModule({
  declarations: [
    AppComponent,
    NotificationsComponent,
    LoaderComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    RouterModule,
    FormsModule,
    PublicModule,
    AuthModule,
    HomeModule,
    DashboardModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    MatDialogModule
  ],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideAnimationsAsync(),
    {
      provide: APP_INITIALIZER,
      useFactory: loadPreferencesFactory,
      deps: [PreferencesService],
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})export class AppModule { }
