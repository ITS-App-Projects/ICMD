import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService, UserModel } from '../../../auth';
import { LoggedInUserDtoModel } from 'src/app/models/auth/login-response-model';

@Component({
  selector: 'app-profile-card',
  templateUrl: './profile-card.component.html',
  styleUrls: ['./profile-card.component.scss']
})
export class ProfileCardComponent {
  user$: Observable<LoggedInUserDtoModel>;
  constructor(public userService: AuthService) {
    this.user$ = this.userService.currentUserSubject.asObservable();
  }
}
