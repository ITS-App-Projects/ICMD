export interface ReportListDtoModel {
    group: string | null;
    items: ReportInfoDtoModel[] | null;
}

export interface ReportInfoDtoModel {
    group: string;
    name: string;
    url: string;
    description: string | null;
    order: number | null;
}