import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component } from "@angular/core";
import { FormArray, FormsModule } from "@angular/forms";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { TagFieldsService } from "src/app/service/tag-Fields";
import { ProjectTagFieldsInfoDtoModel, TagFieldInfoDtoModel } from "./list-tag-Fields-page.model";
import { FormBaseComponent, FormDefaultsModule } from "@c/shared/forms";
import { FormGroupG, getGroup } from "@u/forms";
import { TagFieldSource } from "@e/common";
import { HttpErrorResponse } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";

@Component({
    standalone: true,
    selector: "app-list-tag-Fields-page",
    templateUrl: "./list-tag-Fields-page.component.html",
    imports: [
        CommonModule,
        FormsModule,
        FormDefaultsModule,
        PermissionWrapperComponent
    ],
    providers: [TagFieldsService]
})
export class ListTagFieldsPageComponent extends FormBaseComponent<ProjectTagFieldsInfoDtoModel> {
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";
    protected projectId: string = null;
    protected tagFieldData: TagFieldInfoDtoModel[] = [];
    protected fieldSource = TagFieldSource;
    protected sources: string[] = [];
    private _destroy$ = new Subject<void>();

    constructor(private _tagFieldService: TagFieldsService, protected appConfig: AppConfig, private _cdr: ChangeDetectorRef,
        private _toastr: ToastrService) {
        super(
            getGroup<ProjectTagFieldsInfoDtoModel>({
                tagFields: new FormArray<FormGroupG<TagFieldInfoDtoModel>>([])
            })
        )
        this.sources = Object.keys(this.fieldSource);
        this.getProjectTagFields();
    }

    ngAfterViewInit(): void {
        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id?.toString() ?? null;
                this.getProjectTagFields();
            }
        })
    }

    protected updateProjectTagFields(): void {
        if (this.projectId) {
            const tagFieldValue = this.form.value;
            if (tagFieldValue == null || tagFieldValue == undefined) {
                return;
            }

            this._tagFieldService.updateProjectTagFields(tagFieldValue)
                .pipe(takeUntil(this._destroy$))
                .subscribe(
                    (res) => {
                        if (res && res.isSucceeded) {
                            this._toastr.success(res.message);
                            this.appConfig.isUpdateTagField$.next(true);
                            this.getProjectTagFields();
                        } else {
                            this._toastr.error(res.message);
                        }
                    },
                    (errorRes: HttpErrorResponse) => {
                        if (errorRes?.error?.message) {
                            this._toastr.error(errorRes?.error?.message);
                        }
                    }
                );
        }
    }


    protected getProjectTagFields(): void {
        if (this.projectId) {
            this._tagFieldService.getProjectTagFieldInfo(this.projectId)
                .pipe(takeUntil(this._destroy$))
                .subscribe((res) => {
                    const tagList = this.array("tagFields");
                    tagList.clear();
                    this.tagFieldData = res;
                    res.map((item) => {
                        this.addTagFields(item);
                    })
                    this._cdr.detectChanges();
                })
        }
    }

    private addTagFields(info: TagFieldInfoDtoModel = null): void {
        const tagList = this.array("tagFields");
        tagList.push(
            getGroup<TagFieldInfoDtoModel>({
                id: { v: info?.id ?? this.emptyGuid },
                name: { v: info?.name ?? null },
                source: { v: info?.source != null && info?.source != '' ? info?.source : this.sources[0] },
                separator: { v: info?.separator ?? null },
                projectId: { v: info?.projectId ?? null },
                isEditable: { v: info?.isEditable ?? false }
            })
        );
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}