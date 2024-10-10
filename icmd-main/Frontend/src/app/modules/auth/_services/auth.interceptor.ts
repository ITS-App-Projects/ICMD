import {
  HttpHandlerFn,
  HttpRequest,
} from "@angular/common/http";
import { inject } from "@angular/core";
import { AppConfig } from "src/app/app.config";

// @Injectable()
export function AuthInterceptor(request: HttpRequest<unknown>,
  next: HttpHandlerFn) {
  /**
   * Constructor
   */
  // public appConfig: AppConfig;
  // constructor(public router: Router) {
  //   this.appConfig = new AppConfig();
  // }

  const token = inject(AppConfig).getToken();

  // intercept(
  //   request: HttpRequest<any>,
  //   next: HttpHandler
  // ): Observable<HttpEvent<any>> {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(request);
  // }
}
