import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  isSidebarClosed = true;
  isDarkMode = false;

  @Output() sidebarToggled = new EventEmitter<boolean>();

  // toggleSidebar() {
  //   this.isSidebarClosed = !this.isSidebarClosed;
  //   this.sidebarToggled.emit(this.isSidebarClosed);
  // }

  onSidebarToggle(closed: boolean) {
    this.isSidebarClosed = closed;
  }

}
