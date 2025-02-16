import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/core/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { ContactComponent } from './components/public/contact/contact.component';
import { SigninComponent } from './components/public/auth/signin/signin.component';
import { SignupComponent } from './components/public/auth/signup/signup.component';
import { BlogComponent } from './components/public/blog/blog.component';
import { DashboardComponent } from './components/core/home/dashboard/dashboard.component';
import { GraphycComponent } from './components/core/home/graphycs/graphyc.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'signin', component: SigninComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'blog', component: BlogComponent },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'graphyc', component: GraphycComponent }
    ]
  },
  { path: '**', redirectTo: 'signin' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
