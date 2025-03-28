import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class TranslationService {
  private readonly LANG_KEY = 'lang';
  private readonly defaultLang = 'pt';

  constructor(private translate: TranslateService) {
    const lang = localStorage.getItem(this.LANG_KEY) || this.defaultLang;
    this.setLanguage(lang);
  }

  switchLanguage(lang: string): void {
    if (!lang) return;
    localStorage.setItem(this.LANG_KEY, lang);
    this.translate.use(lang);
  }

  instant(key: string, interpolateParams?: any): string {
    return this.translate.instant(key, interpolateParams);
  }

  private setLanguage(lang: string): void {
    this.translate.setDefaultLang(lang);
    this.translate.use(lang);
  }

}
