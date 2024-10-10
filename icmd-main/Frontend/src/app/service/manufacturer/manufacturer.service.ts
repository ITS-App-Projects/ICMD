import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditManufacturerDtoModel } from "@c/masters/manufacturer/create-edit-manufacturer-form";
import { ManufacturerInfoDtoModel } from "@c/masters/manufacturer/list-manufacturer-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { DropdownInfoDtoModel, ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class ManufacturerService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<ManufacturerInfoDtoModel>> {
        return this._http.post<PagedResultModel<ManufacturerInfoDtoModel>>(
            `${environment.apiUrl}Manufacturer/GetAllManufacturers`,
            request
        );
    }

    public getManufacturerInfo(id: string): Observable<ManufacturerInfoDtoModel> {
        return this._http.get<ManufacturerInfoDtoModel>(
            `${environment.apiUrl}Manufacturer/GetManufacturerInfo?id=${id}`
        );
    }

    public createEditManufacturer(info: CreateOrEditManufacturerDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Manufacturer/CreateOrEditManufacturer`,
            info
        );
    }

    public deleteManufacturer(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Manufacturer/DeleteManufacturer?id=${id}`
        );
    }

    public getAllManufacturerInfo(): Observable<DropdownInfoDtoModel[]> {
        return this._http.get<DropdownInfoDtoModel[]>(
            `${environment.apiUrl}Manufacturer/GetAllManufacturerInfo`
        );
    }

    public importManufacturer(file: File): Observable<ImportFileResultModel<ManufacturerInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);

        return this._http.post<ImportFileResultModel<ManufacturerInfoDtoModel>>(
            `${environment.apiUrl}Manufacturer/ImportManufacturer`,
            formData
        );
    }
}