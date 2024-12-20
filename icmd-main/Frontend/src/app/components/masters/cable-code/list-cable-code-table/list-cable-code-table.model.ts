export interface CableCodeListDtoModel {
    id: string;  
    projectId: string | null;

    cableCode: string | null;
    fr: string | null;
    type: string | null;
    subType: string | null;
    size: string | null;
    core: string | null;
    coreMaterial: string | null;

    screen: string | null;
    rating: string | null;
    innerSheath: string | null;
    outerSheath: string | null;
    other: string | null;
    sheathColour: string | null;
    internalCoreColour: string | null;
    cableDescription: string | null;
    overallDiameter: string | null;
    weight: string | null;
    comment: string | null;
} 
