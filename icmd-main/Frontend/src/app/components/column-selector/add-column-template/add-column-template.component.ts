import { CommonModule } from "@angular/common";
import { Component, ElementRef, EventEmitter, Output, ViewChild } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatTooltipModule } from "@angular/material/tooltip";
import { FormDefaultsModule } from "@c/shared/forms";
import { TemplateListModel } from "@p/dialog/column-selector/add-column-template-dialog";
import { NgScrollbarModule } from "ngx-scrollbar";

@Component({
    standalone: true,
    selector: "app-column-template",
    templateUrl: "./add-column-template.component.html",
    imports: [
        CommonModule,
        MatInputModule,
        FormDefaultsModule,
        MatButtonModule,
        MatIconModule,
        MatTooltipModule,
        NgScrollbarModule
    ],
    providers: [],
})
export class AddColumnTemplateComponent {
    @ViewChild('templateName') templateName!: ElementRef<HTMLInputElement>;
    @Output() saveTemplate = new EventEmitter<string>();
    @Output() deleteTemplate = new EventEmitter<string>();
    public templateList: TemplateListModel[] = [];
    constructor() { }

    protected saveAsTemplate() {
        if (this.templateName.nativeElement.value && this.templateName.nativeElement.value?.trim() != "")
            this.saveTemplate.emit(this.templateName.nativeElement.value);
    }

    protected deleteTemplateById(id: string) {
        this.deleteTemplate.emit(id);
    }

    protected get isNotValid() {
        return (!this.templateName?.nativeElement?.value || this.templateName?.nativeElement?.value?.trim() == "")
    }
}