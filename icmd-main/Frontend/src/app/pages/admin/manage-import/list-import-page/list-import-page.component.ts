import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { mkConfig, generateCsv, download } from "export-to-csv";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ImportService } from "src/app/service/import";

@Component({
    standalone: true,
    selector: "app-list-import-page",
    templateUrl: "./list-import-page.component.html",
    imports: [
        CommonModule,
        MatButtonModule,
        PermissionWrapperComponent
    ],
    providers: [ImportService]
})
export class ListImportPageComponent {
    @ViewChild('fileInput', { static: false }) fileInput!: ElementRef;
    @ViewChild('fileInput1', { static: false }) fileInput1!: ElementRef;
    @ViewChild('fileInput2', { static: false }) fileInput2!: ElementRef;
    protected projectId: string = null;
    private _exportColumnTitles: Array<string> = [
        'ItemId', 'ItemDesc', 'ParentItemId', 'AssetTypeId'
    ];
    private _exportServiceDescriptionColumnTitles: Array<string> = [
        'Tag', 'Service Description', 'Area', 'Stream', 'Bank', 'Service', 'Variable', 'Train'
    ];
    private _exportEquipmentColumnTitles: Array<string> = [
        "PnPId", "Process Number", "Sub Process", "Stream",
        "Equipment Code", "Sequence Number", "Equipment Identifier",
        "Tag", "DWG Title", "REV", "VERSION", "Description",
        "Piping Class", "On Skid", "Function", "Tracking Number"
    ];
    private _exportInstrumentColumnTitles: Array<string> = [
        "PnPId", "Process Number", "Sub Process", "Stream",
        "Equipment Code", "Sequence Number", "Equipment Identifier",
        "Tag", "On Equipment", "On Skid", "Description", "FluidCode",
        "PipeLines.Tag", "Size", "DWG Title", "REV", "VERSION", "To",
        "From", "Tracking Number"
    ];
    private _exportValveColumnTitles: Array<string> = [
        "PnPId", "Process Number", "Sub Process", "Stream",
        "Equipment Code", "Sequence Number", "Equipment Identifier", "Tag",
        "DWG Title", "REV", "VERSION", "Description", "Size", "FluidCode",
        "PipeLines.Tag", "Piping Class", "Class Name", "On Skid", "Failure",
        "Switches", "From", "To", "Accessories", "Design Temp", "Nominal Pressure",
        "Valve Spec Number", "PN Rating", "Tracking Number"
    ];
    private _exportCCMDColumnTitles: Array<string> = ["TagNo", "PLCNumber", "WAP", "SystemCode", "SubsystemCode"];
    private _destroy$ = new Subject<void>();

    constructor(protected _appConfig: AppConfig, private _importService: ImportService, private _toastr: ToastrService,
        private _cdr: ChangeDetectorRef) {
        this.projectId = _appConfig.currentProjectId;
    }

    ngAfterViewInit(): void {
        this._appConfig.projectIdFilter$.subscribe((res) => {
            if (res && this.projectId != res?.id) {
                this.projectId = res?.id;
            }
        });
    }

    protected onFileSelected(event: any, type: string): void {
        if (event) {
            const selectedFile = event.target.files[0] ?? null;
            if (selectedFile && this.projectId) {
                let url = null;
                if (type == 'OM')
                    url = this._importService.uploadOMItem(this.projectId, selectedFile);
                else if (type == 'P&ID')
                    url = this._importService.uploadPIDs(this.projectId, selectedFile);
                else if (type == 'CCMD')
                    url = this._importService.uploadCCMD(this.projectId, selectedFile);


                url.pipe(takeUntil(this._destroy$))
                    .subscribe((res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                        } else {
                            this._toastr.error(res.message);
                        }
                        this.clearFileInput();
                    },
                        (errorRes: HttpErrorResponse) => {
                            this.clearFileInput();
                            if (errorRes?.error?.message) {
                                this._toastr.error(errorRes?.error?.message);
                            }
                        })
            }
        }
    }

    protected sampleFile(): void {
        this.omItemFile();
        this.omServiceDescriptionFile();
    }

    protected pIdSampleFiles(): void {
        this.equipmentFile();
        this.instrumentFile();
        this.valveFile();
    }

    protected ccmdSampleFiles(): void {
        this.ccmdFile();
    }

    private omItemFile(): void {
        this.getCSVFile("Sample_OMItem", this._exportColumnTitles);
    }

    private omServiceDescriptionFile(): void {
        this.getCSVFile("Sample_OMServiceDescription", this._exportServiceDescriptionColumnTitles);
    }

    private equipmentFile(): void {
        this.getCSVFile("Sample_EquipmentList", this._exportEquipmentColumnTitles);
    }

    private instrumentFile(): void {
        this.getCSVFile("Sample_InstrumentList", this._exportInstrumentColumnTitles);
    }

    private valveFile(): void {
        this.getCSVFile("Sample_ValveList", this._exportValveColumnTitles);
    }

    private ccmdFile(): void {
        this.getCSVFile("Sample_CCMD", this._exportCCMDColumnTitles);
    }

    private getCSVFile(fileName: string, columns: string[]) {
        const csvConfig = mkConfig({ filename: fileName, columnHeaders: [...columns], fieldSeparator: "," });
        const csv = generateCsv(csvConfig)([]);
        download(csvConfig)(csv);
    }

    private clearFileInput(): void {
        if (this.fileInput) {
            this.fileInput.nativeElement.value = '';
        }

        if (this.fileInput1) {
            this.fileInput1.nativeElement.value = '';
        }


        if (this.fileInput2) {
            this.fileInput2.nativeElement.value = '';
        }

        this._cdr.detectChanges();
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}