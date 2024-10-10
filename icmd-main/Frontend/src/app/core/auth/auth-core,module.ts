import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { ErrorInterceptor, LoadingInterceptor } from "src/app/interceptors";
import { IdleUserService } from "src/app/service/common";

@NgModule({
    imports: [HttpClientModule],
    providers: [
        IdleUserService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ErrorInterceptor,
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: LoadingInterceptor,
            multi: true,
        },
    ],
})
export class AuthCoreModule { }