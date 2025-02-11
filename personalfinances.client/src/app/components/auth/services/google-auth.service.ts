import { Injectable } from '@angular/core';
import { BaseAuthService } from './base-auth.service';

declare const gapi: any;

@Injectable({
  providedIn: 'root'
})
export class GoogleAuthService extends BaseAuthService {

  private auth2: any;
  private clientId: string = '166483281510-0lqqnrmeourbfnc525n9k2h8qeldu5n6.apps.googleusercontent.com';

  constructor() {
    super();
    this.loadGapiScript()
      .then(() => this.initGoogleAuth())
      .catch(error => console.error(error));
  }

  private loadGapiScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (document.getElementById('gapi-script')) {
        // Se o script já estiver carregado
        resolve();
      } else {
        const script = document.createElement('script');
        script.id = 'gapi-script';
        script.src = 'https://apis.google.com/js/api:client.js';
        script.onload = () => resolve();
        script.onerror = () => reject('Erro ao carregar o gapi script.');
        document.body.appendChild(script);
      }
    });
  }

  private initGoogleAuth(): Promise<any> {
    return new Promise((resolve, reject) => {
      if (!gapi) {
        reject('gapi não está definido.');
      }
      gapi.load('auth2', () => {
        this.auth2 = gapi.auth2.init({
          client_id: this.clientId,
          cookiepolicy: 'single_host_origin'
        });
        resolve(this.auth2);
      });
    });
  }

  signIn(): Promise<string> {
    if (!this.auth2) {
      return this.initGoogleAuth().then(() => this.signIn());
    }
    return this.auth2.signIn().then((googleUser: any) => {
      const idToken = googleUser.getAuthResponse().id_token;
      return idToken;
    });
  }

  signOut(): Promise<void> {
    if (!this.auth2) {
      return Promise.resolve();
    }
    return this.auth2.signOut();
  }
}
