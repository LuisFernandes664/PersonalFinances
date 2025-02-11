// src/app/services/notification.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Toast {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationsSubject = new BehaviorSubject<Toast[]>([]);
  notifications$: Observable<Toast[]> = this.notificationsSubject.asObservable();

  addNotification(toast: Toast): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([...current, toast]);
    setTimeout(() => this.removeNotification(toast.id), 5000);
  }

  removeNotification(id: number): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next(current.filter(t => t.id !== id));
  }

  showToast(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    const toast: Toast = { id: Date.now(), type, message };
    this.addNotification(toast);
  }
}
