import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { AuthService } from '../components/public/auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRole = route.data['role'];
    // const userRole = this.authService.getUserRole();
    const userRole = '';

    if (userRole !== requiredRole) {
      this.router.navigate(['/unauthorized']);
      return false;
    }
    return true;
  }
}
