import { NgModule } from "@angular/core";
import { UserAddEditDialogComponent } from "./user-add-edit-dialog";
import { UserChangePasswordDialogComponent } from "./user-change-password-dialog";

@NgModule({
    imports: [UserAddEditDialogComponent, UserChangePasswordDialogComponent],
    exports: [],
    declarations: [],
    providers: [],
})
export class UserDialogsModule { }