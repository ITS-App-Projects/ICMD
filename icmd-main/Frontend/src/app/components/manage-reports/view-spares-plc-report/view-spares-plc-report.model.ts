export interface SparesReportPLCResponceDtoModel {
    plcNumber: string | null;
    natureOfSignal: string | null;
    totalChanneles: number | null;
    usedChanneles: number;
    spareChannels: number | null;
    percentUsed: number | null;
    percentSpare: number | null;
    childItems: SparesReportPLCResponceDtoModel[];
    expanded: boolean;
}