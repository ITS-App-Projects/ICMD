import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { environment } from './environments/environment';
import { ICMDModule } from './app/icmd.module';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(ICMDModule)
  .catch(err => console.error(err));
