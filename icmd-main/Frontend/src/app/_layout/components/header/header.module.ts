import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { HeaderComponent } from "./header.component";
import { HeaderMenuDynamicComponent } from "./header-menu-dynamic/header-menu-dynamic.component";
import { HeaderMenuComponent } from "./header-menu/header-menu.component";
import { NgbProgressbarModule } from "@ng-bootstrap/ng-bootstrap";
import { InlineSVGModule } from "ng-inline-svg-2";
import { TopbarComponent } from "../topbar/topbar.component";
import { TopbarModule } from "../topbar/topbar.module";
import { MatProgressBarModule } from "@angular/material/progress-bar";


@NgModule({
    declarations: [
        HeaderComponent,
        HeaderMenuComponent,
        HeaderMenuDynamicComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        NgbProgressbarModule,
        InlineSVGModule,
        TopbarModule,
        MatProgressBarModule
        
    ],
    exports     : [
        HeaderComponent
    ]
})
export class HeaderModule {
};