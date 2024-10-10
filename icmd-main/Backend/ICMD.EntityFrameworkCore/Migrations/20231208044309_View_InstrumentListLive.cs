using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class View_InstrumentListLive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_InstrumentListLive""
 AS
 SELECT ""View_Device_Instruments"".""Id"" AS ""DeviceId"",
    ""View_Device_Instruments"".""ProcessName"" AS ""Process No"",
    ""View_Device_Instruments"".""SubProcessName"" AS ""Sub Process"",
    ""View_Device_Instruments"".""StreamName"",
    ""View_Device_Instruments"".""EquipmentCode"" AS ""Equipment Code"",
    ""View_Device_Instruments"".""SequenceNumber"" AS ""Sequence Number"",
    ""View_Device_Instruments"".""EquipmentIdentifier"" AS ""Equipment Identifier"",
    ""View_Device_Instruments"".""TagName"",
    pt.""TagName"" AS ""Instr Parent Tag"",
    ""View_Device_Instruments"".""ServiceDescription"" AS ""Service Description"",
    ""View_Device_Instruments"".""LineVesselNumber"" AS ""Line / Vessel Number"",
    NULL::text AS ""Plant"",
    NULL::text AS ""Area"",
    ""View_Device_Instruments"".""VendorSupply"" AS ""Vendor Supply"",
    skidtag.""TagName"" AS ""Skid Number"",
    standtag.""TagName"" AS ""Stand Number"",
    ""View_Device_Instruments"".""Manufacturer"",
    ""View_Device_Instruments"".""Model"" AS ""Model Number"",
    attrs.""CalibratedRangeMin"" AS ""Calibrated Range (Min)"",
    attrs.""CalibratedRangeMax"" AS ""Calibrated Range (Max)"",
    attrs.""CalibratedRangeUnits"" AS ""CR Units"",
    attrs.""ProcessRangeMin"" AS ""Process Range (Min)"",
    attrs.""ProcessRangeMax"" AS ""Process Range (Max)"",
    attrs.""ProcessRangeUnits"" AS ""PR Units"",
    attrs.""RLPosition"" AS ""RL / Position"",
    docs.""DatasheetNumber"" AS ""Datasheet Number"",
    docs.""SheetNumber"" AS ""Sheet Number"",
    docs.""HookupDrawing"" AS ""Hook-up Drawing"",
    docs.""TerminationDiagram"" AS ""Termination Diagram"",
    docs.""PIDNumber"" AS ""P&Id Number"",
    docs.""LayoutDrawing"" AS ""Layout Drawing"",
    docs.""ArchitecturalDrawing"" AS ""Architectural Drawing"",
    docs.""FunctionalDescriptionDocument"" AS ""Functional Description Document"",
    docs.""ProductProcurementNumber"" AS ""Product Procurement Number"",
    jbtag.""TagName"" AS ""Junction Box Number"",
    ""View_Device_Instruments"".""NatureOfSignalName"" AS ""Nature Of Signal"",
    ""View_Device_Instruments"".""FailStateName"" AS ""Fail State"",
    attrs.""GSDType"" AS ""GSD Type"",
    attrs.""ControlPanelNumber"" AS ""Control Panel Number"",
    attrs.""PLCNumber"" AS ""PLC Number"",
    attrs.""PLCSlotNumber"" AS ""PLC Slot Number"",
    ""panelTag"".""TagName"" AS ""Field Panel Number"",
    attrs.""DPDPCoupler"" AS ""DP/DP Coupler"",
    attrs.""DPPACoupler"" AS ""DP/PA Coupler"",
    attrs.""AFDHubNumber"" AS ""AFD / Hub Number"",
    attrs.""RackNo"" AS ""Rack No"",
    attrs.""SlotNo"" AS ""Slot No"",
    attrs.""ChannelNo"" AS ""Channel No"",
    attrs.""DPNodeAddress"" AS ""DP Node Address"",
    attrs.""PANodeAddress"" AS ""PA Node Address"",
    ""View_Device_Instruments"".""Revision"",
    ""View_Device_Instruments"".""RevisionChanges"" AS ""Revision Changes / Outstanding Comments"",
    ""View_Device_Instruments"".""Zone"",
    ""View_Device_Instruments"".""Bank"",
    ""View_Device_Instruments"".""Service"",
    ""View_Device_Instruments"".""Variable"",
    ""View_Device_Instruments"".""Train"",
    ""View_Device_Instruments"".""WorkAreaPack"" AS ""Work Area Pack"",
    ""View_Device_Instruments"".""System"" AS ""System Code"",
    ""View_Device_Instruments"".""SubSystem"" AS ""SubSystem Code"",
    ""View_Device_Instruments"".""HistoricalLogging"" AS ""Historical Logging"",
    ""View_Device_Instruments"".""HistoricalLoggingFrequency"" AS ""Historical Logging Frequency"",
    ""View_Device_Instruments"".""HistoricalLoggingResolution"" AS ""Historical Logging Resolution"",
    ""View_Device_Instruments"".""IsInstrument"",
    ""View_Device_Instruments"".""IsDeleted"",
    ""View_Device_Instruments"".""ProjectId""
   FROM ""View_Device_Instruments""
     CROSS JOIN LATERAL ""fnGetDeviceAttributesInColumns""(""View_Device_Instruments"".""Id"") attrs(""GSDType"", ""ControlPanelNumber"", ""PLCSlotNumber"", ""DPNodeAddress"", ""PLCNumber"", ""DPDPCoupler"", ""AFDHubNumber"", ""ChannelNo"", ""DPPACoupler"", ""PANodeAddress"", ""RackNo"", ""SlotNo"", ""CalibratedRangeMin"", ""CalibratedRangeMax"", ""CalibratedRangeUnits"", ""ProcessRangeMin"", ""ProcessRangeMax"", ""ProcessRangeUnits"", ""RLPosition"")
     CROSS JOIN LATERAL ""fnGetDeviceDocumentsInColumns""(""View_Device_Instruments"".""Id"") docs(""DatasheetNumber"", ""SheetNumber"", ""HookupDrawing"", ""TerminationDiagram"", ""PIDNumber"", ""LayoutDrawing"", ""ArchitecturalDrawing"", ""FunctionalDescriptionDocument"", ""ProductProcurementNumber"")
     LEFT JOIN ""ControlSystemHierarchy"" h ON h.""ChildDeviceId"" = ""View_Device_Instruments"".""Id"" AND h.""Instrument"" = true
     LEFT JOIN ""Device"" pd ON h.""ParentDeviceId"" = pd.""Id""
     LEFT JOIN ""Tag"" pt ON pd.""TagId"" = pt.""Id""
     LEFT JOIN ""Tag"" skidtag ON skidtag.""Id"" = ""View_Device_Instruments"".""SkidTagId""
     LEFT JOIN ""Tag"" standtag ON standtag.""Id"" = ""View_Device_Instruments"".""StandTagId""
     LEFT JOIN ""Tag"" jbtag ON jbtag.""Id"" = ""View_Device_Instruments"".""JunctionBoxTagId""
     LEFT JOIN ""Tag"" ""panelTag"" ON ""panelTag"".""Id"" = ""View_Device_Instruments"".""PanelTagId"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
