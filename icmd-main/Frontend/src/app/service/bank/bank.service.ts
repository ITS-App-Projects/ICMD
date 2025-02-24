import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditBankDtoModel } from "@c/masters/bank/create-edit-bank-form";
import { BankInfoDtoModel } from "@c/masters/bank/list-bank-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class BankService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<BankInfoDtoModel>> {
        return this._http.post<PagedResultModel<BankInfoDtoModel>>(
            `${environment.apiUrl}Bank/GetAllBanks`,
            request
        );
    }

    public getBankInfo(id: string): Observable<BankInfoDtoModel> {
        return this._http.get<BankInfoDtoModel>(
            `${environment.apiUrl}Bank/GetBankInfo?id=${id}`
        );
    }

    public createEditBank(info: CreateOrEditBankDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Bank/CreateOrEditBank`,
            info
        );
    }

    public deleteBank(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Bank/DeleteBank?id=${id}`
        );
    }

    public deleteBulkBanks(ids: string[]): Observable<BaseResponseModel> {
        return this._http.delete<BaseResponseModel>(
            `${environment.apiUrl}Bank/DeleteBulkBanks`, {
                body: ids,
            }
        );
    }

    public validateImportBank(projectId: string, file: File): Observable<ImportFileResultModel<BankInfoDtoModel>> {
        const formData: FormData = new  FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<BankInfoDtoModel>> (
            `${environment.apiUrl}Bank/ValidateImportBank`,
            formData
        );
    }

    public importBank(projectId: string, file: File): Observable<ImportFileResultModel<BankInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<BankInfoDtoModel>>(
            `${environment.apiUrl}Bank/ImportBank`,
            formData
        );
    }
}