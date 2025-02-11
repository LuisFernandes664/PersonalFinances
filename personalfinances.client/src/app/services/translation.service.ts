import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class TranslationService {
  private defaultLang = 'pt';

  constructor(private translate: TranslateService) {
    const lang = localStorage.getItem('lang') || this.defaultLang;
    this.translate.setDefaultLang(lang);
    this.translate.use(lang);
  }

  switchLanguage(lang: string): void {
    localStorage.setItem('lang', lang);
    this.translate.use(lang);
  }

  instant(key: string, interpolateParams?: any): string {
    return this.translate.instant(key, interpolateParams);
  }
}
