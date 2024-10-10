import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { TopbarComponent } from "./topbar.component";
import { CoreModule } from "src/app/_metronic/core";
import { ExtrasModule } from "src/app/_metronic/partials/layout/extras/extras.module";
import { NgbDropdownModule } from "@ng-bootstrap/ng-bootstrap";
import { InlineSVGModule } from "ng-inline-svg-2";
import { TranslationModule } from "src/app/modules/i18n/translation.module";
import { LanguageSelectorComponent } from "./language-selector/language-selector.component";
import { MatSelectModule } from "@angular/material/select";
import { ProjectService } from "src/app/service/manage-projects";
import { UserService } from "src/app/service/manage-user";

@NgModule({
    declarations: [
        TopbarComponent,
        LanguageSelectorComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        CoreModule,
        ExtrasModule,
        NgbDropdownModule,
        InlineSVGModule,
        TranslationModule,
        MatSelectModule
    ],
    exports: [
        TopbarComponent
    ],
    providers: [ProjectService, UserService]
})
export class TopbarModule {
}