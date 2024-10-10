import { CommonModule } from "@angular/common";
import { AfterViewInit, ChangeDetectorRef, Component, Inject, Pipe, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { ProjectService } from "src/app/service/manage-projects";
import { ProjectDialogInputDataModel, ProjectDialogOutputDataModel } from "./project-add-edit-dialog.model";
import { ToastrService } from "ngx-toastr";
import { ProgressBarService } from "src/app/service/common";
import { CreateOrEditProjectFormComponent } from "@c/manage-projects/create-edit-project-form";
import { HttpErrorResponse } from "@angular/common/http";
import { UserService } from "src/app/service/manage-user";
import { takeUntil } from "rxjs/operators";

@Component({
    standalone: true,
    selector: "app-project-dialog",
    templateUrl: "./project-add-edit-dialog.component.html",
    providers: [
        ProjectService,
        UserService
    ],
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        CreateOrEditProjectFormComponent,
        MatDialogModule
    ],
})
export class ProjectAddEditDialogComponent implements AfterViewInit {
    @ViewChild(CreateOrEditProjectFormComponent) projectForm: CreateOrEditProjectFormComponent;
    emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    protected isLoading: boolean = false;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            ProjectAddEditDialogComponent,
            ProjectDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: ProjectDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        private _projectService: ProjectService,
        protected progressBarService: ProgressBarService,
        private _userService: UserService
    ) { }

    ngAfterViewInit(): void {
        this._userService.getAllUsersInfo()
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                this.projectForm.userDetails$.next(res);
            });

        if (this._inputData.id != null) {
            this._projectService.getProjectInfo(this._inputData.id)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.projectForm.items = res;
                });
        }
        this._cdr.detectChanges();

    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    protected saveProjectInfo(): void {
        const projectInfo = this.projectForm.value;
        if (projectInfo === null || projectInfo == undefined) {
            return;
        }

        this.isLoading = !this.isLoading;
        this._projectService.createEditProject(projectInfo).subscribe(
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