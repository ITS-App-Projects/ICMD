import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Subject, combineLatest } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { HttpErrorResponse } from "@angular/common/http";
import { takeUntil } from "rxjs/operators";
import { CommonDialogInputDataModel, CommonDialogOutputDataModel } from "@m/common";
import { TagService } from "src/app/service/tag";
import { DocumentTypeService } from "src/app/service/documentType";

import { CreateOrEditRoomFormComponent } from "@c/masters/room/create-edit-room-form/create-edit-room-form.component";
import { RoomService } from "src/app/service/room";

@Component({
    standalone: true,
    selector: "app-room-dialog",
    templateUrl: "./room-add-edit-dialog.component.html",
    providers: [
        RoomService,
        TagService,
        DocumentTypeService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditRoomFormComponent,
        MatDialogModule
    ],
})
export class RoomAddEditDialogComponent {
    @ViewChild(CreateOrEditRoomFormComponent) roomForm: CreateOrEditRoomFormComponent;
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
        CreateOrEditRoomFormComponent,
            CommonDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        protected progressBarService: ProgressBarService,
        private _roomService: RoomService,
        private _tagService: TagService,
        private _documentTypeService: DocumentTypeService
    ) { }

    ngAfterViewInit(): void {
        if (this._inputData.projectId) {
            this.roomForm.field('projectId').setValue(this._inputData.projectId);
        }

        if (this._inputData.id != null) {
            this._roomService.getRoomInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.roomForm.value = res;
                });
        }
        this._cdr.detectChanges();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveRoomInfo(): void {
        console.log(this.roomForm.value);
        const roomInfo = this.roomForm.value;
        if (roomInfo === null || roomInfo == undefined) {
            return;
        }

        
        this.isLoading = !this.isLoading;
        this._roomService.createEditRoom(roomInfo).subscribe(
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