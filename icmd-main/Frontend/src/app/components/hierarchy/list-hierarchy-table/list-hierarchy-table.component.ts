import { FlatTreeControl } from "@angular/cdk/tree";
import { CommonModule } from "@angular/common";
import { ChangeDetectorRef, Component, Input } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule } from "@angular/material/tree";
import { FormBaseComponent } from "@c/shared/forms";
import { HierarchyTypes, Options, RecordType } from "@e/common";
import { HierarchyService } from "src/app/service/hierarchy";
import { ExampleFlatNode, HierarchyDeviceInfoDtoModel, HierarchyRequestDtoModel, HierarchyResponceDtoModel } from "./list-hierarchy-table.model";
import { getGroup } from "@u/forms";
import { map, startWith, takeUntil } from "rxjs/operators";
import { Observable, Subject } from "rxjs";
import { DeviceDialogsService } from "src/app/service/device";
import { AppRoute } from "@u/app.route";
import { Router } from "@angular/router";
import { DropdownInfoDtoModel } from "@m/common";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { InlineSVGModule } from "ng-inline-svg-2";
import { ToastrService } from "ngx-toastr";

@Component({
    standalone: true,
    selector: "app-list-hierarchy-table",
    templateUrl: "./list-hierarchy-table.component.html",
    styleUrls: ['./styles/list-hierarchy-table.component.scss'],
    imports: [
        CommonModule,
        MatTreeModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatAutocompleteModule,
        InlineSVGModule
    ],
    providers: [HierarchyService, DeviceDialogsService],
})
export class ListHierarchyTableComponent extends FormBaseComponent<HierarchyRequestDtoModel> {
    projectId: string = null;
    selectedTag: string = null;
    protected hierarchyTypes = HierarchyTypes;
    protected types: string[] = [];
    protected recordTypeEnum = RecordType;
    protected recordType: string[] = [];
    protected tagNameFilteredOptions: Observable<DropdownInfoDtoModel[]>;
    protected hierarchyData: HierarchyResponceDtoModel;
    private _destroy$ = new Subject<void>();

    constructor(private _hierarchyService: HierarchyService, private _deviceDialogsService: DeviceDialogsService,
        private _cdr: ChangeDetectorRef,
        private _toastr: ToastrService,
        private _router: Router) {
        super(
            getGroup<HierarchyRequestDtoModel>(
                {
                    projectId: {},
                    hieararchyType: { v: "Control" },
                    option: { v: "Active" },
                    tagName: {}
                }
            )
        );

        this.types = Object.keys(this.hierarchyTypes);
        const recordkeys = Object.keys(this.recordTypeEnum);
        this.recordType = recordkeys.slice(recordkeys.length / 2);
    }

    @Input() public set items(projectId: string) {
        this.projectId = projectId;
        this.field('projectId').setValue(projectId);
        this.field('projectId').updateValueAndValidity();
        this.getHierarchyData();
    }

    protected async showDeviceInfo(event: string): Promise<void> {
        this.selectedTag = event;
        this._cdr.detectChanges();
        const url = event ? `/${AppRoute.viewDevice}/${event}` : `/${AppRoute.viewDevice}`;
        window.open(url, '_blank');
    }

    protected changeType(): void {
        this.field('tagName').setValue(null);
        this.getHierarchyData();
    }

    protected getHierarchyData(): void {
        const formValue = this.form.value;
        this._hierarchyService.getHierarchyData(formValue)
            .pipe(takeUntil(this._destroy$))
            .subscribe((res) => {
                 this.hierarchyData = res;
                if (res?.deviceList != null && res?.deviceList.length != 0)
                    this.dataSource.data = res?.deviceList;

                this.tagNameFilteredOptions = this.setupFilteredOptions('tagName', res?.tagList || []);
                this._cdr.detectChanges();
            })
    }

    protected searchDevice(): void {
        const tagName = this.field('tagName').value;
        const tagList = this.hierarchyData?.tagList;
        const tagInfo = tagList.find(a => a.name?.trim()?.toLowerCase() == tagName?.trim()?.toLowerCase())
        if (tagInfo) {
            this.dataSource.data = [...this.dataSource.data];
            this.dataSource.data.forEach((node) => {
                this.expandNodeIfContainsChild(node, tagName, tagInfo?.id);
            });
            setTimeout(() => {
                this.showDeviceInfo(tagInfo?.id);
            }, 1000);
        }
        else {
            this._toastr.error("There are no matching tags.");
        }
    }

    protected resetDevice(): void {
        this.form.reset();
        this.value = {
            projectId: this.projectId,
            hieararchyType: 'Control',
            option: 'Active',
            tagName: null
        }
        this.getHierarchyData();
    }

