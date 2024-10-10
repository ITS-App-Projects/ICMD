import { NgModule } from "@angular/core";
import { BrowserModule, provideClientHydration } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HTTP_INTERCEPTORS, HttpClientModule, provideHttpClient, withInterceptors, withInterceptorsFromDi } from "@angular/common/http";
import { TranslateModule } from "@ngx-translate/core";
import { InlineSVGModule } from "ng-inline-svg-2";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ICMDRoutingModule } from "./icmd-routing.module";
import { AuthService } from "./modules/auth/_services/auth.service";
// Highlight JS
import { HighlightModule, HIGHLIGHT_OPTIONS } from "ngx-highlightjs";
import { SplashScreenModule } from "./_metronic/partials/layout/splash-screen/splash-screen.module";
// #fake-start#
import { RootComponent } from "./components/root/root.component";
import { AuthInterceptor } from "./modules/auth/_services/auth.interceptor";
import { AppConfig } from "./app.config";
import { CookieService } from "ngx-cookie-service";
import { AuthGuard } from "./modules/auth/_services/auth.guard";
import { FormDefaultsModule } from "./components/shared/forms/form-defaults.module";
import { ToastrModule } from "ngx-toastr";
import { ICMDCoreModule } from "./core/icmd-core.module";
import { CommonModule } from "@angular/common";
import { CommonFunctions } from "@u/helper";
import { NgbootstrapModule } from "@module/ngbootstrap/ngbootstrap.module";
import { MAT_PAGINATOR_DEFAULT_OPTIONS } from "@angular/material/paginator";
import { MAT_CHECKBOX_DEFAULT_OPTIONS } from "@angular/material/checkbox";
// #fake-end#

function appInitializer(authService: AuthService) {
  // return () => {
  //   return new Promise((resolve) => {
  //     authService.getUserByToken().subscribe().add(resolve);
  //   });
  // };
}

@NgModule({
  declarations: [RootComponent],
  imports: [
    CommonModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    HighlightModule,
    SplashScreenModule,
    ICMDCoreModule,
    ICMDRoutingModule,
    TranslateModule.forRoot(),
    FormDefaultsModule,
    //ClipboardModule,
    // #fake-start#
    // environment.isMockEnabled
    //   ? HttpClientInMemoryWebApiModule.forRoot(FakeAPIService, {
    //     passThruUnknownUrl: true,
    //     dataEncapsulation: false,
    //   })
    //   : [],
    // #fake-end#
    InlineSVGModule.forRoot(),
    NgbModule,
    NgbootstrapModule,
    ToastrModule.forRoot(),
  ],
  providers: [
    AppConfig,
    CommonFunctions,
    // AuthService,
    // {
    //     provide : HTTP_INTERCEPTORS,
    //     useClass: AuthInterceptor,
    //     multi   : true
    // },
    // {
    //   provide: APP_INITIALIZER,
    //   useFactory: appInitializer,
    //   multi: true,
    //   deps: [AuthService],
    // },
    AuthGuard,
    CookieService,
    AuthService,
    // {
    //   provide: HTTP_INTERCEPTORS,
    //   useClass: AuthInterceptor,
    //   multi: true,
    // },
    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([AuthInterceptor])
    ),
    {
      provide: HIGHLIGHT_OPTIONS,
      useValue: {
        coreLibraryLoader: () => import("highlight.js/lib/core"),
        languages: {
          xml: () => import("highlight.js/lib/languages/xml"),
          typescript: () => import("highlight.js/lib/languages/typescript"),
          scss: () => import("highlight.js/lib/languages/scss"),
          json: () => import("highlight.js/lib/languages/json"),
        },
      },
    },
    {provide: MAT_PAGINATOR_DEFAULT_OPTIONS, useValue: {formFieldAppearance: 'fill'}},
    {provide:MAT_CHECKBOX_DEFAULT_OPTIONS, useValue: {color: 'primary'}}
  ],
  bootstrap: [RootComponent],
})
export class ICMDModule { }
