import { CommonModule } from "@angular/common";
import { Component, OnInit, ViewChild } from "@angular/core";
import { MatDialogModule } from "@angular/material/dialog";
import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { MenuInfoModel, MenuListTableComponent } from "@c/menu-role/menu-list-table";
import { SortOrderModel } from "@m/menu";
import { ToastrService } from "ngx-toastr";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { DialogsService } from "src/app/service/common";
import { MenuDialogsService, MenuService, SubMenuDialogsService } from "src/app/service/menu";
import { mapHelperModelToGetMenuInfoModel } from "./menu-list-page.mapper";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatIconModule } from "@angular/material/icon";

@Component({
    standalone: true,
    selector: "app-menu-list-page",
    templateUrl: "./menu-list-page.component.html",
    imports: [
        CommonModule,
        MatDialogModule,
        MenuListTableComponent,
        MatCheckboxModule,
        MatIconModule
    ],
    providers: [
        DialogsService,
        MenuDialogsService,
        SubMenuDialogsService,
        MenuService
    ],
})
export class MenuListPageComponent {
    @ViewChild(MenuListTableComponent) menuTable: MenuListTableComponent;
    protected emptyGuid: string = "00000000-0000-0000-0000-000000000000";

    private menuList: MenuInfoModel[] = [];
    private _destroy$ = new Subject<void>();
    constructor(
        private _dialog: DialogsService,
        private _menuDialogService: MenuDialogsService,
        private _subMenuDialogService: SubMenuDialogsService,
        private _menuservice: MenuService,
        private _toastr: ToastrService
    ) {
        this.getAllMenuData();
    }



    protected async addEditMenuDialog(id: string = this.emptyGuid): Promise<void> {
        const sortOrders = this.menuTable.dataSource.data.map((x) => x.sortOrder);
        const result = await this._menuDialogService.openMenuDialog(
            id,
            id == this.emptyGuid ? Math.max(0, ...sortOrders) + 1 : 0
        );
        if (result) this.refreshMenuTableRecord(result, id);
    }

    protected async deleteMenu($event: string) {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this menu?",
            "Confirm"
        );
        if (isOk) {
            this._menuservice.deleteMenu($event).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.changeSortOrder($event);
                        this.refreshMenuTableRecord(null, $event);
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

    protected async editMenu($event: string) {
        await this.addEditMenuDialog($event);
    }

    protected async viewMenu($event: string) {
        await this.showSubMenuListDialog($event);
    }

    protected changeSortOrder(id: string) {
        let changeSortOrderList: SortOrderModel[] = [];

        const record = this.menuList.find((x) => x.id === id);
        if (record) {
            const getRecordSortOrder: number = record.sortOrder;
            const newItems: MenuInfoModel[] = this.menuList
                .filter((x) => x.id !== id && x.sortOrder > getRecordSortOrder)
                .map((x) => ({
                    ...x,
                    sortOrder: x.sortOrder - 1,
                }));

            changeSortOrderList = newItems.map((x) => ({
                id: x.id,
                sortOrder: x.sortOrder,
            }));
        }

        if (changeSortOrderList.length > 0) {
            this._menuservice.changeSortOrderOfMenu(changeSortOrderList).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        changeSortOrderList.forEach((x) => {
                            const indexOfRecord = this.menuList.findIndex(
                                (item) => item.id === x.id
                            );
                            if (indexOfRecord > -1)
                                this.menuList[indexOfRecord].sortOrder = x.sortOrder;
                        });
                        this.menuTable.menuListResponse = this.menuList;
                    }
                },
                (errorRes) => {
                    this._toastr.error(errorRes?.error?.message);
                }
            );
        }
    }

    protected refreshMenuTableRecord(result: CreateOrEditMenuModel, id: string) {
        if (result) {
            if (id == this.emptyGuid) {
                this.menuList.push(mapHelperModelToGetMenuInfoModel(result));
            } else {
                const indexOfRecord = this.menuList.findIndex((item) => item.id === id);
                if (indexOfRecord > -1)
                    this.menuList[indexOfRecord] =
                        mapHelperModelToGetMenuInfoModel(result);
            }
        } else {
            if (id != this.emptyGuid) {
                const indexOfRecord = this.menuList.findIndex((item) => item.id === id);
                if (indexOfRecord > -1) this.menuList.splice(indexOfRecord, 1);
            }
        }

        this.menuTable.menuListResponse = this.menuList;
    }

    protected async showSubMenuListDialog(id: string): Promise<void> {
        const list = this.menuList.find((x) => x.id == id).subMenus;
        await this._subMenuDialogService.openSubMenuListDialog(list ?? [], id);
        this.getAllMenuData();
    }

    private getAllMenuData() {
        this._menuservice
            .getAllMenus()
            .pipe(takeUntil(this._destroy$))
            .subscribe((model: MenuInfoModel[]) => {
                this.menuList = model;
                this.menuTable.menuListResponse = model;
            });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}