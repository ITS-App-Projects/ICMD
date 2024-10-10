import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { MenuInfoModel } from "./menu-list-table.model";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";

@Component({
    standalone: true,
    selector: "app-menu-list-table",
    templateUrl: "./menu-list-table.component.html",
    imports: [FormDefaultsModule, MatTableModule, MatIconModule, MatButtonModule],

})
export class MenuListTableComponent implements OnInit {
    @Output() public edit = new EventEmitter<string>();
    @Output() public view = new EventEmitter<string>();
    @Output() public delete = new EventEmitter<string>();
    public dataSource: MatTableDataSource<MenuInfoModel>;
    protected displayedColumns = [
        "sortOrder",
        "menuName",
        "menuDescription",
        "controllerName",
        "url",
        "icon",
    ];
    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set menuListResponse(res: MenuInfoModel[]) {
        if (res) this.dataSource = new MatTableDataSource([...res]);
        this._cdr.detectChanges();
    }

    ngOnInit(): void { }

    protected deleteMenu(id: string) {
        this.delete.emit(id);
    }

    protected editMenu(id: string) {
        this.edit.emit(id);
    }

    protected viewMenu(id: string) {
        this.view.emit(id);
    }
}