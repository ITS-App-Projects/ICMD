import {
    ChangeDetectorRef,
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output,
} from "@angular/core";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import {
    MenuPermissionListModel,
    PermissionByMenuRoleModel,
} from "@m/permission";

@Component({
    standalone: true,
    selector: "app-list-permission-management-table",
    templateUrl: "./list-permission-management-table.component.html",
    imports: [FormDefaultsModule, MatTableModule, MatCheckboxModule],
    providers: [],
})
export class ListPermissionManagementTableComponent implements OnInit {
    @Output() public isPermissionChanged =
        new EventEmitter<PermissionByMenuRoleModel>();

    public isSystemAdmin: boolean = false;
    protected permissionList: {
        mainMenuName: string;
        data: MenuPermissionListModel[];
    }[] = [];

    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set menuListResponse(result: MenuPermissionListModel[]) {
        if (result) {
            this.permissionList = [];
            //this.dataSource = res;

            var groups = new Set(
                result.map((item) =>
                    item.parentMenuName ? item.parentMenuName : item.menuName
                )
            );
            groups.forEach((g) =>
                this.permissionList.push({
                    mainMenuName: g,
                    data: result.filter((i) =>
                        i.parentMenuName ? i.parentMenuName === g : i.menuName === g
                    ) as MenuPermissionListModel[],
                })
            );
        }
        this._cdr.detectChanges();
    }

    ngOnInit(): void { }

    ngAfterViewInit() { }

    protected changePermission(
        operationId: number,
        menuPermissionId: number,
        isGranted: boolean
    ): void {
        this.isPermissionChanged.emit({
            operation: operationId,
            isGranted: isGranted,
            menuPermissionId: menuPermissionId,
        });
    }
}
