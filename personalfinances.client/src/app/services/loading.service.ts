import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  // Subject para controlar o estado do loading
  private loadingSubject = new BehaviorSubject<boolean>(false);

  // Observable público para expor o estado do loading
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  constructor() {}

  /**
   * Ativa o estado de loading.
   */
  show(): void {
    if (!this.loadingSubject.value) {
      this.loadingSubject.next(true);
    }
  }

  /**
   * Desativa o estado de loading.
   */
  hide(): void {
    if (this.loadingSubject.value) {
      this.loadingSubject.next(false);
    }
  }

  /**
   * Retorna o estado atual do loading.
   * @returns O estado atual do loading (true ou false).
   */
  get isLoading(): boolean {
    return this.loadingSubject.value;
  }
}
