import { SearchType } from "@e/common"
import { DropdownInfoDtoModel } from "@m/common";

export interface SearchNonInstrumentFilterModel {
    tag: string,
    tagSearchType: SearchType,
    plcNo: string,
    plcNoSearchType: SearchType,
    equipmentCode: string,
    equipmentCodeSearchType: SearchType,
    location: string;
    locationSearchType: SearchType;
    type: number;
}

export interface NonInstrumentDropdownInfoDtoModel {
    tagList: DropdownInfoDtoModel[] | null;
    plcNumberList: DropdownInfoDtoModel[] | null;
    equipmentCodeList: DropdownInfoDtoModel[] | null;
    locationList: DropdownInfoDtoModel[] | null;
}