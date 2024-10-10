import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatTreeModule } from "@angular/material/tree";
import { ViewDeviceInfoDtoModel } from "./view-device-detail.model";
import { Router } from "@angular/router";
import { AppRoute } from "@u/app.route";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-view-device-detail",
    templateUrl: "./view-device-detail.component.html",
    imports: [
        CommonModule,
        MatTreeModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        NgScrollbarModule
    ],
    providers: [],
})
export class ViewDeviceDetailComponent {
    deviceData: ViewDeviceInfoDtoModel = null;

    constructor(private _router: Router) {

    }

    protected manageInstruments(): void {
        this._router.navigate([AppRoute.manageHierarchy]);
    }
}