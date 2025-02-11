import { Component, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TranslationService } from '../../../../services/translation.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class HeaderComponent {
  currentLang: string = localStorage.getItem('lang') || 'pt';

  constructor(private translationService: TranslationService, private router: Router) {
  }

  switchLanguage(event: Event) {
    const target = event.target as HTMLSelectElement;
    if (target) {
      this.currentLang = target.value;
      this.translationService.switchLanguage(this.currentLang);
    }
  }

  goToSignup() {
    this.router.navigate(['/signup']);
  }

}
