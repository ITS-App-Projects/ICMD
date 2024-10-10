import { CommonModule } from "@angular/common";
import { Component, ViewChild, ChangeDetectorRef, Inject } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { ListColumnSelectorComponent } from "@c/column-selector/list-column-selector";
import { AddMemoryCacheModel, CommonColumnSelectorDialogInputDataModel, CommonColumnSelectorDialogOutputDataModel } from "@m/common";
import { instrumentListTableColumns } from "@u/constants";
import { listColumnMemoryCacheKey } from "@u/default";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { ColumnTemplateDialogsService, ColumnTemplateService } from "src/app/service/column-selector";
import { CommonService, DialogsService } from "src/app/service/common";

@Component({
    standalone: true,
    selector: "app-column-selector-dialog",
    templateUrl: "./column-selector-dialog.component.html",
    imports: [
        CommonModule,
        MatIconModule,
        MatButtonModule,
        ListColumnSelectorComponent,
        MatDialogModule
    ],
    providers: [
        DialogsService,
        CommonService,
        ColumnTemplateDialogsService
    ],
})
export class ColumnSelectorDialogComponent {
    @ViewChild(ListColumnSelectorComponent) columnSelectorList: ListColumnSelectorComponent;
    isTemplateShow: boolean = false;
    protected isLoading: boolean = false;
    protected instrumentListColumns: {
        key: string;
        label: string;
    }[] = [];
    private listName: string;
    private _destroy$ = new Subject<void>();

    constructor(
        private _dialog: DialogsService,
        private _dialogRef: MatDialogRef<
            ColumnSelectorDialogComponent,
            CommonColumnSelectorDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA) protected _inputData: CommonColumnSelectorDialogInputDataModel,
        private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef,
        private _commonService: CommonService,
        private _columnTemplateService: ColumnTemplateService,
        private _columnTemplateDialogService: ColumnTemplateDialogsService
    ) { }

    ngOnInit(): void { }

    ngAfterViewInit(): void {
        this.instrumentListColumns = this._inputData.itemList;
        this.listName = this._inputData.listName;
        this.columnSelectorList.isTemplateShow = (this.listName == listColumnMemoryCacheKey.instrumentColumn);
        this._cdr.detectChanges();

        this._commonService.getMemoryCacheItem(this.listName)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res: string[]) => {
                const selectedColumn = res;
                if (selectedColumn.length > 0) {
                    this.columnSelectorList.columnCheckBoxes.forEach(checkbox => {
                        if (selectedColumn.includes(checkbox.value))
                            checkbox.checked = true;
                        else
                            checkbox.checked = false;
                    });

                    this.columnSelectorList.defaultSelectedColumns = this.columnSelectorList.columnCheckBoxes.filter(x => x.checked).map(x => x.value);
                } else
                    this.selectDeselectAll(true);

                this.getTemplateList();
                this._cdr.detectChanges();
            });


        this.columnSelectorList.openColumnTemplate.pipe(takeUntil(this._destroy$)).subscribe((res) => {
            this.openTemplateDialog();
        });
    }

    protected saveSelectedColumn() {
        const selectedCheckboxes: string[] = this.columnSelectorList.columnCheckBoxes
            .filter(checkbox => checkbox.checked)
            .map(checkbox => checkbox.value);

        const model: AddMemoryCacheModel = {
            key: this.listName,
            value: selectedCheckboxes
        }

        this._commonService.setMemoryCacheItem(model).pipe(takeUntil(this._destroy$)).subscribe((res) => {
            if (selectedCheckboxes.length > 0)
                selectedCheckboxes.push(instrumentListTableColumns[instrumentListTableColumns.length - 1].key);

            this._dialogRef.close({ selectedColumns: selectedCheckboxes, success: true });
        });
    }

    protected selectDeselectAll(isSelectAll: boolean = false) {
        this.columnSelectorList.columnCheckBoxes.forEach(checkbox => checkbox.checked = isSelectAll);
    }

    protected cancel(): void {
        this._dialogRef.close({ selectedColumns: [], success: false });
    }

    private async openTemplateDialog() {
        const currentSelectedColumn = this.columnSelectorList.columnCheckBoxes.filter(x => x.checked).map(x => x.value).join(", ");
        let response = await this._columnTemplateDialogService.openColumnTemplateDialog(this.columnSelectorList.templateListModel.filter(x => !x.isDefault), currentSelectedColumn);
        this.getTemplateList(response.id);
    }

    private getTemplateList(newItemId: string = null) {
        this._columnTemplateService.getColumnTemplate().pipe(takeUntil(this._destroy$)).subscribe((res) => {
            this.columnSelectorList.templateListModel = res;
            if (newItemId)
                this.columnSelectorList.templateSelectValue = newItemId;
            this._cdr.detectChanges();
        });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}