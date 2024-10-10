import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output, QueryList, ViewChildren } from "@angular/core";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatSelectModule } from "@angular/material/select";
import { TemplateListModel } from "@p/dialog/column-selector/add-column-template-dialog";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-list-column-selector",
    styleUrls: ["./list-column-selector.component.scss"],
    templateUrl: "./list-column-selector.component.html",
    imports: [
        CommonModule,
        MatSelectModule,
        MatCheckboxModule,
        NgScrollbarModule
    ],
    providers: [],
})
export class ListColumnSelectorComponent {
    @ViewChildren('checkboxRef') columnCheckBoxes: QueryList<any>;
    @Output() openColumnTemplate = new EventEmitter<string>();
    public defaultSelectedColumns: string[] = [];
    @Input() instrumentListColumns: {
        key: string;
        label: string;
    }[] = [];
    public templateListModel: TemplateListModel[] = []
    @Input() isTemplateShow: boolean = false;
    public templateSelectValue: string;

    constructor() { }

    protected async applyTemplate(templateId: string) {
        if (templateId == "new") {
            this.templateSelectValue = null;
            this.openColumnTemplate.emit();
        }
        else if (templateId == "0") {
            const selectedColumn: string[] = this.defaultSelectedColumns;

            if (selectedColumn.length > 0) {
                this.columnCheckBoxes.forEach(checkbox => {
                    checkbox.checked = selectedColumn.includes(checkbox.value);
                });
            }
        }
        else if (templateId && templateId != "") {
            const selectedTemplate = this.templateListModel.find(x => x.id == templateId).value;
            const selectedColumn: string[] = selectedTemplate.split(", ");

            if (selectedColumn.length > 0) {
                this.columnCheckBoxes.forEach(checkbox => {
                    checkbox.checked = selectedColumn.includes(checkbox.value);
                });
            }
        }
    }
}