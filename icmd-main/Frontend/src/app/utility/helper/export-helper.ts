import {
  download,
  generateCsv,
  mkConfig
} from 'export-to-csv';
import * as XLSX from 'xlsx';

import { ImportFileResponseModel } from '@m/common';

import { importFileResponseCommonColumns } from '../';
import { CommonFunctions } from './';

export class ExcelHelper {
    public exportExcel(data: any[], mapping: any, fileName: string) {
        const modifiedData = this.modifyColumnNames(data, mapping);
        this.exportDataToExcel(modifiedData, fileName);
    }

    private modifyColumnNames(data: any[], mapping: any): any[] {
        return data.map(item => {
            const modifiedItem: any = {};
            for (const key of Object.keys(item)) {
                const newKey = mapping[key] || key;
                if (mapping[key]) {
                    modifiedItem[newKey] = item[key];
                }
            }
            return modifiedItem;
        });
    }

    private exportDataToExcel(data: any[], fileName: string) {
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

        XLSX.writeFile(wb, `${fileName}.xlsx`);
    }

    public downloadImportResponseFile<T>(fileName: string, records: (T & ImportFileResponseModel)[], fieldNames: string[], isTags: boolean = false) {
        fieldNames = [...fieldNames, ...importFileResponseCommonColumns];

        const _commonFunctions = new CommonFunctions();

        let transformedRecords: any[];
        if (!isTags) {
            transformedRecords = records.map(record => {
                const transformedRecord: Partial<T> = {};
                fieldNames.forEach(fieldName => {
                    transformedRecord[fieldName] = record[_commonFunctions.toCamelCase(fieldName)];
                });
                return transformedRecord;
            });
        } else {
            transformedRecords = [...records];
        }

        const csvConfig = mkConfig({ filename: `${fileName}_Import_Result`, columnHeaders: fieldNames });
        const csv = generateCsv(csvConfig)(transformedRecords);
        download(csvConfig)(csv);
    }
}