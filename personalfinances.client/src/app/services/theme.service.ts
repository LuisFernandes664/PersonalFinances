import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private darkModeKey = 'darkMode';

  constructor() {
    this.loadTheme();
  }

  toggleDarkMode() {
    const darkMode = !this.isDarkMode();
    localStorage.setItem(this.darkModeKey, JSON.stringify(darkMode));
    document.body.classList.toggle('dark', darkMode);

    return darkMode;
  }

  isDarkMode(): boolean {
    return JSON.parse(localStorage.getItem(this.darkModeKey) || 'false');
  }

  loadTheme() {
    if (this.isDarkMode()) {
      document.body.classList.add('dark');
    }
  }
}
