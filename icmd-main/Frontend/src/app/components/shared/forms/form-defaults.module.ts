import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { NgScrollbarModule } from "ngx-scrollbar";

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    NgScrollbarModule
  ],
  //declarations: [InfiniteScrollSelectComponent],
  providers: [],
  //providers: [{ provide: MAT_TOOLTIP_DEFAULT_OPTIONS, useValue: formsTooltipDefaults }],
})
export class FormDefaultsModule { }
