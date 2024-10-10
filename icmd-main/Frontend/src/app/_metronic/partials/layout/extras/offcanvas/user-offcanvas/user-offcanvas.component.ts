import { Component, OnInit } from '@angular/core';
import { LayoutService } from '../../../../../core';
import { AppConfig } from 'src/app/app.config';
import { LoggedInUser } from '@m/auth';
import { AuthService } from '@module/auth';

@Component({
  selector: 'app-user-offcanvas',
  templateUrl: './user-offcanvas.component.html',
  styleUrls: ['./user-offcanvas.component.scss'],
})
export class UserOffcanvasComponent implements OnInit {
  extrasUserOffcanvasDirection = 'offcanvas-right';
  user$: LoggedInUser;

  constructor(private layout: LayoutService,private auth: AuthService, private appConfig: AppConfig) {}

  ngOnInit(): void {
    this.extrasUserOffcanvasDirection = `offcanvas-${this.layout.getProp(
      'extras.user.offcanvas.direction'
    )}`;
    this.user$ = this.appConfig.getCurrentUser(); 
    //this.auth.currentUserSubject.asObservable();
  }

  logout() {
    this.auth.logout().subscribe();
  }
}
