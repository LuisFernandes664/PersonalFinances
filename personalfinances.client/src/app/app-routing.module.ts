import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { ContactComponent } from './components/public/contact/contact.component';
import { SigninComponent } from './components/public/auth/signin/signin.component';
import { SignupComponent } from './components/public/auth/signup/signup.component';

const routes: Routes = [
  { path: '', redirectTo: 'signin', pathMatch: 'full' },
  { path: 'signin', component: SigninComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'signin' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
