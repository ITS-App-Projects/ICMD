import {
  merge,
  BehaviorSubject,
  Observable
} from 'rxjs';
import { map } from 'rxjs/operators';
import { HierarchyService } from 'src/app/service/hierarchy';

import {
  CollectionViewer,
  DataSource,
  SelectionChange
} from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';

import {
  ChildrenRequestDtoModel,
  ExampleFlatNode,
  HierarchyDeviceInfoDtoModel
} from './list-hierarchy-table.model';

export class DynamicDataSource implements DataSource<ExampleFlatNode> {
    dataChange = new BehaviorSubject<ExampleFlatNode[]>([]);

    get data(): ExampleFlatNode[] {
      return this.dataChange.value;
    }
    set data(value: ExampleFlatNode[]) {
      this._treeControl.dataNodes = value;
      this.dataChange.next(value);
    }

    constructor(
      private _treeControl: FlatTreeControl<ExampleFlatNode>,
      private _hierarchyService: HierarchyService,
      private _projectId: string,
      private _option: string,
      private _hieararchyType: string
    ) {}

    connect(collectionViewer: CollectionViewer): Observable<ExampleFlatNode[]> {
      this._treeControl.expansionModel.changed.subscribe((change) => {
        if (
          (change as SelectionChange<ExampleFlatNode>).added ||
          (change as SelectionChange<ExampleFlatNode>).removed
        ) {
          this.handleTreeControl(change as SelectionChange<ExampleFlatNode>);
        }
      });

      return merge(collectionViewer.viewChange, this.dataChange).pipe(
        map(() => this.data)
      );
    }

    disconnect(collectionViewer: CollectionViewer): void {}

    //#region  Handle expand/collapse behaviors
    handleTreeControl(change: SelectionChange<ExampleFlatNode>) {
      if (change.added) {
        change.added.forEach((node) => this.toggleNode(node, true));
      }
      if (change.removed) {
        change.removed
          .slice()
          .reverse()
          .forEach((node) => this.toggleNode(node, false));
      }
    }

    // Toggle the node, remove from display list
    //#region Toggle Node
    toggleNode(node: ExampleFlatNode, expand: boolean) {
        const payload: ChildrenRequestDtoModel = {
            deviceId: node.id,
            projectId: this._projectId,
            option: this._option,
            hieararchyType: this._hieararchyType,
        };

      const children = this._hierarchyService.getChildrenData(payload);
      const index = this.data.indexOf(node);
      if (!children || index < 0) {
        return;
      }

        children.pipe()
        .subscribe((res: HierarchyDeviceInfoDtoModel[]) => {
            var nodes = res.map(child => new ExampleFlatNode(
                child.id,
                child.name,
                child.instrument,
                child.isFolder,
                child.isActive,
                child.childrenList && child.childrenList.length > 0,
                node.level + 1
            ));

            if (expand) {
                this.data.splice(index + 1, 0, ...nodes);
            } else {
                let count = 0;
                for (
                  let i = index + 1;
                  i < this.data.length && this.data[i].level > node.level;
                  i++, count++
                ) {}
                this.data.splice(index + 1, count);
            }

            this.dataChange.next(this.data);
        });
    }
}