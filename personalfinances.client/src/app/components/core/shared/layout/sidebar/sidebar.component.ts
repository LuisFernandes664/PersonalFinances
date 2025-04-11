import { Component, EventEmitter, Output, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../public/auth/auth.service';
import { ThemeService } from '../../../../../services/theme.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SidebarComponent {
  isSidebarClosed = true;
  isDarkMode = this.themeService.isDarkMode();

  @Output() sidebarToggled = new EventEmitter<boolean>();

  constructor(
    private authService: AuthService,
    private router: Router,
    private themeService: ThemeService
  ) {}

  logout(): void {
    this.authService.removeToken();
    this.router.navigate(['/signin']);
  }

  toggleSidebar(): void {
    this.isSidebarClosed = !this.isSidebarClosed;
    this.sidebarToggled.emit(this.isSidebarClosed);
  }

  toggleDarkMode(): void {
    this.isDarkMode = this.themeService.toggleDarkMode();
    if (this.isDarkMode) {
      document.body.classList.add('dark');
    } else {
      document.body.classList.remove('dark');
    }
  }
}
