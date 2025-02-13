import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { NotificationService } from '../../shared/notifications/notification.service';
import { APIResponse } from '../../../models/api-response.model';
import { DecodedToken } from '../../../models/decodedToken.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private endPointSignIn = `${environment.apiUrl}/auths/signin/`;
  private endPointSignUp = `${environment.apiUrl}/auths/RegisterUser/`;
  private readonly TOKEN_KEY = 'authToken';

  constructor(private http: HttpClient, private notificationService: NotificationService, private router: Router) {}

  signIn(data: { email: string; password: string }): Observable<APIResponse<any>>;
  signIn(data: any): Observable<any>;

  signIn(data: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(this.endPointSignIn, data).pipe(
      tap(response => {
        if (response.success) {
          this.setToken(response.data.token);
          this.router.navigate(['/home']);
          this.notificationService.showToast('success', 'Sucesso: Login efetuado com sucesso.');
        }
      }),
      map(response => {
        if (response.success) {
          return response;
        }
        throw new Error(response.message || 'Erro desconhecido no login.');
      }),
      catchError(err => {
        console.log(err);
        const errorMessage = err.error || err.message || 'Erro: Algo correu mal no login.';
        this.notificationService.showToast('error', errorMessage);
        return throwError(() => new Error(errorMessage));
      })
    );
  }


  signUp(data: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(this.endPointSignUp, data).pipe(
      tap(response => {
        if (response.success) {
          this.notificationService.showToast('success', 'Sucesso: Conta criada com sucesso.');
        }
      }),
      map(response => {
        if (response.success) {
          return response;
        }
        throw new Error(response.message || 'Erro desconhecido no registo.');
      }),
      catchError(err => {
        const errorMessage = err.error?.message || err.message || 'Erro: Algo correu mal no registo.';
        this.notificationService.showToast('error', errorMessage);
        return throwError(() => new Error(errorMessage));
      })
    );
  }



    /**
   * Obtém o token armazenado no localStorage.
   * @returns O token JWT ou null se não existir.
   */
    getToken(): string | null {
      const name = `${this.TOKEN_KEY}=`;
      const decodedCookie = decodeURIComponent(document.cookie);
      const cookiesArray = decodedCookie.split(';');

      for (let cookie of cookiesArray) {
        cookie = cookie.trim();
        if (cookie.indexOf(name) === 0) {
          return cookie.substring(name.length, cookie.length);
        }
      }

      return null;
    }

    /**
     * Armazena o token no localStorage.
     * @param token - O token JWT a ser armazenado.
     */
    setToken(token: string): void {
      if (!token) {
        throw new Error('O token não pode ser nulo ou vazio.');
      }
      const expires = new Date();
      expires.setTime(expires.getTime() + (7 * 24 * 60 * 60 * 1000)); // Expira em 7 dias
      document.cookie = `${this.TOKEN_KEY}=${token};expires=${expires.toUTCString()};path=/`;
    }

    /**
     * Remove o token do localStorage (logout).
     */
    removeToken(): void {
      document.cookie = `${this.TOKEN_KEY}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`;
    }

    /**
     * Verifica se o utilizador está autenticado.
     * @returns True se o token existir e não estiver expirado, caso contrário, False.
     */
    isAuthenticated(): boolean {
      const token = this.getToken();
      return !!token && !this.isTokenExpired(token);
    }

    /**
     * Decodifica o token JWT e retorna os dados do utilizador.
     * @returns O token decodificado ou um objeto vazio se o token for inválido.
     */
    getDecodedToken(): DecodedToken {
      const token = this.getToken();
      if (!token) {
        return {} as DecodedToken;
      }

      try {
        return jwtDecode<DecodedToken>(token);
      } catch (error) {
        console.error('Erro ao decodificar o token:', error);
        return {} as DecodedToken;
      }
    }

    /**
     * Verifica se o token está expirado.
     * @param token - O token JWT a ser verificado.
     * @returns True se o token estiver expirado, caso contrário, False.
     */
    isTokenExpired(token: string): boolean {
      try {
        const decoded = jwtDecode<{ exp?: number }>(token);
        if (decoded?.exp) {
          const currentTime = Math.floor(Date.now() / 1000);
          return decoded.exp < currentTime;
        }
        return true; // Se não houver exp, considera como expirado
      } catch (error) {
        console.error('Erro ao verificar expiração do token:', error);
        return true; // Em caso de erro, considera como expirado
      }
    }


}
