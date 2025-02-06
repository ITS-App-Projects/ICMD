export interface ImportFileResultModel<T> {
    records: (T & ImportFileResponseModel)[];
    headers: string[] | null;
    message: string;
    isSucceeded: boolean;
    isWarning: boolean | null;
}

export interface ImportFileResponseModel {
    status: string | null;
    message: string | null;
}