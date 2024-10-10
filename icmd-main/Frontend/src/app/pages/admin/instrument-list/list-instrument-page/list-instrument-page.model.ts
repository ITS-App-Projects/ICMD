import { SearchType } from "@e/common"
import { DropdownInfoDtoModel } from "@m/common";

export interface SearchInstrumentFilterModel {
    tag: string,
    tagSearchType: SearchType,
    plcNo: string,
    plcNoSearchType: SearchType,
    equipmentCode: string,
    equipmentCodeSearchType: SearchType,
    process: string,
    processSearchType: SearchType,
    manufacturer: string,
    manufacturerSearchType: SearchType,
    natureOfSignal: string,
    natureOfSignalSearchType: SearchType,
    zone: string
    zoneSearchType: SearchType,
    type: number;
}

export interface InstrumentDropdownInfoDtoModel {
    equipmentCodeList: DropdownInfoDtoModel[] | null;
    manufacturerList: DropdownInfoDtoModel[] | null;
    processList: DropdownInfoDtoModel[] | null;
    zoneList: DropdownInfoDtoModel[] | null;
    tagList: DropdownInfoDtoModel[] | null;
    natureOfSignalList: DropdownInfoDtoModel[] | null;
    plcNumberList: DropdownInfoDtoModel[] | null;
}