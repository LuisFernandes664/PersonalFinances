import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { AuthModule } from './auth/auth.module';
import { ContactComponent } from './contact/contact.component';
import { HeaderPublicComponent } from './shared/header/header.component';
import { BrowserModule } from '@angular/platform-browser';
import { BlogComponent } from './blog/blog.component';


@NgModule({
  declarations: [
    ContactComponent,
    BlogComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule.forChild(),
    RouterModule,
    AuthModule,
    BrowserModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [
    ContactComponent,
    BlogComponent
  ]
})
export class PublicModule { }
