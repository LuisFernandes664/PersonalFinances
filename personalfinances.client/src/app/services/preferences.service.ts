import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ThemeService } from './theme.service';

export interface UserPreferences {
  darkMode: boolean;
  language: string;
}

@Injectable({
  providedIn: 'root'
})
export class PreferencesService {
  private readonly DARK_MODE_KEY = 'darkMode';
  private readonly LANGUAGE_KEY = 'language';

  private defaultPreferences: UserPreferences = {
    darkMode: false,
    language: 'pt'
  };

  private preferencesSubject = new BehaviorSubject<UserPreferences>(this.defaultPreferences);
  public preferences$ = this.preferencesSubject.asObservable();

  constructor(private themeService: ThemeService) {
    this.loadPreferences();
  }

  /**
   * Carrega as preferências do utilizador a partir do localStorage.
   * Atualiza o estado das preferências e aplica o modo escuro, se necessário.
  */
  async loadPreferences(): Promise<void> {
    try {
      const storedDarkMode = JSON.parse(localStorage.getItem(this.DARK_MODE_KEY) || 'false');
      const storedLanguage = localStorage.getItem(this.LANGUAGE_KEY) || 'pt';

      const loadedPreferences: UserPreferences = {
        darkMode: storedDarkMode,
        language: storedLanguage,
      };

      this.preferencesSubject.next(loadedPreferences);
      this.themeService.loadTheme();
    } catch (error) {
      console.error('Falha ao carregar as preferências:', error);
    }
  }

  /**
   * Atualiza as preferências do utilizador.
   * @param newPreferences - Novas preferências a serem aplicadas.
   * Atualiza o localStorage e aplica ou remove o modo escuro no DOM.
   */
  updatePreferences(newPreferences: Partial<UserPreferences>): void {
    try {
      const current = this.preferencesSubject.value;
      const updated = { ...current, ...newPreferences };

      this.preferencesSubject.next(updated);

      localStorage.setItem(this.DARK_MODE_KEY, JSON.stringify(updated.darkMode));
      localStorage.setItem(this.LANGUAGE_KEY, updated.language);

      this.themeService.toggleDarkMode();
    } catch (error) {
      console.error('Falha ao atualizar as preferências:', error);
    }
  }

}
