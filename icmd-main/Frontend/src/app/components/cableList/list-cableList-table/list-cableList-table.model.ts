export interface ViewCableListDtoModel {
    cableIdAssetName: string | null;  
    assetDiscipline: string |null; 
    assetType: string | null; 
    assetDescription: string | null; 
    
    originZone: string | null;
    cableOriginArea: string | null;
    cableOriginRoomTag: string | null; 
    cableOriginRoomDescription: string | null; 
    cableOriginDevice: string | null;
    cableOriginCircuitBreaker: string | null;
    destinationZone: string | null; 
    cableDestinationArea: string | null;
    cableDestinationRoomTag: string | null; 
    cableDestinationRoomDescription: string | null; 
    cableDestinationDevice: string | null;
    cableDestinationCircuitBreaker: string | null;

    cableRouteLength: string | null; // Manual Type
    cableTotalCableLength: string | null; // Manual Type

    cableCategory: string | null; 
    cableType: string | null; 

    cableCode: string | null; 
    cableClass: string | null; 
    cableVoltage: string | null // Manual Type
    cableConductorSize: string | null; 
    cableNumber: string | null; // Manual Type
    noOfCores: string | null; 
    coreMaterial: string | null; 
    insulation: string | null; 
    internalCoreInsulation: string | null; 
    internalCoreInsulationColour: string | null; 
    externalSheathInsulation: string | null; 
    externalSheathColour: string | null; 
    fireRating: string | null; 
    other: string | null;
    cableDescription: string | null; 
    cableDrumNo: string | null; //  Manual Type
    cableOverallDiameter: string | null;
    cableWeight: string | null; 
    cableSerialNo: String | null; // Manual Type
    cableRevision: string | null; // Manual Type

    designPackage: string | null; 
    designPackageName: string | null; 
    designVendor: string | null; 
    designLead: string | null;
    leadName: string | null; 
    packageStatus: string | null; 

    cableScheduleDocumentNo: string | null; // Manual Type
    revision: string | null; // Manual Type
    originControlLine: string | null; // Manual Type
    originChainage: string | null; // 
    destinationControlLine: string | null; // Manual Type

    destinationChainage: string | null; // Manual Type
    identifier: string | null; // Manual Type
    identifierLabel: string | null; // Manual Type
    identifierCode: string | null; // Manual Type
    existingRoadNumber: string | null; // Manual Type
}