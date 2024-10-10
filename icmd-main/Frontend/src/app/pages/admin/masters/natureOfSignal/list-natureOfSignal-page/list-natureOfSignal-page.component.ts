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
  NatureOfSignalDialogsService,
  NatureOfSignalSearchHelperService,
  NatureOfSignalService
} from 'src/app/service/natureOfSignal';

import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild
} from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { ListNatureOfSignalTableComponent } from '@c/masters/natureOfSignal/list-natureOfSignal-table';
import { FormDefaultsModule } from '@c/shared/forms';
import { ListActionsComponent } from '@c/shared/list-actions';
import { PermissionWrapperComponent } from '@c/shared/permission-wrapper';
import { SearchType } from '@e/common';
import {
  CustomFieldSearchModel,
  DropdownInfoDtoModel
} from '@m/common';
import { importNatureOfSignalTypeColumns } from '@u/constants';
import { ExcelHelper } from '@u/helper';

import { NatureOfSignalExportDtoModel } from './list-natureOfSignal-table-export.model';

@Component({
    standalone: true,
    selector: "app-list-natureOfSignal-page",
    templateUrl: "./list-natureOfSignal-page.component.html",
    imports: [
        CommonModule,
        FormDefaultsModule,
        ListNatureOfSignalTableComponent,
        MatDialogModule,
        PermissionWrapperComponent,
        ListActionsComponent
    ],
    providers: [
        NatureOfSignalService,
        NatureOfSignalSearchHelperService,
        DialogsService,
        ExcelHelper,
        NatureOfSignalDialogsService
    ]
})
export class ListNatureOfSignalPageComponent {
    @ViewChild('importFileInput', { static: false }) importFileInput!: ElementRef;
    @ViewChild(ListNatureOfSignalTableComponent) natureOfSignalTable: ListNatureOfSignalTableComponent;
    protected manufacturerData: DropdownInfoDtoModel[] = [];
    private customFilters$: BehaviorSubject<CustomFieldSearchModel[]> = new BehaviorSubject([]);
    private _destroy$ = new Subject<void>();
    private columnFilterList: CustomFieldSearchModel[] = [];

    constructor(
        protected _natureOfSignalSearchHelperService: NatureOfSignalSearchHelperService,
        private _natureOfSignalService: NatureOfSignalService,
        private _toastr: ToastrService,
        private _dialog: DialogsService,
        protected appConfig: AppConfig,
        private _natureOfSignalDialogService: NatureOfSignalDialogsService,
        private _excelHelper: ExcelHelper,
        private _cd: ChangeDetectorRef) {
        this.getNatureOfSignalData();
    }

    ngAfterViewInit(): void {
        this.natureOfSignalTable.sortingChanged.pipe().subscribe((res) => {
            this.defaultCustomFilter();
            this._natureOfSignalSearchHelperService.updateSortingChange(res);
        });

        this.natureOfSignalTable.pagingChanged.pipe().subscribe((page) => {
            this.defaultCustomFilter();
            this._natureOfSignalSearchHelperService.updatePagingChange(page);
        });

        this.customFilters$.pipe(takeUntil(this._destroy$)).subscribe((filter) => {
            this._natureOfSignalSearchHelperService.updateFilterChange(filter);
        });

        this.tableColumnchanges();
    }

    protected search($event): void {
        this.defaultCustomFilter();
        this._natureOfSignalSearchHelperService.commonSearch($event);
    }

    protected async delete($event): Promise<void> {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this nature of signal?",
            "Confirm"
        );
        if (isOk) {
            this._natureOfSignalService.deleteNatureOfSignal($event).pipe(takeUntil(this._destroy$)).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getNatureOfSignalData();
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

    protected async addEditNatureOfSignalDialog(event: string = null): Promise<void> {
        await this._natureOfSignalDialogService.openNatureOfSignalDialog(event);
        this.getNatureOfSignalData();
    }

    protected exportData(): void {
        const fileName = 'Export_Nature_Of_Signals';

        this.defaultCustomFilter(true, this.columnFilterList);
        this._natureOfSignalSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$), take(1))
            .subscribe((model) => {
                const res = model.items;
                const columnMapping = {
                    'natureOfSignalName': 'Name'
                };
                this._excelHelper.exportExcel(res, columnMapping, fileName);
            });
    }

    private getNatureOfSignalData(): void {
        this.defaultCustomFilter();
        this._natureOfSignalSearchHelperService
            .loadDataFromRequest()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model) => { });
    }

    protected defaultCustomFilter(isExport: boolean = false, columnFilterList: CustomFieldSearchModel[] = []): void {
        let filters: CustomFieldSearchModel[] = [
            { fieldName: "isExport", fieldValue: isExport ? "true" : "false", searchType: SearchType.Contains, isColumnFilter: false }
        ];
        if (columnFilterList && columnFilterList.length > 0)
            filters.push(...columnFilterList)

        this.customFilters$.next(filters);
    }

    private tableColumnchanges() {
        this._cd.detectChanges();
        this.columnFilterList = [];
        combineLatest(this.natureOfSignalTable.columnFiltersList.map(x => x.columnFilterModel$))
            .pipe(takeUntil(this._destroy$)).subscribe((res) => {
                if (res && res.length > 0) {
                    this.columnFilterList = res.filter(x => x);
                    this.defaultCustomFilter(false, this.columnFilterList);
                }
            });
    }

     //#region Import Functionality
     protected importFileDownload() {
        const csvConfig = mkConfig({ filename: 'Sample_NatureOfSignal', columnHeaders: importNatureOfSignalTypeColumns, fieldSeparator: "," });
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

        this._natureOfSignalService.importNatureOfSignal(selectedFile).pipe(takeUntil(this._destroy$))
            .subscribe({
                next: (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.getNatureOfSignalData();
                    } else {
                        this._toastr.error(res.message);
                    }
                    this.clearFileInput();

                    if (res.records && res.records?.length > 0)
                        this._excelHelper.downloadImportResponseFile<NatureOfSignalExportDtoModel>("NatureOfSignal", res.records, importNatureOfSignalTypeColumns);

                },
                error: (errorRes) => {
                    this.clearFileInput();
                    if (errorRes?.error?.message) {
                        this._toastr.error(errorRes?.error?.message);
                    }
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