import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { FooterComponent } from "./footer.component";

@NgModule({
    declarations: [
        FooterComponent
    ],
    imports: [
        RouterModule,
        CommonModule
    ],
    exports     : [
        FooterComponent
    ]
})
export class FooterModule {
}