import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppleAuthService } from '../../services/apple-auth.service';
import { GoogleAuthService } from '../../services/google-auth.service';
import { FacebookAuthService } from '../../services/facebook-auth.service';
import { AuthService } from '../../auth.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-social-media',
  templateUrl: './social-media.component.html',
  styleUrls: ['./social-media.component.scss']
})
export class SocialMediaComponent {

  constructor(
    private appleAuthService: AppleAuthService,
    private googleAuthService: GoogleAuthService,
    private facebookAuthService: FacebookAuthService,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  signInWithApple(): void {
    this.appleAuthService.signIn()
      .then(token => {
        const data = { provider: 'apple', token: token };
        this.authService.signIn(data).subscribe(
          res => {
            if (res.status === 200) {
              localStorage.setItem('jwtToken', res.json.token);
              this.notificationService.showToast('success', 'Sucesso: Login efetuado com sucesso.');
              setTimeout(() => this.router.navigate(['/home']), 3000);
            } else {
              this.notificationService.showToast('error', 'Erro: Problema no login com Apple.');
            }
          },
          err => {
            this.notificationService.showToast('error', 'Erro: Algo correu mal no login com Apple.');
          }
        );
      })
      .catch(err => {
        this.notificationService.showToast('error', 'Erro: Não foi possível autenticar com Apple.');
      });
  }

  signInWithGoogle(): void {
    this.googleAuthService.signIn()
      .then(token => {
        const data = { provider: 'google', token: token };
        this.authService.signIn(data).subscribe(
          res => {
            if (res.status === 200) {
              localStorage.setItem('jwtToken', res.json.token);
              this.notificationService.showToast('success', 'Sucesso: Login efetuado com sucesso.');
              setTimeout(() => this.router.navigate(['/home']), 3000);
            } else {
              this.notificationService.showToast('error', 'Erro: Problema no login com Google.');
            }
          },
          err => {
            this.notificationService.showToast('error', 'Erro: Algo correu mal no login com Google.');
          }
        );
      })
      .catch(err => {
        this.notificationService.showToast('error', 'Erro: Não foi possível autenticar com Google.');
      });
  }

  signInWithFacebook(): void {
    this.facebookAuthService.signIn()
      .then(token => {
        const data = { provider: 'facebook', token: token };
        this.authService.signIn(data).subscribe(
          res => {
            if (res.status === 200) {
              localStorage.setItem('jwtToken', res.json.token);
              this.notificationService.showToast('success', 'Sucesso: Login efetuado com sucesso.');
              setTimeout(() => this.router.navigate(['/home']), 3000);
            } else {
              this.notificationService.showToast('error', 'Erro: Problema no login com Facebook.');
            }
          },
          err => {
            this.notificationService.showToast('error', 'Erro: Algo correu mal no login com Facebook.');
          }
        );
      })
      .catch(err => {
        this.notificationService.showToast('error', 'Erro: Não foi possível autenticar com Facebook.');
      });
  }
}
