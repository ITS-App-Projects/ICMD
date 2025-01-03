import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JunctionBoxListDtoModel } from "@c/masters/junction-box/list-junction-box-table";
import { Observable } from "rxjs";
import { environment } from "@env/environment";
import { BaseResponseModel } from "@m/auth/login-response-model";
import { ImportFileResultModel, PagedAndSortedResultRequestModel, PagedResultModel } from "@m/common";

import { RoomListDtoModel } from "@c/masters/room/list-room-table";
import { CreateOrEditRoomDtoModel } from "@c/masters/room/create-edit-room-form/create-edit-room-form.model";

@Injectable()
export class RoomService {
    constructor(private _http: HttpClient) { }

    public getAll(
        request: PagedAndSortedResultRequestModel
    ): Observable<PagedResultModel<RoomListDtoModel>> {
        return this._http.post<PagedResultModel<RoomListDtoModel>>(
            `${environment.apiUrl}CableAssetType/GetAllCableAssetTypes`,
            request
        );
    }

    public getRoomInfo(id: string): Observable<CreateOrEditRoomDtoModel> {
        return this._http.get<CreateOrEditRoomDtoModel>(
            `${environment.apiUrl}CableAssetType/GetCableAssetTypeInfo?id=${id}`
        );
    }

    public createEditRoom(info: CreateOrEditRoomDtoModel): Observable<BaseResponseModel> {
        return this._http.post<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/CreateOrEditCableAsset`,
            info
        );
    }

    public deleteRoom(id: string): Observable<BaseResponseModel> {
        return this._http.get<BaseResponseModel>(
            `${environment.apiUrl}CableAssetType/DeleteCableAssetType?id=${id}`
        );
    }

    public importRoom(projectId: string, file: File): Observable<ImportFileResultModel<JunctionBoxListDtoModel>> {
        const formData: FormData = new FormData();
        formData.append('file', file);
        formData.append('projectId', projectId);

        return this._http.post<ImportFileResultModel<JunctionBoxListDtoModel>>(
            `${environment.apiUrl}CableAssetType/ImportCableAssetType`,
            formData
        );
    }
}