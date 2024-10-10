import { NgModule, Optional, SkipSelf } from "@angular/core";
import { AuthCoreModule } from "./auth/auth-core,module";

@NgModule({
    imports: [AuthCoreModule],
})
export class ICMDCoreModule {
    /**
     * Constructor
     */
    constructor(@Optional() @SkipSelf() parentModule?: ICMDCoreModule) {
        // Do not allow multiple injections
        if (parentModule) {
            throw new Error(
                "ICMDCoreModule has already been loaded. Import this module in the AppModule only."
            );
        }
    }
}