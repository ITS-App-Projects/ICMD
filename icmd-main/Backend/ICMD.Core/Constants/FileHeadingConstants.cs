namespace ICMD.Core.Constants
{
    public class FileHeadingConstants
    {
        public static string IdHeading = "Id";

        public static List<string> OMItemsHeadings = ["ItemId", "ItemDesc", "ParentItemId", "AssetTypeId"];

        public static List<string> OMServiceDescriptionHeadings = [ "Tag", "Service Description", "Area",
                                                                                        "Stream", "Bank", "Service", "Variable",
                                                                                        "Train" ];


        public static List<string> CCMDHeadings = ["TagNo", "PLCNumber", "WAP", "SystemCode", "SubsystemCode"];

        public static List<string> EquipmentListHeadings = ["PnPId", "Process Number", "Sub Process", "Stream",
                                                                               "Equipment Code", "Sequence Number", "Equipment Identifier",
                                                                               "Tag", "DWG Title", "REV", "VERSION", "Description",
                                                                               "Piping Class", "On Skid", "Function",   "Tracking Number"];

        public static List<string> InstrumentListHeadings = [ "PnPId", "Process Number", "Sub Process", "Stream",
                                                                                 "Equipment Code", "Sequence Number", "Equipment Identifier",
                                                                                 "Tag", "On Equipment", "On Skid", "Description", "FluidCode",
                                                                                 "PipeLines.Tag", "Size", "DWG Title", "REV", "VERSION", "To",
                                                                                 "From", "Tracking Number" ];

        public static List<string> ValveListHeadings = [ "PnPId", "Process Number", "Sub Process", "Stream",
                                                                            "Equipment Code", "Sequence Number", "Equipment Identifier", "Tag",
                                                                            "DWG Title", "REV", "VERSION", "Description", "Size", "FluidCode",
                                                                            "PipeLines.Tag", "Piping Class", "Class Name", "On Skid", "Failure",
                                                                            "Switches", "From", "To", "Accessories", "Design Temp", "Nominal Pressure",
                                                                            "Valve Spec Number", "PN Rating", "Tracking Number" ];

        public static List<string> BankListHeadings = ["Bank"];

        public static List<string> WorkAreaPackHeadings = ["Number", "Description"];

        public static List<string> TrainHeadings = ["Train"];

        public static List<string> ZoneHeadings = ["Zone", "Description", "Area"];

        public static List<string> SystemHeadings = ["Number", "Description", "Work Area Pack"];

        public static List<string> SubSystemHeadings = ["System", "Number", "Description"];
        public static List<string> SubSystemExportHeadings = ["Work Area Pack", "System", "Number", "Description"];

        public static List<string> TagField1Headings = ["Process Name", "Description"];

        public static List<string> TagField2Headings = ["Sub-Process Name", "Description"];

        public static List<string> TagField3Headings = ["Stream Name", "Description"];

        public static List<string> ReferenceDocumentHeadings = ["Document Number", "Reference Document Type", "URL", "Description", "Version", "Revision", "Date (MM/dd/yyyy)", "Sheet"];
        public static List<string> ReferenceDocumentExportHeadings = ["Document No", "Type", "URL", "Description", "Version", "Revision", "Date", "Sheet"];

        public static List<string> TagHeadings = ["Field1Id", "Field1String", "Field2Id", "Field2String", "Field3Id", "Field3String", "Field4Id", "Field4String", "Field5Id", "Field5String", "Field6Id", "Field6String", "TagName"];

        public static List<string> JunctionBoxHeadings = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];
        public static List<string> JunctionBoxExportHeadings = ["Tag", "Process No", "Sub Process", "Stream", "Equipment Code", "Sequence Number", "Equipment Identifier", "Type", "Description", "Reference Document Type", "Reference Document"];

        public static List<string> PanelHeadings = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];
        public static List<string> PanelExportHeadings = ["Tag", "Process No", "Sub Process", "Stream", "Equipment Code", "Sequence Number", "Equipment Identifier", "Type", "Description", "Reference Document Type", "Reference Document"];

        public static List<string> SkidHeadings = ["Tag", "Type", "Description", "Reference Document Type", "Document Number"];
        public static List<string> SkidExportHeadings = ["Tag", "Process No", "Sub Process", "Stream", "Equipment Code", "Sequence Number", "Equipment Identifier", "Type", "Description", "Reference Document Type", "Reference Document"];

        public static List<string> StandHeadings = ["Tag", "Description", "Type", "Area", "Reference Document Type", "Document Number"];
        public static List<string> StandExportHeadings = ["Tag", "Process No", "Sub Process", "Stream", "Equipment Code", "Sequence Number", "Equipment Identifier", "Description", "Type", "Area", "Reference Document Type", "Reference Document"];

        public static List<string> ReferenceDocumentTypeHeadings = ["Type"];

        public static List<string> EquipmentCodeHeadings = ["Code", "Descriptor"];

        public static List<string> FailStateHeadings = ["Fail State Name"];

        public static List<string> TagTypeHeadings = ["Name", "Description"];

        public static List<string> TagDescriptorHeadings = ["Name", "Description"];

        public static List<string> ManufacturerHeadings = ["Name", "Description", "Comment"];

        public static List<string> DeviceModelHeadings = ["Model", "Description", "Manufacturer"];

        public static List<string> DeviceTypeHeadings = ["Type", "Description"];

        public static List<string> NatureOfSignalTypeHeadings = ["Name"];
    }
}
