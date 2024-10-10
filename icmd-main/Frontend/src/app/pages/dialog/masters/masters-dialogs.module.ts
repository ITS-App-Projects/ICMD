import { NgModule } from "@angular/core";
import { BankDialogsModule } from "./bank/bank-dialogs.module";
import { WorkAreaPackDialogsModule } from "./work-area-pack/work-area-pack-dialogs.module";
import { TrainDialogsModule } from "./train/train-dialogs.module";
import { ZoneDialogsModule } from "./zone/zone-dialogs.module";
import { SystemDialogsModule } from "./system/system-dialogs.module";
import { DocumentTypeDialogsModule } from "./documentType/documentType-dialogs.module";
import { SubSystemDialogsModule } from "./sub-system/sub-system-dialogs.module";
import { ProcessDialogsModule } from "./process/process-dialogs.module";
import { SubProcessDialogsModule } from "./sub-process/sub-process-dialogs.module";
import { StreamDialogsModule } from "./stream/stream-dialogs.module";
import { ReferenceDocumentDialogsModule } from "./reference-document/reference-document-dialogs.module";
import { EquipmentCodeDialogsModule } from "./equipmentCode/equipmentCode-dialogs.module";
import { ManufacturerDialogsModule } from "./manufacturer/manufacturer-dialogs.module";
import { FailStateDialogsModule } from "./failState/failState-dialogs.module";
import { TagTypeDialogsModule } from "./tagType/tagType-dialogs.module";
import { TagDescriptorDialogsModule } from "./tagDescriptor/tagDescriptor-dialogs.module";
import { DeviceModelDialogsModule } from "./device-model/device-model-dialogs.module";
import { DeviceTypeDialogsModule } from "./device-type/device-type-dialogs.module";
import { NatureOfSignalDialogsModule } from "./natureOfSignal/natureOfSignal-dialogs.module";
import { TagDialogsModule } from "./tag/tag-dialogs.module";
import { JunctionBoxDialogsModule } from "./junction-box/junction-box-dialogs.module";
import { PanelDialogsModule } from "./panel/panel-dialogs.module";
import { SkidDialogsModule } from "./skid/skid-dialogs.module";
import { StandDialogsModule } from "./stand/stand-dialogs.module";

@NgModule({
    imports: [BankDialogsModule, WorkAreaPackDialogsModule, TrainDialogsModule, ZoneDialogsModule, SystemDialogsModule,
        DocumentTypeDialogsModule, SubSystemDialogsModule, ProcessDialogsModule, SubProcessDialogsModule, StreamDialogsModule, ReferenceDocumentDialogsModule,
        EquipmentCodeDialogsModule, ManufacturerDialogsModule, FailStateDialogsModule, TagTypeDialogsModule, TagDescriptorDialogsModule,
        DeviceModelDialogsModule, DeviceTypeDialogsModule, NatureOfSignalDialogsModule, TagDialogsModule, JunctionBoxDialogsModule,
        PanelDialogsModule, SkidDialogsModule, StandDialogsModule],
    exports: [],
    declarations: [],
    providers: [],
})
export class MastersDialogsModule { }
