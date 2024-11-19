import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-bulk-delete-dialog',
  standalone: true,
  imports: [MatDialogModule, MatTableModule],
  template: `
  
  <h1 mat-dialog-title>Confirm Bulk Deletion</h1>

  <div mat-dialog-content>
    <p>Are you sure you want to delete the following devices?</p>
    
    <mat-table [dataSource]="data">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef> Name </th>
        <td mat-cell *matCellDef="let device"> {{ device.name }} </td>
      </ng-container>

      <ng-container matColumnDef="id">
        <th mat-header-cell *matHeaderCellDef> ID </th>
        <td mat-cell *matCellDef="let device"> {{ device.deviceId }} </td>
      </ng-container>

      <ng-container matColumnDef="type">
        <th mat-header-cell *matHeaderCellDef> Type </th>
        <td mat-cell *matCellDef="let device"> {{ device.type }} </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </mat-table>
   </div>
  
  <div mat-dialog-actions>
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="warn" (click)="onConfirm()">Delete</button>
  </div>

  
  `,
})
export class BulkDeleteDialogComponent {

  displayedColumns: string[] = ['id', 'name', 'type'];

  constructor(
    public dialogRef: MatDialogRef<BulkDeleteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any[]
  ) {}

  onCancel(): void {
    this.dialogRef.close(null); 
  }

  onConfirm(): void {
    const ids = this.data.map(device => device.deviceId);
    console.log(ids);
    this.dialogRef.close(ids); 
  }

}
