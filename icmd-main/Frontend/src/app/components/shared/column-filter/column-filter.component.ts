import { CommonModule } from "@angular/common";
import { Component, ElementRef, Input, OnDestroy, ViewChild } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import { FormDefaultsModule } from "../forms";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatMenuModule } from "@angular/material/menu";
import { SearchFilterType } from "@e/common";
import { CustomFieldSearchModel } from "@m/common";

@Component({
    standalone: true,
    selector: "column-filter",
    templateUrl: "./column-filter.component.html",
    styleUrls: ["./column-filter.component.scss"],
    imports: [CommonModule, FormDefaultsModule, MatIconModule, MatButtonModule, MatIconModule, MatMenuModule],
})
export class ColumnFilterComponent implements OnDestroy {
    public columnFilterModel$: BehaviorSubject<CustomFieldSearchModel> = new BehaviorSubject(null);
    @ViewChild('filterInput') searchText: ElementRef<HTMLInputElement>;
    @Input() fieldName: string = '';

    protected searchTypesEnum = SearchFilterType;
    protected selectedFilterType: string = this.searchTypesEnum.Contains;
    protected placeHolder: string = this.searchTypesEnum.Contains;
    protected searchTypes: string[] = [];
    private _destroy$: Subject<void> = new Subject<void>();
    constructor() {
        const keys = Object.keys(this.searchTypesEnum);
        this.searchTypes = keys;
    }

    ngAfterViewInit(): void { }

    public setFilter(filterValue: any): void {
        this.columnFilterModel$.next(filterValue);
    }

    protected changeFilterType(filterType: string = "") {
        if (filterType) {
            this.selectedFilterType = filterType;
            this.placeHolder = this.searchTypesEnum[filterType];
        }
        else {
            this.searchText.nativeElement.value = "";
            this.selectedFilterType = this.searchTypesEnum.Contains;
            this.placeHolder = this.searchTypesEnum.Contains;
        }
        this.applyFilter(this.searchText.nativeElement.value);
    }

    protected applyFilter(searchTest: string) {
        if (searchTest?.trim() == "") {
            this.columnFilterModel$.next(null);
        } else {
            const model: CustomFieldSearchModel = {
                fieldName: this.fieldName.split("_Filter")[0],
                fieldValue: searchTest ?? "",
                isColumnFilter: true,
                searchType: SearchFilterType[this.selectedFilterType]
            };
            this.columnFilterModel$.next(model);
        }
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}