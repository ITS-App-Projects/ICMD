import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { ScriptsInitComponent } from "./scripts-init.component";

@NgModule({
    declarations: [
        ScriptsInitComponent
    ],
    imports: [
        RouterModule,
        CommonModule
    ],
    exports     : [
        ScriptsInitComponent
    ]
})
export class ScriptsInitModule {
}