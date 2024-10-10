import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class ProgressBarService {
    public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject(false);

    constructor() { }

    get isLoading(): Observable<boolean> {
        return this.isLoading$.asObservable();
    }
}
