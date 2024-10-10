export interface SparesReportDetailsResponceDtoModel {
    plcNumber: string | null;
    rack: string | null;
    natureOfSignal: string | null;
    slotNumber: number | null;
    totalChanneles: number | null;
    usedChanneles: number;
    spareChannels: number | null;
    percentUsed: number | null;
    percentSpare: number | null;
    childItems: SparesReportDetailsResponceDtoModel[];
    expanded: boolean;
}