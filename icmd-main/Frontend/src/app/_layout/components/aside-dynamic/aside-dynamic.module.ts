import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { AsideDynamicComponent } from "./aside-dynamic.component";
import { InlineSVGModule } from "ng-inline-svg-2";

@NgModule({
    declarations: [
        AsideDynamicComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        InlineSVGModule
    ],
    exports     : [
        AsideDynamicComponent
    ]
})
export class AsideDynamicModule {
}