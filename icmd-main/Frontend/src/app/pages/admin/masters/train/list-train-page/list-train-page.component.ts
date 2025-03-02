import {
  download,
  generateCsv,
  mkConfig
} from 'export-to-csv';
import { ToastrService } from 'ngx-toastr';
import {
  combineLatest,
  BehaviorSubject,
  Subject
} from 'rxjs';
import {
  take,
  takeUntil
} from 'rxjs/operators';
import { AppConfig } from 'src/app/app.config';
import { DialogsService } from 'src/app/service/common';
import {
  TrainDialogsService,
  TrainSearchHelperService,
  TrainService
} from 'src/app/service/train';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import {
  ListTrainTableComponent,
  TrainInfoDtoModel
} from '@c/masters/train/list-train-table';
import { TrainBulkDialogComponent } from '@c/shared/bulkDelete-dialog/project-master/train-master/train-bulk-dialog.component';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import { CustomFieldSearchModel } from '@m/common';
import { importTrainFileColumns } from '@u/constants';
import { ExcelHelper } from '@u/helper';
import { ImportPreviewDialogComponent } from '@c/shared/import-preview-dialog/import-preview-dialog.component';

@Component({
    standalone: true,
    selector: "app-list-train-page",
    templateUrl: "./list-train-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        MatDialogModule,
        ListTrainTableComponent,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        TrainService,
        TrainSearchHelperService,
        TrainDialogsService,
        DialogsService,
        ExcelHelper
    ]
})
export class ListTrainPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListTrainTableComponent) trainTable: ListTrainTableComponent;
    protected projectId: string = null;
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];
    private filterState: { [key: string]: any } = {};

    constructor(
        protected _trainSearchHelperService: TrainSearchHelperService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        private dialog: MatDialog,
        private _trainService: TrainService,
        private _trainDialogService: TrainDialogsService,
        protected appConfig: AppConfig,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getTrainData();
    }

    ngAfterViewInit(): void {
        this.trainTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._trainSearchHelperService.updateSortingChange(res);
        });

        this.trainTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._trainSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._trainSearchHelperService.updateFilterChange(filter);
        });

        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this._cd.detectChanges();
                this.tableColumnchanges();
                this.getTrainData();
            }
        });

        this.trainTable.columnsChanged.subscribe(() => {
            this.tableColumnchanges();
        });
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._trainSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this train?",
            "Confirm"
        );
        if (isOk) {
            this._trainService.deleteTrain($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getTrainData();
                    } else {
                        this._toastr.error(res.message);
                    }
                },
                (errorRes) => {
                    this._toastr.error(errorRes?.error?.message);
                }
            );
        }
    }

    // #region Delete Bulk
    protected async deleteBulkTrain(ids: string[]): Promise<void> {
        const dialogRef = this.dialog.open(TrainBulkDialogComponent, {
            width: '600px',
            data: ids,
        });

          dialogRef.afterClosed().subscribe((result: string[] | null) => {

            if (result) {
                this._trainService.deleteBulkTrain(result).pipe(takeUntil(this._destroy$)).subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                            this.getTrainData();
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes) => {
                        this._toastr.error(errorRes?.error?.message);
                    }
                );
            }
          });
    }

    protected async addEditTrainDialog(event: string = null): Promise<void> {
        await this._trainDialogService.openTrainDialog(event, this.projectId);
        this.getTrainData();
    }

    protected async bulkEditTrain(): Promise<void> {
        const fileName = "Edit_Train"

        this.defaultCustomFilter(true, this.columnFilterList);
        this._trainSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'id' : 'Id',
                    'train': 'Train',
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    protected exportData(): void {
        const fileName = 'Export_Trains';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._trainSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const columnMapping = {
                    'train': 'Train',
                };
                this._excelHelper.exportExcel(model.items ?? [], columnMapping, fileName);
            });
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.trainTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            const currentFilterValue = filterComponent.columnFilterModel$.value;
            if (currentFilterValue) {
                this.filterState[columnKey] = currentFilterValue;
            }
        });
        this.trainTable.columnFiltersList.forEach((filterComponent) => {
            const columnKey = filterComponent.fieldName;
            if (this.filterState[columnKey] !== undefined) {
                filterComponent.setFilter(this.filterState[columnKey]);
            }
        });

        combineLatest(this.trainTable.columnFiltersList.map(x => x.columnFilterModel$))
        .pipe(takeUntil(this._destroy$))
        .subscribe((res) => {
            if (res && res.length > 0) {
                this.columnFilterList = res.filter(x => x);
                this.defaultCustomFilter(false, this.columnFilterList);
            }
        });
    }

    private getTrainData(): void {
        if (this.projectId) {
            this.defaultCustomFilter();
            this._trainSearchHelperService
                .loadDataFromRequest()
                .pipe(takeUntil(this._destroy$))
                .subscribe((model) => { });
        }
    }

    private defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [
            { fieldName: "projectIds", fieldValue: this.projectId, searchType: SearchType.Contains, isColumnFilter: false },
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        ];
        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList);

        this.customFilters$.next(filters);
    }

    //#region Import Functionality
    protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_Train', columnHeaders: importTrainFileColumns, fieldSeparator: "," });
        const csv = generateCsv(csvConfig)([]);
        download(csvConfig)(csv);
    }

    protected onFileSelected(event: any): void {
        if (!event) return;

        const selectedFile = event.target.files[0] ?? null;
        if (!selectedFile) {
            this._toastr.error("Please select a file for import.");
            this.clearFileInput();
            return;
        }

        if (!this.projectId) {
            this._toastr.error("Please select a project.");
            this.clearFileInput();
            return;
        }

        this._trainService.validateImportTrain(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (!res || !res.isSucceeded) {
                        this._toastr.error("Validation failed. Please check your file.");
                        this.clearFileInput();
                        return;
                    } 

                    const dialogRef = this.dialog.open(ImportPreviewDialogComponent, {
                        width: '750px',
                        data: res.records
                    });

                    dialogRef.afterClosed().subscribe((confirmed) => {
                        if (confirmed) {
                            this.proceedImport(selectedFile);
                            this.getTrainData();
                        } else {
                            this.clearFileInput();
                        }
                    });

                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "File validation failed.")
                }
        });

    }

    private proceedImport(selectedFile: File): void {

        this._trainService.importTrain(this.projectId, selectedFile)
            .pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        (res.isWarning) ? this._toastr.warning(res.message) : this._toastr.success(res.message);
                        this.getTrainData();

                        if (res.records && res.records?.length > 0) 
                            this._excelHelper.downloadImportResponseFile<TrainInfoDtoModel>("Train", res.records, importTrainFileColumns);
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();
                },
                error: (errorRes) => {
                    this.clearFileInput();
                    this._toastr.error(errorRes?.error?.message || "Import failed.");
                }
            });
    }

    private clearFileInput(): void {
        if (this.importFileInput)
            this.importFileInput.nativeElement.value = '';

        this._cd.detectChanges();
    }
    //#endregion

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}