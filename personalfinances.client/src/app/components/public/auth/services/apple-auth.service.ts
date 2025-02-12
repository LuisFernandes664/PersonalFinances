import { Injectable } from '@angular/core';
import { BaseAuthService } from './base-auth.service';

declare const AppleID: any;

@Injectable({
  providedIn: 'root'
})
export class AppleAuthService extends BaseAuthService {

  constructor() {
    super();
    if ((window as any).AppleID) {
      AppleID.auth.init({
        clientId: 'com.tuaempresa.tuaplicacao',       // Substitui pelo teu Client ID
        scope: 'name email',
        redirectURI: 'https://tuadominio.com/callback', // Substitui pela tua URL de callback
        state: 'random_state_string',
        usePopup: true
      });
    } else {
      console.error('AppleID não está disponível.');
    }
  }

  signIn(): Promise<string> {
    if ((window as any).AppleID) {
      return AppleID.auth.signIn().then((response: any) => {
        // Retorna o token de autorização (ajusta conforme a resposta)
        return response.authorization.id_token;
      });
    } else {
      return Promise.reject('AppleID não está disponível.');
    }
  }

  signOut(): Promise<void> {
    // Se for necessário implementar o logout, coloca a lógica aqui
    return Promise.resolve();
  }
}
