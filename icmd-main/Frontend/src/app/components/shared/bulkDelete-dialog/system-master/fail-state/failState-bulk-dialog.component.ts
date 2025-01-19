import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';


@Component({
  selector: 'app-failstate-bulk-dialog',
  standalone: true,
  imports: [MatDialogModule, MatTableModule],
  template: `
  
  <h1 class="text-center fs-8 fw-bold" mat-dialog-title>Confirm Bulk Deletion</h1>

  <div mat-dialog-content>
    <p class="text-center fs-5">Are you sure you want to delete the following?</p>
    
    <div class="table-responsive">
      <table mat-table [dataSource]="data" class="table-hover mat-elevation-z8">

        <!-- State Column -->
        <ng-container matColumnDef="state">
          <th mat-header-cell *matHeaderCellDef> Fail State Name </th>
          <td mat-cell *matCellDef="let state"> {{state.failStateName}}</td>
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
export class FailStateBulkDialogComponent {

  displayedColumns: string[] = ['state'];

  constructor(
    public dialogRef: MatDialogRef<FailStateBulkDialogComponent>,
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


