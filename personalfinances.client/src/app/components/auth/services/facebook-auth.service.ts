import { Injectable } from '@angular/core';
import { BaseAuthService } from './base-auth.service';

declare const FB: any;

@Injectable({
  providedIn: 'root'
})
export class FacebookAuthService extends BaseAuthService {
  private appId: string = 'YOUR_FACEBOOK_APP_ID';
  private fbInitPromise: Promise<void>;

  constructor() {
    super();
    // Inicia a inicialização do SDK e guarda a Promise
    this.fbInitPromise = this.initFacebookSDK();
  }

  /**
   * Inicializa o SDK do Facebook e retorna uma Promise que resolve quando o SDK estiver pronto.
   */
  private initFacebookSDK(): Promise<void> {
    return new Promise((resolve, reject) => {
      // Define a função fbAsyncInit que será chamada assim que o SDK carregar
      (window as any).fbAsyncInit = () => {
        FB.init({
          appId: this.appId,
          cookie: true,
          xfbml: false,
          version: 'v12.0'
        });
        resolve();
      };

      // Verifica se o script já foi inserido
      if (!document.getElementById('facebook-jssdk')) {
        const js = document.createElement('script');
        js.id = 'facebook-jssdk';
        js.src = 'https://connect.facebook.net/en_US/sdk.js';
        js.onerror = () => reject('Erro ao carregar o SDK do Facebook.');
        document.body.appendChild(js);
      } else {
        setTimeout(() => {
          if (typeof FB !== 'undefined') {
            resolve();
          } else {
            reject('FB não está definido mesmo após o script carregado.');
          }
        }, 1000);
      }
    });
  }

  /**
   * Efetua o login com o Facebook após garantir que o SDK está inicializado.
   */
  signIn(): Promise<string> {
    return new Promise((resolve, reject) => {
      this.fbInitPromise.then(() => {
        FB.login((response: any) => {
          if (response.authResponse) {
            // Retorna o token de acesso
            resolve(response.authResponse.accessToken);
          } else {
            reject('Erro na autenticação com o Facebook');
          }
        }, { scope: 'public_profile,email' });
      }).catch(reject);
    });
  }

  /**
   * Efetua o logout chamando FB.logout() após garantir que o SDK está inicializado.
   */
  signOut(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.fbInitPromise.then(() => {
        FB.logout((response: any) => {
          resolve();
        });
      }).catch(reject);
    });
  }
}
