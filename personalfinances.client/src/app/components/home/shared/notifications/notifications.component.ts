import { Component, OnInit } from '@angular/core';
import { NotificationService, Toast } from './notification.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  notifications: Toast[] = [];

  constructor(private notificationService: NotificationService) { }

  ngOnInit(): void {
    this.notificationService.notifications$.subscribe(data => {
      this.notifications = data;
    });
  }

  removeNotification(id: number): void {
    this.notificationService.removeNotification(id);
  }
}
