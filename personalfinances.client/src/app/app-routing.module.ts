import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { AuthReverseGuard } from './guards/auth-reverse.guard';

const routes: Routes = [
  // Rotas públicas
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'auth', loadChildren: () => import('./components/public/auth/auth.module').then(m => m.AuthModule) },
  { path: 'public', loadChildren: () => import('./components/public/public.module').then(m => m.PublicModule) },

  // Rotas protegidas
  {
    path: 'home',
    loadChildren: () => import('./components/core/home.module').then(m => m.HomeModule),
    canActivate: [AuthGuard],
  },

  // Página de erro 404
  // { path: '404', component: NotFoundComponent },
  { path: '**', redirectTo: '404' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
