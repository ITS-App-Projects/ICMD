import { DropdownInfoDtoModel } from '@m/common';

export class CommonFunctions {
    numberOnly(event): boolean {
        const charCode = event.which ? event.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }

    isEmptyOrNull(value: number | string | null): boolean {
        return value == null || value?.toString() === "" ? true : false;
    }

    _filter(value: string, dataList: DropdownInfoDtoModel[]): DropdownInfoDtoModel[] {
        const filterValue = value.toLowerCase();
        return dataList.filter(option => option?.name?.toLowerCase().includes(filterValue));
    }

    toCamelCase(input: string): string {
        return input.replace(/-+/g,'').replace(/\s+/g, '').replace(/\s(.)/g, ($1) => $1.toUpperCase()).replace(/\s/g, '').replace(/^(.)/, ($1) => $1.toLowerCase());
    }

}