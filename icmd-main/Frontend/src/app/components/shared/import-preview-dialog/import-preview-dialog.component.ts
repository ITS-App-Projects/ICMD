import { AfterViewInit, Component, Inject, ElementRef } from "@angular/core";
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
      <h2 class="mx-5 my-4"> Changes Preview <b>( {{ data.length }} )</b> </h2>

      <ng-scrollbar style="height:600px" >
      <mat-accordion>
        <mat-expansion-panel *ngFor="let item of data" class="border border-light mt-2 mx-5 rounded">
          <mat-expansion-panel-header>
            <mat-panel-title>
                <h4 class="card-label pt-1" 
                    [attr.title]="item.name" 
                    data-bs-toggle="tooltip" 
                    data-bs-placement="top"> 
                    {{ item.name.length > 15 ? (item.name | slice:0:15) + '...' : item.name }} 
                </h4>
            </mat-panel-title>
            <mat-panel-description>
                <span [ngClass]="item.status === 'Success' ? 'text-success text-center h6' : 'text-danger h6'">
                    {{ item.status | capitalize }} 
                    
                    @if (item.message) {
                      <span> - {{item.message}} </span>
                    }
                </span>
            </mat-panel-description>
          </mat-expansion-panel-header>

          @if (item.operation) {
            <div class="text-start h6 my-3">
            Operation: {{ item.operation | capitalize }}
            </div>
          }

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
      </ng-scrollbar>
    </div>
    
    <div class="d-flex justify-content-end my-5">
        <button type="button" class="btn btn-outline-secondary" (click)="cancelImport()">Cancel</button>
        <button type="button" class="btn btn-primary ml-3 mr-5" (click)="proceedImport()">Proceed</button>
    </div>
  `,
})
export class ImportPreviewDialogComponent implements AfterViewInit {

  constructor(
    public dialogRef: MatDialogRef<ImportPreviewDialogComponent>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngAfterViewInit() {
    import('bootstrap').then((bootstrap) => {
      const tooltipTriggerList = this.el.nativeElement.querySelectorAll('[data-bs-toggle="tooltip"]');
      tooltipTriggerList.forEach((tooltip) => new bootstrap.Tooltip(tooltip));
    });
  }

  proceedImport() {
    this.dialogRef.close(true);
  }

  cancelImport() {
    this.dialogRef.close(false);
  }
}      