import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ExtrasModule } from "../_metronic/partials/layout/extras/extras.module";
import { SubheaderModule } from "../_metronic/partials/layout/subheader/subheader.module";
import { LayoutComponent } from "./layout.component";
import { AsideModule } from "./components/aside/aside.module";
import { AsideDynamicModule } from "./components/aside-dynamic/aside-dynamic.module";
import { HeaderMobileModule } from "./components/header-mobile/header-mobile.module";
import { FooterModule } from "./components/footer/footer.module";
import { HeaderModule } from "./components/header/header.module";
import { TopbarModule } from "./components/topbar/topbar.module";
import { ScriptsInitModule } from "./init/scipts-init/scripts-init.module";
import { RouterModule } from "@angular/router";
import { PagesRoutingModule } from "../pages/pages-routing.module";

const layoutModules = [
    AsideModule,
    AsideDynamicModule,
    FooterModule,
    HeaderModule,
    HeaderMobileModule,
    TopbarModule,
    ScriptsInitModule
];

@NgModule({
  declarations: [
    LayoutComponent,
  ],
  imports: [
    RouterModule,
    CommonModule,
    SubheaderModule,
    ExtrasModule,
    PagesRoutingModule,
    ...layoutModules
  ],
  exports: [
    ...layoutModules
  ]
  
})
export class LayoutModule {}
