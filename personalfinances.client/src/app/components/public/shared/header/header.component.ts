import { Component, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { TranslationService } from '../../../../services/translation.service';

@Component({
  selector: 'app-header-public',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class HeaderPublicComponent {
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
