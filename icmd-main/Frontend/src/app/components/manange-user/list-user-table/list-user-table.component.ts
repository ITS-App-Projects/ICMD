import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from "@angular/core";
import { MatPaginator, MatPaginatorModule } from "@angular/material/paginator";
import { MatSort, MatSortModule } from "@angular/material/sort";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms/form-defaults.module";
import { SearchSortType } from "@e/search";
import { LoggedInUser } from "@m/auth";
import { PagingDataModel, SortingDataModel } from "@m/common";
import { pageSizeOptions } from "@u/index";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { AppConfig } from "src/app/app.config";
import { ListUserTableModel } from ".";
import { NoRecordComponent } from "@c/shared/no-record";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule, MatIconRegistry } from "@angular/material/icon";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { manangeUserListTableColumns } from "@u/constants";
import { FilterColumnsPipe } from "@u/pipe";
import { ColumnFilterComponent } from "@c/shared/column-filter";
import { DomSanitizer } from "@angular/platform-browser";
import { RoleEnum } from "@e/common";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatProgressBarModule } from "@angular/material/progress-bar";

@Component({
  standalone: true,
  selector: "app-list-user-table",
  templateUrl: "./list-user-table.component.html",
  imports: [
    FormDefaultsModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    NoRecordComponent,
    MatIconModule, MatButtonModule, PermissionWrapperComponent,
    ColumnFilterComponent,
    FilterColumnsPipe,
    MatTooltipModule,
    MatProgressBarModule
  ],
  providers: [],
})
export class ListUserTableComponent implements OnInit {
  @ViewChildren(ColumnFilterComponent) columnFiltersList: QueryList<ColumnFilterComponent>;
  @Output() public pagingChanged = new EventEmitter<PagingDataModel>();
  @Output() public sortingChanged = new EventEmitter<SortingDataModel>();
  @Output() public search = new EventEmitter<string>();
  @Output() public delete = new EventEmitter<number>();
  @Output() public edit = new EventEmitter<number>();
  @Output() public passwordChange = new EventEmitter<number>();
  @Input() dataSource: MatTableDataSource<ListUserTableModel>;
  @Input() totalLength: number = 0;
  protected currentUserInfo: LoggedInUser;
  protected roleInfo = RoleEnum;
  private _destroy$ = new Subject<void>();

  public displayedColumns = [...manangeUserListTableColumns].map(x => x.key);
  protected isLoading: boolean;
  protected pageSizeOptions = pageSizeOptions;

  @ViewChild(MatPaginator) private _paginator: MatPaginator;
  @ViewChild(MatSort) private _sort: MatSort;

  constructor(protected appConfig: AppConfig, private matIconRegistry: MatIconRegistry, private domSanitizer: DomSanitizer) {
    this.matIconRegistry.addSvgIcon(
      'my-svg-icon',
      this.domSanitizer.bypassSecurityTrustResourceUrl('../../../../assets/media/svg/icons/Communication/Shield-user.svg')
    );

    this.currentUserInfo = this.appConfig.getCurrentUser();
  }

  ngOnInit(): void { }

  @Input() public set items(value: ReadonlyArray<ListUserTableModel>) {
    this.dataSource = new MatTableDataSource([...value]);
  }

  ngAfterViewInit() {
    this._sort.sortChange.pipe(takeUntil(this._destroy$)).subscribe((sort) => {
      this._paginator.firstPage();
      this.sortingChanged.emit({
        sortType:
          sort.direction === "asc"
            ? SearchSortType.Ascending
            : SearchSortType.Descending,
        sortField: sort.active,
      });
    });

    this._paginator.page.pipe(takeUntil(this._destroy$)).subscribe((page) => {
      this.pagingChanged.emit({
        pageSize: page.pageSize,
        pageNumber: page.pageIndex + 1,
      });
    });
  }

  protected deleteUser(id: number) {
    this.delete.emit(id);
  }

  protected editUser(id: number) {
    this.edit.emit(id);
  }

  protected changePassword(id: number) {
    this.passwordChange.emit(id);
  }

  protected applyFilter(search: string) {
    this.search.emit(search);
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }
}