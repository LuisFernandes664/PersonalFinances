import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { AuthService } from '../components/public/auth/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private loadingService: LoadingService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loadingService.show();

    const token = this.authService.getToken();

    // Verifica se a requisição contém FormData (upload de arquivo)
    const isFormData = req.body instanceof FormData;

    let authReq = req;

    if (token) {
      // Se for FormData, apenas adicione o token de autenticação, sem alterar o Content-Type
      if (isFormData) {
        authReq = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
      } else {
        // Para outras requisições, mantenha o comportamento atual
        authReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
      }
    }

    return next.handle(authReq).pipe(
      finalize(() => this.loadingService.hide())
    );
  }
}
