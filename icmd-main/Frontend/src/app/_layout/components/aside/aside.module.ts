import { NgModule } from "@angular/core";
import { AsideComponent } from "./aside.component";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { InlineSVGModule } from "ng-inline-svg-2";
import { TranslationModule } from "src/app/modules/i18n/translation.module";
import { MatIconModule } from "@angular/material/icon";
import { ProjectService } from "src/app/service/manage-projects";

@NgModule({
    declarations: [
        AsideComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        InlineSVGModule,
        TranslationModule,
        MatIconModule
    ],
    exports: [
        AsideComponent
    ],
    providers: [ProjectService]
})
export class AsideModule {
}