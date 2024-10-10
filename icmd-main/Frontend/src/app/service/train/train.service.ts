import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CreateOrEditTrainDtoModel } from "@c/masters/train/create-edit-train-form";
import { TrainInfoDtoModel } from "@c/masters/train/list-train-table";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";
import { Observable } from "rxjs";

@Injectable()
export class TrainService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<TrainInfoDtoModel>> {
        return this._http.post<PagedResultModel<TrainInfoDtoModel>>(
            `${environment.apiUrl}Train/GetAllTrains`,
            request
        );
    }

    public getTrainInfo(id: string): Observable<TrainInfoDtoModel> {
        return this._http.get<TrainInfoDtoModel>(
            `${environment.apiUrl}Train/GetTrainInfo?id=${id}`
        );
    }

    public createEditTrain(info: CreateOrEditTrainDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}Train/CreateOrEditTrain`,
            info
        );
    }

    public deleteTrain(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}Train/DeleteTrain?id=${id}`
        );
    }

    public importTrain(projectId: string, file: File): Observable<ImportFileResultModel<TrainInfoDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<TrainInfoDtoModel>>(
            `${environment.apiUrl}Train/ImportTrain`,
            formData
        );
    }
}