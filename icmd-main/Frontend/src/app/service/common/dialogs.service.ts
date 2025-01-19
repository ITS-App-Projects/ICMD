import { Injectable, TemplateRef } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmDialogComponent, ConfirmDialogInputDataModel, ConfirmDialogOutputDataModel } from "@p/dialog/common";
import { ComponentType } from "ngx-toastr";
import { map } from "rxjs/operators";

@Injectable({ providedIn: 'root' })
export class DialogsService {
    constructor(private _dialog: MatDialog) { }

    openDialog<TDialog, TDialogInput, TDialogOutput, TOutput>(
        componentOrTemplateRef: ComponentType<TDialog> | TemplateRef<TDialog>,
        data: TDialogInput,
        mapper: (value: TDialogOutput) => TOutput,
        minWidth = 320,
        minHeight?: number,
        closeOnNavigation = true,
        disableClose = true,
        customPanelClasses: ReadonlyArray<string> = []
    ): Promise<TOutput> {
        return this._dialog
            .open<TDialog, TDialogInput, TDialogOutput>(componentOrTemplateRef, {
                data: data,
                closeOnNavigation,
                disableClose,
                minWidth,
                minHeight,
                maxWidth: '100vw',
                maxHeight: '100vh',
                panelClass: ['toms-dialog', ...customPanelClasses],
            })
            .afterClosed()
            .pipe(map((value) => mapper(value)))
            .toPromise();
    }

    customConfirm(data?: Partial<ConfirmDialogInputDataModel>): Promise<boolean> {
        return this.openDialog<ConfirmDialogComponent, Partial<ConfirmDialogInputDataModel>, ConfirmDialogOutputDataModel, boolean>(
            ConfirmDialogComponent,
            data || {},
            (value) => value.confirm
        );
    }

    confirm(text?: string, header?: string): Promise<boolean> {
        return this.customConfirm(text ? { confirmationText: text, header: header || 'Confirm' } : { header: header || 'Confirm' });
    }
    confirmBulk(text?: string, header?: string): Promise<boolean> {
        return this.customConfirm(text ? { confirmationText: text, header: header || 'Confirm' } : { header: header || 'Confirm' });
    }

    info(text: string, header?: string): Promise<boolean> {
        return this.customConfirm({ confirmationText: text, icon: 'info', showCancel: false, header: header || 'Info' });
    }

    warning(text: string, header?: string): Promise<boolean> {
        return this.customConfirm({
            confirmationText: text,
            icon: 'report_problem',
            showCancel: false,
            header: header || 'Warning',
        });
    }

    closeAllDialog(){
        this._dialog.closeAll();
    }
}