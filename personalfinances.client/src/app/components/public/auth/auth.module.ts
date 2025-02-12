import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SigninComponent } from './signin/signin.component';
import { SignupComponent } from './signup/signup.component';
import { AuthFormComponent } from './shared/auth-form/auth-form.component';
import { SocialMediaComponent } from './shared/social-media/social-media.component';
import { IntroductionTextComponent } from './shared/introduction-text/introduction-text.component';
import { FormsModule } from '@angular/forms';
import { AuthService } from './auth.service';
import { HeaderPublicComponent } from '../shared/header/header.component';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { HttpLoaderFactory } from '../../../app.module';
import { RouterModule } from '@angular/router';
import { ContactComponent } from './../contact/contact.component';


@NgModule({
  declarations: [
    SigninComponent,
    SignupComponent,
    AuthFormComponent,
    SocialMediaComponent,
    IntroductionTextComponent,
    HeaderPublicComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    RouterModule,

  ],
  providers: [
    AuthService
  ],
  exports: [
    SigninComponent,
    SignupComponent,
    RouterModule,
    HeaderPublicComponent
  ]
})
export class AuthModule { }
