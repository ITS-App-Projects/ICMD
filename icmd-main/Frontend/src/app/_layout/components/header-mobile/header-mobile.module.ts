import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { HeaderMobileComponent } from "./header-mobile.component";
import { InlineSVGModule } from "ng-inline-svg-2";

@NgModule({
    declarations: [
        HeaderMobileComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        InlineSVGModule
    ],
    exports     : [
        HeaderMobileComponent
    ]
})
export class HeaderMobileModule {
}