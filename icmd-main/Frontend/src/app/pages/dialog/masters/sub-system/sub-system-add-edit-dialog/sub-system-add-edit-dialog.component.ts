import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { CommonDialogOutputDataModel } from "@m/common";
import { Subject } from "rxjs";
import { SubSystemService } from "src/app/service/sub-system";
import { SubSystemDialogInputDataModel } from "./sub-system-add-edit-dialog.model";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { CreateOrEditSubSystemFormComponent } from "@c/masters/sub-system/create-edit-sub-system-form";
import { takeUntil } from "rxjs/operators";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
    standalone: true,
    selector: "app-sub-system-dialog",
    templateUrl: "./sub-system-add-edit-dialog.component.html",
    providers: [
        SubSystemService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditSubSystemFormComponent,
        MatDialogModule
    ],
})
export class SubSystemAddEditDialogComponent {
    @ViewChild(CreateOrEditSubSystemFormComponent) subSystemForm: CreateOrEditSubSystemFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            SubSystemAddEditDialogComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: SubSystemDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _subSystemService: SubSystemService
    ) { }

    ngAfterViewInit(): void {
        this.subSystemForm.systemData = this._inputData.systems;
        if (this._inputData.id != null) {
            this._subSystemService.getSubSystemInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.subSystemForm.value =
                    {
                        id: res?.id,
                        number: res?.number,
                        description: res?.description,
                        systemId: res?.systemId,
                    };
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveSubSystemInfo(): void {
        const subSystemInfo = this.subSystemForm.value;
        if (subSystemInfo === null || subSystemInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._subSystemService.createEditSubSystem(subSystemInfo).subscribe(
            (res) => {
                if (res && res.isSucceeded) {
                    this._toastr.success(res.message);
                    this._dialogRef.close({ success: true });
                } else {
                    this.isLoading = !this.isLoading;
                    this._toastr.error(res.message);
                }
            },
            (errorRes: HttpErrorResponse) => {
                this.isLoading = !this.isLoading;
                if (errorRes?.error?.message) {
                    this._toastr.error(errorRes?.error?.message);
                }
            }
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}