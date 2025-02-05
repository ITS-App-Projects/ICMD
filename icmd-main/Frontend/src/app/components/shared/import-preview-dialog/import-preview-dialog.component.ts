import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatButtonModule } from "@angular/material/button";
import { MatDialog, MatDialogModule } from "@angular/material/dialog";

@Component({
    standalone: true,
    selector: "app-import-preview-dialog",
    imports: [ MatButtonModule, MatDialogModule ],
    template: `
    
    <h2 mat-dialog-title>Changes Preview</h2>

    <mat-dialog-content class="mat-typography"></mat-dialog-content>

    <mat-dialog-actions >
        <button mat-button (click)="cancelImport()">Cancel</button>
        <button mat-button color="primary" (click)="proceedImport()">Proceed</button>
    </mat-dialog-actions>
    
    `,
})
export class ImportPreviewDialogComponent {
    
    constructor(
        public dialogRef: MatDialogRef<ImportPreviewDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) {}


    proceedImport() {
        this.dialogRef.close(true);
    }

    cancelImport() {
        this.dialogRef.close(false);
    }
}