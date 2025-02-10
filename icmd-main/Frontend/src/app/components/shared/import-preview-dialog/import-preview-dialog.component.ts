import { Component, Inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from "@angular/material/dialog";
import { MatTableModule } from "@angular/material/table";
import { MatExpansionModule } from "@angular/material/expansion";
import { CapitalizePipe } from "@u/pipe/capitalize.pipe";
import { NgScrollbarModule } from "ngx-scrollbar";


@Component({
  standalone: true,
  selector: "app-import-preview-dialog",
  imports: [
    MatDialogModule,
    MatTableModule,
    MatExpansionModule,
    CommonModule,
    CapitalizePipe,
    NgScrollbarModule
  ],
  template: `
    <div>
      <h2 class="m-5"> Changes Preview </h2>
      <mat-accordion>
        <mat-expansion-panel *ngFor="let item of dummyData" class="border border-secondary mt-2 mx-5 rounded">
          <mat-expansion-panel-header>
            <mat-panel-title>
                <h4 class="card-label pt-1"> {{ item.name }} </h4>
            </mat-panel-title>
            <mat-panel-description>
                <span [ngClass]="item.status === 'success' ? 'text-success text-center h5' : 'text-danger text-center h5'">
                    {{ item.status | capitalize }} 
                </span>
            </mat-panel-description>
          </mat-expansion-panel-header>

          <div class="text-start h6 my-3">
            Operation: {{ item.operation | capitalize }}
          </div>

          <div class="row col-12 table-responsive mt-3">
            <table class="table table-stripped">
                <thead>
                    <tr>
                        <th>Item Column Name</th>
                        <th>Previous Value</th>
                        <th>New Value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr *ngFor="let change of item.changes">
                        <td>{{ change.itemColumnName }}</td>
                        <td>{{ change.previousValue || 'N/A' }}</td>
                        <td>{{ change.newValue }}</td>
                    </tr>
                </tbody>                    
            </table>
          </div>
        </mat-expansion-panel>
      </mat-accordion>
    </div>
    
    <div class="d-flex justify-content-end my-5">
        <button type="button" class="btn btn-outline-secondary btn-sm" (click)="cancelImport()">Cancel</button>
        <button type="button" class="btn btn-primary btn-sm ml-3 mr-5" (click)="proceedImport()">Proceed</button>
    </div>
  `,
})
export class ImportPreviewDialogComponent {

  dummyData = [
    {
      name: "Bank 1",
      status: "success",
      operation: "insert",
      changes: [
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
      ],
    },
    {
      name: "Bank 2",
      status: "failed",
      operation: "edit",
      changes: [
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
      ],
    },
    {
      name: "Bank 3",
      status: "success",
      operation: "insert",
      changes: [
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
      ],
    },
    {
      name: "Bank 4",
      status: "failed",
      operation: "insert",
      changes: [
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
      ],
    },
    {
      name: "Bank 5",
      status: "success",
      operation: "insert",
      changes: [
        {
          itemColumnName: "Description",
          previousValue: "",
          newValue: "Sample Description",
        },
      ],
    },
  ];
  constructor(
    public dialogRef: MatDialogRef<ImportPreviewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOninit() {
    console.log(this.dummyData);
  }
  proceedImport() {
    this.dialogRef.close(true);
  }

  cancelImport() {
    this.dialogRef.close(false);
  }
}      