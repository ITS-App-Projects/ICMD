import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class BulkDeleteService {

  private checkboxesMap = new Map<string, BehaviorSubject<boolean>>();

  constructor() { }


  getCheckboxState(context: string): Observable<boolean> {
    if (!this.checkboxesMap.has(context)) {
      this.checkboxesMap.set(context, new BehaviorSubject<boolean>(false));
    }
    return this.checkboxesMap.get(context)!.asObservable();
  }

  toggleBulkDelete(context: string, show: boolean) {
    if (!this.checkboxesMap.has(context)) {
      this.checkboxesMap.set(context, new BehaviorSubject<boolean>(show));
    }
    this.checkboxesMap.get(context)!.next(show);
  }

  cancelBulkDelete() {
    this.checkboxesMap.forEach((subject) => {
      subject.next(false); 
    });
  }

}
