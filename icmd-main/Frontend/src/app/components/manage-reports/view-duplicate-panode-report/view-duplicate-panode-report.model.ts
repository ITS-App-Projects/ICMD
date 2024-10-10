import { ViewInstrumentListLiveDtoModel } from "../view-duplicate-dpnode-report";

export interface DuplicatePANodeAddressDtoModel {
    dppaCoupler: string | null;
    items: ViewInstrumentListLiveDtoModel[];
    expanded: boolean;
}