import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { BulkDeleteService } from 'src/app/service/instrument/bulkDelete/bulk-delete.service';


@Component({
  selector: 'app-stand-bulk-dialog',
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
          <td mat-cell *matCellDef="let jb"> {{jb.tag}}</td>
        </ng-container>

        <!-- Process No Column -->
        <ng-container matColumnDef="process">
          <th mat-header-cell *matHeaderCellDef> Process No </th>
          <td mat-cell *matCellDef="let jb"> {{jb.process}}</td>
        </ng-container>

        <!-- Sub Process Column -->
        <ng-container matColumnDef="sub">
          <th mat-header-cell *matHeaderCellDef> Sub Process </th>
          <td mat-cell *matCellDef="let jb"> {{jb.subProcess}}</td>
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
export class StandBulkDialogComponent {

  displayedColumns: string[] = ['tag','process', 'sub'];

  constructor(
    public dialogRef: MatDialogRef<StandBulkDialogComponent>,
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


