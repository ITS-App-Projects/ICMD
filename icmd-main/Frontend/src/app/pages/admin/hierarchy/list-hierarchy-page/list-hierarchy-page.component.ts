import { FlatTreeControl } from "@angular/cdk/tree";
import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogModule } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { MatNestedTreeNode, MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule } from "@angular/material/tree";
import { Route, Router } from "@angular/router";
import { ListHierarchyTableComponent } from "@c/hierarchy/list-hierarchy-table";
import { PermissionWrapperComponent } from "@c/shared/permission-wrapper";
import { AppRoute } from "@u/app.route";
import { AppConfig } from "src/app/app.config";
import { DialogsService } from "src/app/service/common";
import { HierarchyService } from "src/app/service/hierarchy";

@Component({
    standalone: true,
    selector: "app-list-hierarchy-page",
    templateUrl: "./list-hierarchy-page.component.html",
    imports: [
        CommonModule,
        MatDialogModule,
        MatTreeModule,
        MatButtonModule,
        MatIconModule,
        ListHierarchyTableComponent,
        PermissionWrapperComponent
    ],
    providers: [DialogsService, HierarchyService]
})
export class ListHierarchyPageComponent {
    @ViewChild(ListHierarchyTableComponent) hierarchyTable: ListHierarchyTableComponent;
    private projectId: string | null = null;

    constructor(protected appConfig: AppConfig, private _router: Router, private _cdr: ChangeDetectorRef) {
    }

    ngAfterViewInit(): void {
        this.appConfig.projectIdFilter$.subscribe((res) => {
            if (res) {
                this.projectId = res?.id;
                this.hierarchyTable.items = this.projectId;
                this._cdr.detectChanges();
            }
        });
    }

    protected showDevice(event: string = null): void {
        this.appConfig.isPreviousURL$ = AppRoute.manageHierarchy;
        if (event) {
            const url = `/${AppRoute.manageDevice}/${event}`;
            window.open(url, '_blank');
        }
        else {
            this._router.navigate([AppRoute.manageDevice, event ?? ""]);
        }
    }
}