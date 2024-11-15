import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BulkDeleteService {

  constructor() { }

  private showCheckboxesSource = new BehaviorSubject<boolean>(false);
  showCheckboxes$ = this.showCheckboxesSource.asObservable();

  toggleCheckboxes(show: boolean) {
    this.showCheckboxesSource.next(show);
  }
}
