import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { Observable, Subject } from "rxjs";
import { map, startWith, takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { LogsService } from "src/app/service/logs";
import { UIChangeLogRequestDtoModel, UIChangeLogTypeDropdownInfoDtoModel } from "./list-logs-page.model";
import { getGroup } from "@u/forms";
import { ChangeLogResponceDtoModel, ListLogsTableComponent } from "@c/manage-logs/list-logs-table";
import { CommonFunctions } from "@u/helper";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatNativeDateModule } from "@angular/material/core";
import { DropdownInfoDtoModel, PagingDataModel } from "@m/common";
import { NgScrollbarModule } from "ngx-scrollbar";
import { MatTabsModule } from "@angular/material/tabs";

@Component({
    standalone: true,
    selector: "app-list-logs-page",
    templateUrl: "./list-logs-page.component.html",
    imports: [
        CommonModule,
        MatTabsModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        FormDefaultsModule,
        MatExpansionModule,
        ListLogsTableComponent,
        MatAutocompleteModule,
        MatDatepickerModule,
        MatNativeDateModule,
        NgScrollbarModule
    ],
    providers: [LogsService]
})
export class ListLogsPageComponent extends FormBaseComponent<UIChangeLogRequestDtoModel> {
    @ViewChild('logsTable', { static: false }) logsTable: ListLogsTableComponent;
    projectId: string | null = null;
    protected typesAndDropdownData: UIChangeLogTypeDropdownInfoDtoModel = null;
    protected changeLogsData: ChangeLogResponceDtoModel[] = [];
    protected tagFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected plcNoFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected userNoFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    private _destroy$ = new Subject<void>();
    public totalLength: number = 0;

    constructor(private _logsService: LogsService, private _appConfig: AppConfig, private _cdr: ChangeDetectorRef,
        private _commonFunctions: CommonFunctions) {
        super(
            getGroup<UIChangeLogRequestDtoModel>({
                projectId: {},
                type: {},
                tag: {},
                plcNo: {},
                username: {},
                startDate: {},
                endDate: {},
                pageNumber: {},
                pageSize: {}
            })
        );
        this.projectId = _appConfig.currentProjectId;
        this.getChangeLogTypes();
    }

    ngAfterViewInit(): void {
        this.field('projectId').setValue(this.projectId);
        this._appConfig.projectIdFilter$.subscribe((res) => {
            if (res && this.projectId != res?.id) {
                this.projectId = res?.id;
                this.field('projectId').setValue(this.projectId);
                this.getChangeLogTypes();
            }
        });
    }

    protected changeTab(event: any): void {
        if (event) {
            if (event.textLabel && this.field('type').value != event.textLabel) {
                this.field('type').setValue(event.textLabel);
                this.getChangeLogsData();
            }
        }
    }

    protected searchLogs(): void {
        this.getChangeLogsData();
    }

    protected resetFilter() {
        const formValue = this.value;
        this.form.reset();
        this.field('projectId').setValue(this.projectId);
        this.field('type').setValue(formValue?.type);
        this._cdr.detectChanges();
        this.getChangeLogsData();
    }

    private getChangeLogTypes(): void {
        if (this.projectId) {
            this._logsService.getChangeLogTypes(this.projectId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.typesAndDropdownData = res;
                    this.field('type').setValue(res?.types.length != 0 ? res?.types[0] : "");
                    this.getChangeLogsData();
                    this.autoCompleteValueChange();
                    this._cdr.detectChanges();
                })
        }
    }

    private getChangeLogsData(pageNumber: number = 1, pageSize: number = 10): void {
        if (this.field('type').value) {
            const formData = this.form.value;
            formData.startDate = this._commonFunctions.isEmptyOrNull(formData.startDate) ? null : formData.startDate;
            formData.endDate = this._commonFunctions.isEmptyOrNull(formData.endDate) ? null : formData.endDate;

            formData.pageNumber = pageNumber;
            formData.pageSize = pageSize;
            
            this._logsService.getChangeLogsData(formData)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    this.changeLogsData = res.items;
                    console.log(this.changeLogsData);
                    
                    this.totalLength = res.totalCount;
                    console.log(this.totalLength);
                    this._cdr.detectChanges();
                })
        }
    }

    onPagingChanged(event: PagingDataModel) {
        this.getChangeLogsData(event.pageNumber, event.pageSize);
    }

    private autoCompleteValueChange(): void {
        this.tagFilteredOptions = this.setupFilteredOptions('tag', this.typesAndDropdownData?.tagList || []);
        this.plcNoFilteredOptions = this.setupFilteredOptions('plcNo', this.typesAndDropdownData?.plcList || []);
        this.userNoFilteredOptions = this.setupFilteredOptions('username', this.typesAndDropdownData?.userList || []);
    }

    private setupFilteredOptions(field: string, dataList: DropdownInfoDtoModel[]): Observable<DropdownInfoDtoModel[]> {
        return this.field(field).valueChanges.pipe(
            startWith(''),
            map(val => val?.length >= 1 ? this._commonFunctions._filter(val || '', dataList) : [])
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}