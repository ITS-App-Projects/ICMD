import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AppRoute } from "@u/app.route";

const routes: Routes = [
    {
        path: "",
        canActivate: [],
        children: [
            {
                path: AppRoute.manageBanks,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./bank/list-bank-page/list-bank-page.module"
                    ).then((m) => m.ListBankPageModule),
            },
            {
                path: AppRoute.manageWorkArea,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./workAreaPack/list-work-area-page/list-work-area-page.module"
                    ).then((m) => m.ListWorkAreaPageModule),
            },
            {
                path: AppRoute.manageTrain,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./train/list-train-page/list-train-page.module"
                    ).then((m) => m.ListTrainPageModule),
            },
            {
                path: AppRoute.manageZone,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./zone/list-zone-page/list-zone-page.module"
                    ).then((m) => m.ListZonePageModule),
            },
            {
                path: AppRoute.manageSystem,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./system/list-system-page/list-system-page.module"
                    ).then((m) => m.ListSystemPageModule),
            },
            {
                path: AppRoute.manageSubSystem,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./sub-system/list-sub-system-page/list-sub-system-page.module"
                    ).then((m) => m.ListSubSystemPageModule),
            },
            {
                path: AppRoute.manageDocumentType,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./documentType/list-document-type-page/list-document-type-page.module"
                    ).then((m) => m.ListDocumentTypePageModule),
            },
            {
                path: AppRoute.manageProcess,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./process/list-process-page/list-process-page.module"
                    ).then((m) => m.ListProcessPageModule),
            },
            {
                path: AppRoute.manageSubProcess,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./sub-process/list-sub-process-page/list-sub-process-page.module"
                    ).then((m) => m.ListSubProcessPageModule),
            },
            {
                path: AppRoute.manageStream,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./stream/list-stream-page/list-stream-page.module"
                    ).then((m) => m.ListStreamPageModule),
            },
            {
                path: AppRoute.manageReferenceDocument,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./reference-document/list-reference-document-page/list-reference-document-page.module"
                    ).then((m) => m.ListReferenceDocumentPageModule),
            },
            {
                path: AppRoute.manageEquipmentCode,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./equipmentCode/list-equipmentCode-page/list-equipmentCode-page.module"
                    ).then((m) => m.ListEquipmentCodePageModule),
            },
            {
                path: AppRoute.manageManufacturer,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./manufacturer/list-manufacturer-page/list-manufacturer-page.module"
                    ).then((m) => m.ListManufacturerPageModule),
            },
            {
                path: AppRoute.manageFailState,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./failState/list-failState-page/list-failState-page.module"
                    ).then((m) => m.ListFailStatePageModule),
            },
            {
                path: AppRoute.manageTagTypes,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./tagType/list-tagType-page/list-tagType-page.module"
                    ).then((m) => m.ListTagTypePageModule),
            },
            {
                path: AppRoute.manageTagDescriptors,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./tagDescriptor/list-tagDescriptor-page/list-tagDescriptor-page.module"
                    ).then((m) => m.ListTagDescriptorPageModule),
            },
            {
                path: AppRoute.manageDeviceModel,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./device-model/list-device-model-page/list-device-model-page.module"
                    ).then((m) => m.ListDeviceModelPageModule),
            },
            {
                path: AppRoute.manageDeviceType,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./device-type/list-device-type-page/list-device-type-page.module"
                    ).then((m) => m.ListDeviceTypePageModule),
            },
            {
                path: AppRoute.manageNatureOfSignal,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./natureOfSignal/list-natureOfSignal-page/list-natureOfSignal-page.module"
                    ).then((m) => m.ListNatureOfSignalPageModule),
            },
            {
                path: AppRoute.manageTagFields,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./tag-Fields/list-tag-Fields-page/list-tag-Fields-page.module"
                    ).then((m) => m.ListTagFieldsPageModule),
            },
            {
                path: AppRoute.manageTag,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./tag/list-tag-page/list-tag-page.module"
                    ).then((m) => m.ListTagPageModule),
            },
            {
                path: AppRoute.manageJunctionBox,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./junction-box/list-junction-box-page/list-junction-box-page.module"
                    ).then((m) => m.ListJunctionBoxPageModule),
            },
            {
                path: AppRoute.managePanel,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./panel/list-panel-page/list-panel-page.module"
                    ).then((m) => m.ListPanelPageModule),
            },
            {
                path: AppRoute.manageSkid,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./skid/list-skid-page/list-skid-page.module"
                    ).then((m) => m.ListSkidPageModule),
            },
            {
                path: AppRoute.manageStand,
                pathMatch: "full",
                canActivate: [],
                loadChildren: () =>
                    import(
                        "./stand/list-stand-page/list-stand-page.module"
                    ).then((m) => m.ListStandPageModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [],
    providers: [],
})
export class MastersRoutingModule { }