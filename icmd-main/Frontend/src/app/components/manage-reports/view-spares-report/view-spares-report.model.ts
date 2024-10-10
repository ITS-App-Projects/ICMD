export interface SparesReportResponceDtoModel {
    totalChanneles: number | null;
    usedChanneles: number | null;
    rack: string | null;
    plcNumber: string | null;
    natureOfSignal: string | null;
    spareChannels: number | null;
    percentUsed: number | null;
    percentSpare: number | null;
    childItems: SparesReportResponceDtoModel[];
    expanded: boolean;
}