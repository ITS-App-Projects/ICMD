import { Component, ViewChild, Inject } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { Subject } from "rxjs";
import { ColumnTemplateDialogInputDataModel, ColumnTemplateDialogOutputDataModel, CreateMetaDataModel } from ".";
import { ColumnTemplateService } from "src/app/service/column-selector";
import { takeUntil } from "rxjs/operators";
import { ToastrService } from "ngx-toastr";
import { AddColumnTemplateComponent } from "@c/column-selector/add-column-template";

@Component({
    standalone: true,
    selector: "app-column-template-dialog",
    templateUrl: "./add-column-template-dialog.component.html",
    providers: [
    ],
    imports: [
        MatIconModule,
        MatButtonModule,
        AddColumnTemplateComponent,
        MatDialogModule
    ],
})
export class ColumnTemplateDialogComponent {
    @ViewChild(AddColumnTemplateComponent) columnTemplate: AddColumnTemplateComponent;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialogRef: MatDialogRef<
            ColumnTemplateDialogComponent,
            ColumnTemplateDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: ColumnTemplateDialogInputDataModel,
        private _columnTemplateService: ColumnTemplateService,
        private _toastr: ToastrService,
    ) { }

    ngOnInit(): void { }

    ngAfterViewInit(): void {
        this.columnTemplate.templateList = this._inputData.userDefinedTemplates;
    }

    protected saveTemplate(name: string) {
        const model: CreateMetaDataModel = {
            templateName: name,
            value: this._inputData.currentSelectedColumns
        }
        this._columnTemplateService.createColumnTemplate(model).pipe(takeUntil(this._destroy$)).subscribe((res) => {
            if (res && res.isSucceeded) {
                this._toastr.success(res.message);
                this._dialogRef.close({ success: true, id: res.data?.id });
            } else {
                this._toastr.error(res.message);
            }
        })
    }

    protected deleteTemplate(id: string) {
        this._columnTemplateService.deleteColumnTemplate(id).pipe(takeUntil(this._destroy$)).subscribe((res) => {
            if (res && res.isSucceeded) {
                this._toastr.success(res.message);
                this._dialogRef.close({ success: true, id: null });
            } else {
                this._toastr.error(res.message);
            }
        })
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false, id: null });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}