    protected treeControl = new FlatTreeControl<ExampleFlatNode>(
        node => node.level,
        node => node.expandable,
    );

    private expandNodeIfContainsChild(treeNode: HierarchyDeviceInfoDtoModel, childName: string, id: string): void {

        const node = this.treeControl.dataNodes.find(a => a.name == treeNode.name);
        if (treeNode.name == childName && node) {
            this.selectedTag = id;
            this._cdr.detectChanges();
            return;
        }
        if (treeNode.childrenList) {
            const matchingChild = this.findChildInNodeAndDescendants(treeNode, childName, id);
            if (matchingChild) {
                this.selectedTag = id;
                const currentNode = this.treeControl.dataNodes.find(a => a.name == matchingChild.name);
                if (currentNode && currentNode.level > 0) {
                    this.expandNodesByLevel(treeNode, treeNode.childrenList, currentNode.level - 1, currentNode.level, childName);
                }
                else {
                    this.expandParentChildNode(treeNode.name);
                }
                return;
            }
        }
    }

    private expandParentChildNode(name: string): void {
        const node = this.treeControl.dataNodes.find(a => a.name == name);
        if (node) {
            this.treeControl.expand(node);
            this._cdr.detectChanges();
            return;
        }
    }

    private expandNodesByLevel(nodeInfo: HierarchyDeviceInfoDtoModel, nodes: HierarchyDeviceInfoDtoModel[], level: number, currentLevel: number, childName: string): void {

        nodes?.forEach(node => {
            if (this.containsChildWithName(node, childName)) {
                const treeNode = this.treeControl.dataNodes.find(a => a.name === node.name);
                if (treeNode && treeNode.level < currentLevel) {
                    this.treeControl.expand(treeNode);
                    this._cdr.detectChanges();
                    this.expandNodesByLevel(nodeInfo, node.childrenList, level - 1, currentLevel, childName);
                }
                else if (level == 0) {
                    this.expandParentChildNode(nodeInfo.name);
                }
            }
        });
    }

    private containsChildWithName(node: HierarchyDeviceInfoDtoModel, childName: string): boolean {
        if (node.childrenList) {
            const isExist = node.name == childName;
            if (!isExist) {

                if (node.childrenList.some(child => child.name === childName)) {
                    return true;
                }

                // Recursively check each child's descendants
                for (const childNode of node.childrenList) {
                    if (this.containsChildWithName(childNode, childName)) {
                        return true; // Return true if found in descendants
                    }
                }
            }
            else {
                return isExist;
            }
        }
        else {
            return node.name == childName;
        }

        return false;
    }

    private findChildInNodeAndDescendants(node: HierarchyDeviceInfoDtoModel, childName: string, id: string): HierarchyDeviceInfoDtoModel | undefined {
        if (node) {
            const matchingParent = (node.name == childName);
            if (matchingParent) {
                return node;
            }

            if (node.childrenList) {
                const matchingChild = node.childrenList.find((child) => child.name === childName);
                if (matchingChild) {
                    return matchingChild;
                }

                // Recursively search in each child's descendants
                for (const childNode of node.childrenList) {
                    const matchingDescendant = this.findChildInNodeAndDescendants(childNode, childName, id);
                    if (matchingDescendant) {
                        return matchingDescendant;
                    }
                }
            }
        }

        return undefined;
    }

    private _transformer = (node: HierarchyDeviceInfoDtoModel, level: number) => {
        return {
            expandable: !!node.childrenList && node.childrenList.length > 0,
            name: node.name,
            id: node.id,
            level: level,
            isFolder: node.isFolder,
            isActive: node.isActive,
        };
    };

    private treeFlattener = new MatTreeFlattener(
        this._transformer,
        node => node.level,
        node => node.expandable,
        node => node.childrenList ?? [],
    );

    private setupFilteredOptions(field: string, dataList: DropdownInfoDtoModel[]): Observable<DropdownInfoDtoModel[]> {
        return this.field(field).valueChanges.pipe(
            startWith(''),
            map(val => val?.length >= 1 ? this._filter(val || '', dataList) : [])
        );
    }

    private _filter(value: string, dataList: DropdownInfoDtoModel[]): DropdownInfoDtoModel[] {
        const filterValue = value.toLowerCase();
        return dataList.filter(option => option?.name?.toLowerCase().includes(filterValue));
    }

    protected dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    protected hasChild = (_: number, node: ExampleFlatNode) => node.expandable;

    ngOnDestroy(): void {
        this._destroy$.next();
        this._destroy$.complete();
    }
}