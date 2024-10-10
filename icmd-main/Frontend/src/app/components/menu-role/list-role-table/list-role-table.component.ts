import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MAT_CHECKBOX_DEFAULT_OPTIONS, MatCheckboxModule } from "@angular/material/checkbox";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatSortModule } from "@angular/material/sort";
import { MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { RoleEnum } from "@e/common";
import { MenuListWithRoleModel, RoleMenuPermissionModel } from "@m/menu";

@Component({
    standalone: true,
    selector: "app-list-role-table",
    templateUrl: "./list-role-table.component.html",
    imports: [
        FormDefaultsModule,
        MatTableModule,
        MatCheckboxModule
    ],
    providers: [
        {provide: MAT_CHECKBOX_DEFAULT_OPTIONS, useValue: {color: "primary"}}
    ]
})
export class ListRoleTableComponent implements OnInit {
    @Output() public isPermissionChanged =
        new EventEmitter<RoleMenuPermissionModel>();
    public dataSource: MenuListWithRoleModel = {
        menuList: [],
        roleList: [],
    };
    protected administrator = RoleEnum.Administrator;

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set menuListResponse(res: MenuListWithRoleModel) {
        if (res) this.dataSource = res;
        this._cdr.detectChanges();
    }

    ngOnInit(): void { }
    ngAfterViewInit() { }

    protected changePermission(
        roleId: string,
        menuId: number,
        isGranted: boolean
    ): void {
        this.isPermissionChanged.emit({
            roleId: roleId,
            menuId: menuId,
            isGranted: isGranted,
        });
    }
}