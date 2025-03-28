import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly darkModeKey = 'darkMode';
  private renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
    this.loadTheme();
  }

  toggleDarkMode(): boolean {
    const darkMode = !this.isDarkMode();
    localStorage.setItem(this.darkModeKey, JSON.stringify(darkMode));
    this.updateBodyClass(darkMode);
    return darkMode;
  }

  isDarkMode(): boolean {
    return JSON.parse(localStorage.getItem(this.darkModeKey) || 'false');
  }

  private updateBodyClass(darkMode: boolean): void {
    if (darkMode) {
      this.renderer.addClass(document.body, 'dark');
    } else {
      this.renderer.removeClass(document.body, 'dark');
    }
  }

  loadTheme(): void {
    this.updateBodyClass(this.isDarkMode());
  }
}
