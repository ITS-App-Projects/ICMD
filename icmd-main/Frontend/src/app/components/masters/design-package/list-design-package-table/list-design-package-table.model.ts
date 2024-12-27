export interface DesignPackageListDtoModel {
    id: string;  
    projectId: string | null;

   designPackage: string | null;
   designPackageName: string | null;
   designVendor: string | null;
   designLead: string | null;
   leadName: string | null;
   packageStatus: string | null;
} 
