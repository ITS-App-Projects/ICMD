export interface DPPADevicesDtoModel {
    plcNumber: string | null;
    plcSlotNumber: string | null;
    dppaCoupler: string | null;
    afdHubNumber: string | null;
    noOfDPDevices: number | null;
    noOfPADevices: number | null;
    childInfo: DPPADevicesDtoModel[];
    expanded: boolean;
}