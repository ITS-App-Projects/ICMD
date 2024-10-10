import { ChangeDetectorRef, Component, Input, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { FormGroupG, getGroup } from "@u/forms";
import { CreateOrEditProjectDtoModel, UserAuthorizationDtoModel } from "./create-edit-project-form.model";
import { FormArray, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatIconModule } from "@angular/material/icon";
import { AuthorizationType } from "@e/common";
import { UserDetailsModel } from "@m/common";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelect, MatSelectModule } from "@angular/material/select";
import { ToastrService } from "ngx-toastr";
import { CommonModule } from "@angular/common";
import { BehaviorSubject, Observable } from "rxjs";
import { map, startWith } from "rxjs/operators";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-create-edit-project-form",
    templateUrl: "./create-edit-project-form.component.html",
    imports: [
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatAutocompleteModule,
        ReactiveFormsModule,
        CommonModule,
        MatButtonModule, MatIconModule, MatSelectModule, NgScrollbarModule],
    providers: [],
})
export class CreateOrEditProjectFormComponent extends FormBaseComponent<CreateOrEditProjectDtoModel> {
    userDetails$: BehaviorSubject<UserDetailsModel[]> = new BehaviorSubject([]);
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    protected authorizationTypes = AuthorizationType;
    protected authorization: string[] = [];
    constructor(private _cdr: ChangeDetectorRef, private _toastr: ToastrService) {
        super(
            getGroup<CreateOrEditProjectDtoModel>(
                {
                    id: { v: "00000000-0000-0000-0000-000000000000" },
                    name: { vldtr: [Validators.required] },
                    description: {},
                    userAuthorizations: new FormArray<FormGroupG<UserAuthorizationDtoModel>>([])
                }
            )
        );
        this.authorization = Object.keys(this.authorizationTypes);
    }

    ngAfterViewInit() { }

    @Input() public set items(val: CreateOrEditProjectDtoModel) {
        if (val) {
            super.value = {
                id: val?.id,
                name: val?.name,
                description: val?.description,
                userAuthorizations: []
            }
            val.userAuthorizations?.map((res) => {
                this.addUser(res);
            })
            this._cdr.detectChanges();
        }
    }

    protected getGroup(index: number): FormGroupG<UserAuthorizationDtoModel> {
        return this.array('userAuthorizations').controls[index] as FormGroupG<UserAuthorizationDtoModel>;
    }

    protected addUser(userAuthorizationInfo: UserAuthorizationDtoModel = null): void {
        const userList = this.array("userAuthorizations");
        userList.push(
            getGroup<UserAuthorizationDtoModel>({
                id: { v: userAuthorizationInfo?.id ?? this.emptyGuid },
                userId: { v: userAuthorizationInfo?.userId ?? null, vldtr: [Validators.required] },
                authorization: { v: userAuthorizationInfo?.authorization ?? this.authorizationTypes.ReadOnly },
            })
        );
    }

    protected onUserChange(id: string, currentIndex: number): void {
        const userList = this.array("userAuthorizations").value;
        const foundUser = userList.find((record, index) => {
            return record.userId === id && index !== currentIndex;
        });

        if (foundUser) {
            this._toastr.error("User already selected. Please select a different one.");
            this.getGroup(currentIndex).controls['userId'].setValue(null);
        }
    }

    protected deleteUser(index: number): void {
        this.array("userAuthorizations").removeAt(index);
    }

}