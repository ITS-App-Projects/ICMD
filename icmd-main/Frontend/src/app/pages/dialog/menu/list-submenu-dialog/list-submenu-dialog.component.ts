import { ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { ListSubMenuTableComponent } from "@c/menu-role/list-sub-menu-table";
import { Subject } from "rxjs";
import { DialogsService } from "src/app/service/common";
import { MenuDialogsService, MenuService } from "src/app/service/menu";
import { ListSubMenuDialogInputDataModel, ListSubMenuDialogOutputDataModel } from "./list-submenu-dialog.model";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { CreateOrEditMenuModel } from "@c/menu-role/create-or-edit-menu-form";
import { mapHelperModelToGetCreateEditMenuModel } from "./list-submenu-dialog.mapper";

@Component({
    standalone: true,
    selector: "list-sub-menu-dialog",
    templateUrl: "./list-submenu-dialog.component.html",
    providers: [
        MenuDialogsService,
        MenuService,
        DialogsService,

    ],
    imports: [MatButtonModule, ListSubMenuTableComponent, MatDialogModule],
})
export class ListSubMenuDialogComponent implements OnInit {
    @ViewChild("subMenuTable") subMenuTable: ListSubMenuTableComponent;
    private emptyGuid: string = "00000000-0000-0000-0000-000000000000";

    private _destroy$ = new Subject<void>();
    constructor(
        private _dialogRef: MatDialogRef<
            ListSubMenuDialogComponent,
            ListSubMenuDialogOutputDataModel
        >,
        @Inject(MAT_DIALOG_DATA)
        protected _inputData: ListSubMenuDialogInputDataModel,
        private _menuDialogService: MenuDialogsService,
        private _dialog: DialogsService,
        private _toastr: ToastrService,
        private _menuService: MenuService,
        private _cdr: ChangeDetectorRef
    ) { }
    ngOnInit(): void { }

    ngAfterViewInit(): void {
        this.getAllSubMenuData();
    }

    private getAllSubMenuData() {
        this.subMenuTable.subMenuListResponse = this._inputData.subMenus;
    }

    protected async addEditSubMenuDialog(id: string = this.emptyGuid): Promise<void> {
        const sortOrders = this.subMenuTable.dataSource.data.map(
            (x) => x.sortOrder
        );
        if (sortOrders.length > 0) sortOrders.push(0);
        const result = await this._menuDialogService.openMenuDialog(
            id,
            id == this.emptyGuid ? Math.max(0, ...sortOrders) + 1 : 1,
            true,
            this._inputData.menuId
        );
        if (result) this.refreshSubMenuTableRecord(result, id);
    }

    protected async deleteMenu($event: string) {
        const isOk = await this._dialog.confirm(
            "Are you sure you want to delete this menu?",
            "Confirm"
        );
        if (isOk) {
            this._menuService.deleteMenu($event).subscribe(
                (res) => {
                    if (res && res.isSucceeded) {
                        this._toastr.success(res.message);
                        this.refreshSubMenuTableRecord(null, $event);
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
        await this.addEditSubMenuDialog($event);
    }

    private refreshSubMenuTableRecord(result: CreateOrEditMenuModel, id: string) {
        if (result) {
            if (id == this.emptyGuid) {
                this._inputData.subMenus.push(
                    mapHelperModelToGetCreateEditMenuModel(result)
                );
            } else {
                const indexOfRecord = this._inputData.subMenus.findIndex(
                    (item) => item.id === id
                );
                if (indexOfRecord > -1)
                    this._inputData.subMenus[indexOfRecord] =
                        mapHelperModelToGetCreateEditMenuModel(result);
            }
        } else {
            if (id != this.emptyGuid) {
                const indexOfRecord = this._inputData.subMenus.findIndex(
                    (item) => item.id === id
                );
                if (indexOfRecord > -1)
                    this._inputData.subMenus.splice(indexOfRecord, 1);
            }
        }
        this.getAllSubMenuData();
    }

    protected cancel(): void {
        this._dialogRef.close({ success: false });
    }

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}