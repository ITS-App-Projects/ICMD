import { ViewInstrumentListLiveDtoModel } from "../view-duplicate-dpnode-report";

export interface DuplicateRackSlotChannelDtoModel {
    rackNo: string | null;
    items: ViewInstrumentListLiveDtoModel[];
    expanded: boolean;
}