import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface UserPreferences {
  darkMode: boolean;
  language: string;
}

@Injectable({
  providedIn: 'root'
})
export class PreferencesService {
  private defaultPreferences: UserPreferences = {
    darkMode: false,
    language: 'pt'
  };

  private preferencesSubject = new BehaviorSubject<UserPreferences>(this.defaultPreferences);
  public preferences$ = this.preferencesSubject.asObservable();

  constructor() {}

  loadPreferences(): Promise<void> {
    return new Promise(resolve => {
      const storedDarkMode = JSON.parse(localStorage.getItem('darkMode') || 'false');
      if (storedDarkMode) {
        document.body.classList.add('dark');
      }
      const loadedPreferences: UserPreferences = {
        darkMode: storedDarkMode,
        language: localStorage.getItem('language') || 'pt'
      };

      this.preferencesSubject.next(loadedPreferences);
      resolve();
    });
  }

  updatePreferences(newPreferences: Partial<UserPreferences>): void {
    const current = this.preferencesSubject.value;
    const updated = { ...current, ...newPreferences };
    this.preferencesSubject.next(updated);
    localStorage.setItem('darkMode', JSON.stringify(updated.darkMode));
    localStorage.setItem('language', updated.language);
  }
}
