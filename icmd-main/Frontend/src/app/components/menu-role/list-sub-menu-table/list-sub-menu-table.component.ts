import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { FormDefaultsModule } from "@c/shared/forms";
import { MenuInfoModel } from "../menu-list-table";
import { ActiveInActiveDtoModel } from "@m/common";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";

@Component({
    standalone: true,
    selector: "app-list-sub-menu-table",
    templateUrl: "./list-sub-menu-table.component.html",
    imports: [FormDefaultsModule, MatTableModule, MatButtonModule, MatIconModule],
    providers: [],
})
export class ListSubMenuTableComponent implements OnInit {
    @Output() public activeInActive = new EventEmitter<ActiveInActiveDtoModel>();
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
        "actions",
    ];
    constructor(private _cdr: ChangeDetectorRef) { }

    @Input() public set subMenuListResponse(res: MenuInfoModel[]) {
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
}
