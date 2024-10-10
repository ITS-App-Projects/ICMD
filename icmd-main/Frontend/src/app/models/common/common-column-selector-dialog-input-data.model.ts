export interface CommonColumnSelectorDialogInputDataModel {
    itemList: {
        key: string;
        label: string;
    }[],
    listName: string
}

export interface CommonColumnSelectorDialogOutputDataModel {
    selectedColumns: string[],
    success: boolean
}