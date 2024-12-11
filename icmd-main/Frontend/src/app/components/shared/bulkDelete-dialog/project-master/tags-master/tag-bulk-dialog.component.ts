import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';


@Component({
  selector: 'app-tag-bulk-dialog',
  standalone: true,
  imports: [MatDialogModule, MatTableModule],
  template: `
  
  <h1 class="text-center fs-8 fw-bold" mat-dialog-title>Confirm Bulk Deletion</h1>

  <div mat-dialog-content>
    <p class="text-center fs-5">Are you sure you want to delete the following?</p>
    
    <div class="table-responsive">
      <table mat-table [dataSource]="data" class="table-hover mat-elevation-z8">

        <!-- Tag Column -->
        <ng-container matColumnDef="tag">
          <th mat-header-cell *matHeaderCellDef> Tag </th>
          <td mat-cell *matCellDef="let tag"> {{tag.tag}}</td>
        </ng-container>

        <!-- Location Column -->
        <ng-container matColumnDef="loc">
          <th mat-header-cell *matHeaderCellDef> Location </th>
          <td mat-cell *matCellDef="let tag"> {{tag.field1String}}</td>
        </ng-container>

        <!-- Sub Location Column -->
        <ng-container matColumnDef="sub-loc">
          <th mat-header-cell *matHeaderCellDef> Sub Location </th>
          <td mat-cell *matCellDef="let tag"> {{tag.field2String}}</td>
        </ng-container>

        <!-- Room Column -->
        <ng-container matColumnDef="room">
          <th mat-header-cell *matHeaderCellDef> Room </th>
          <td mat-cell *matCellDef="let tag"> {{tag.field3String}}</td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
    </div>   
   </div>
  
  <div class="d-flex justify-content-center" mat-dialog-actions>
      <button class="btn btn-outline-secondary btn-sm mr-2" mat-button (click)="onCancel()">Cancel</button>
      <button class="btn btn-danger btn-sm" mat-raised-button color="warn" (click)="onConfirm()">Delete</button>
  </div>

  
  `,
})
export class TagBulkDialogComponent {

  displayedColumns: string[] = ['tag','loc', 'sub-loc', 'room'];

  constructor(
    public dialogRef: MatDialogRef<TagBulkDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any[],
    private bulkDeleteService: BulkDeleteService
  ) {}

  onCancel(): void {
    this.dialogRef.close(null);
    this.bulkDeleteService.cancelBulkDelete(); 
  }

  onConfirm(): void {
    const ids = this.data.map(bank => bank.id);
    this.dialogRef.close(ids);

    this.bulkDeleteService.cancelBulkDelete();
  }

}


