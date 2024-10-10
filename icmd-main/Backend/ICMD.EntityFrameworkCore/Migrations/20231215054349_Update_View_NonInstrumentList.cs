using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_View_NonInstrumentList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
//            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnGetDeviceAttributesInColumns""(
//	""_DeviceId"" uuid)
//    RETURNS TABLE(""GSDType"" text, ""ControlPanelNumber"" text, ""PLCSlotNumber"" text, ""DPNodeAddress"" text, ""PLCNumber"" text, ""DPDPCoupler"" text, ""AFDHubNumber"" text, ""ChannelNo"" text, ""DPPACoupler"" text, ""PANodeAddress"" text, ""RackNo"" text, ""SlotNo"" text, ""CalibratedRangeMin"" text, ""CalibratedRangeMax"" text, ""CalibratedRangeUnits"" text, ""ProcessRangeMin"" text, ""ProcessRangeMax"" text, ""ProcessRangeUnits"" text, ""RLPosition"" text) 
//    LANGUAGE 'plpgsql'
//    COST 100
//    VOLATILE PARALLEL UNSAFE
//    ROWS 1000

//AS $BODY$

//            BEGIN
//                RETURN QUERY
//                SELECT     
//                    MIN(CASE ""Name"" WHEN 'GSD Type' THEN ""Value"" END) AS ""GSDType"", 
//                    MIN(CASE ""Name"" WHEN 'Control Panel Number' THEN ""Value"" END) AS ""ControlPanelNumber"", 
//                    MIN(CASE ""Name"" WHEN 'PLC Slot Number' THEN ""Value"" END) AS ""PLCSlotNumber"", 
//                    MIN(CASE ""Name"" WHEN 'DP Node Address' THEN ""Value"" END) AS ""DPNodeAddress"", 
//                    MIN(CASE ""Name"" WHEN 'PLC Number' THEN ""Value"" END) AS ""PLCNumber"", 
//                    MIN(CASE ""Name"" WHEN 'DP/DP Coupler' THEN ""Value"" END) AS ""DPDPCoupler"", 
//                    MIN(CASE ""Name"" 
//                            WHEN 'AFD Number' THEN ""Value"" 
//                            WHEN 'DP Hub Number' THEN ""Value""
//                        END) AS ""AFDHubNumber"", 
//                    MIN(CASE ""Name"" WHEN 'Channel Number' THEN ""Value"" END) AS ""ChannelNo"", 
//                    MIN(CASE ""Name"" WHEN 'DP/PA Coupler' THEN ""Value"" END) AS ""DPPACoupler"", 
//                    MIN(CASE ""Name"" WHEN 'PA Node Address' THEN ""Value"" END) AS ""PANodeAddress"",
                    
//                    MIN(CASE ""Name"" 
//                            WHEN 'RIO Rack Number' THEN ""Value"" 
//                            WHEN 'VMB Rack Number' THEN ""Value"" 
//                        END) AS ""RackNo"", 
//                    MIN(CASE ""Name"" WHEN 'Slot Number' THEN ""Value"" END) AS ""SlotNo"", 
//                    MIN(CASE ""Name"" WHEN 'Calibrated Range Min' THEN ""Value"" END) AS ""CalibratedRangeMin"", 
//                    MIN(CASE ""Name"" WHEN 'Calibrated Range Max' THEN ""Value"" END) AS ""CalibratedRangeMax"", 
//                    MIN(CASE ""Name"" WHEN 'Calibrated Range Units' THEN ""Value"" END) AS ""CalibratedRangeUnits"",
//                    MIN(CASE ""Name"" WHEN 'Process Range Min' THEN ""Value"" END) AS ""ProcessRangeMin"", 
//                    MIN(CASE ""Name"" WHEN 'Process Range Max' THEN ""Value"" END) AS ""ProcessRangeMax"", 
//                    MIN(CASE ""Name"" WHEN 'Process Range Units' THEN ""Value"" END) AS ""ProcessRangeUnits"",
//                    MIN(CASE ""Name"" WHEN 'RL / Position' THEN ""Value"" END) AS ""RLPosition""
//                FROM
//                    ""View_AllAttributes"" 
//                RIGHT OUTER JOIN ""Device"" ON ""Device"".""Id"" = ""View_AllAttributes"".""Id"" WHERE ""Device"".""Id""=""_DeviceId"";
                
//                RETURN;
//            END;
            
//$BODY$;");

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
    ""View_Device_Instruments"".""ProjectId"",
""View_Device_Instruments"".""IsActive""
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

 //           migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_NonInstrumentList""
 //AS
 //SELECT ""View_Device_NonInstruments"".""Id"" AS ""DeviceId"",
 //   ""View_Device_NonInstruments"".""ProcessName"" AS ""Process No"",
 //   ""View_Device_NonInstruments"".""SubProcessName"" AS ""Sub Process"",
 //   ""View_Device_NonInstruments"".""StreamName"",
 //   ""View_Device_NonInstruments"".""EquipmentCode"" AS ""Equipment Code"",
 //   ""View_Device_NonInstruments"".""SequenceNumber"" AS ""Sequence Number"",
 //   ""View_Device_NonInstruments"".""EquipmentIdentifier"" AS ""Equipment Identifier"",
 //   ""View_Device_NonInstruments"".""TagName"",
 //   dvt.""Description"" AS ""Device Type"",
 //   ""View_Device_NonInstruments"".""ServiceDescription"" AS ""Service Description"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'Description'::text THEN ""View_AllAttributes"".""Value""
 //           ELSE NULL::character varying
 //       END::text) AS ""Description"",
 //   ""View_Device_NonInstruments"".""NatureOfSignalName"" AS ""Nature of Signal"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'DP Node Address'::text THEN ""View_AllAttributes"".""Value""::text
 //           ELSE NULL::text
 //       END) AS ""DP Node Address"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'No of Slots/Channels'::text THEN ""View_AllAttributes"".""Value""::text
 //           ELSE NULL::text
 //       END) AS ""No of Slots or Channels"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
 //           ELSE NULL::text
 //       END) AS ""Slot Number"",
 //   pt.""TagName"" AS ""Connection Parent"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'PLC Number'::text THEN ""View_AllAttributes"".""Value""
 //           ELSE NULL::character varying
 //       END::text) AS ""PLC Number"",
 //   min(
 //       CASE ""View_AllAttributes"".""Name""
 //           WHEN 'PLC Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
 //           ELSE NULL::text
 //       END) AS ""PLC Slot Number"",
 //   ""panelTag"".""TagName"" AS ""Location"",
 //   ""View_Device_NonInstruments"".""Manufacturer"",
 //   ""View_Device_NonInstruments"".""Model"" AS ""Model Number"",
 //   ""View_Device_NonInstruments"".""ModelDescription"" AS ""Model Description"",
 //   min(
 //       CASE ""View_AllDocuments"".""Type""
 //           WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""DocumentNumber""
 //           ELSE NULL::character varying
 //       END::text) AS ""Architecture Drawing"",
 //   min(
 //       CASE ""View_AllDocuments"".""Type""
 //           WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""Sheet""
 //           ELSE NULL::character varying
 //       END::text) AS ""Architecture Drawing Sheet"",
 //   ""View_Device_NonInstruments"".""Revision"",
 //   ""View_Device_NonInstruments"".""RevisionChanges"",
 //   ""View_Device_NonInstruments"".""IsInstrument"",
 //   ""View_Device_NonInstruments"".""IsDeleted"",
 //   ""View_Device_NonInstruments"".""IsActive"",
 //   ""View_Device_NonInstruments"".""ProjectId""
 //  FROM ""View_Device_NonInstruments""
 //    LEFT JOIN ""View_AllDocuments"" ON ""View_AllDocuments"".""DeviceId"" = ""View_Device_NonInstruments"".""Id""
 //    LEFT JOIN ""View_AllAttributes"" ON ""View_AllAttributes"".""Id"" = ""View_Device_NonInstruments"".""Id""
 //    LEFT JOIN ""ControlSystemHierarchy"" h ON h.""ChildDeviceId"" = ""View_Device_NonInstruments"".""Id"" AND h.""Instrument"" = false
 //    LEFT JOIN ""Device"" pd ON h.""ParentDeviceId"" = pd.""Id""
 //    LEFT JOIN ""Tag"" pt ON pd.""TagId"" = pt.""Id""
 //    LEFT JOIN ""DeviceType"" dvt ON dvt.""Id"" = ""View_Device_NonInstruments"".""DeviceTypeId""
 //    LEFT JOIN ""Tag"" ""panelTag"" ON ""panelTag"".""Id"" = ""View_Device_NonInstruments"".""PanelTagId""
 // GROUP BY ""View_Device_NonInstruments"".""ProcessName"", ""View_Device_NonInstruments"".""SubProcessName"", ""View_Device_NonInstruments"".""StreamName"", ""View_Device_NonInstruments"".""EquipmentCode"", ""View_Device_NonInstruments"".""SequenceNumber"", ""View_Device_NonInstruments"".""EquipmentIdentifier"", ""View_Device_NonInstruments"".""TagName"", ""View_Device_NonInstruments"".""ServiceDescription"", ""View_Device_NonInstruments"".""Manufacturer"", ""View_Device_NonInstruments"".""Model"", ""View_Device_NonInstruments"".""Revision"", ""View_Device_NonInstruments"".""RevisionChanges"", ""View_Device_NonInstruments"".""IsDeleted"", ""View_Device_NonInstruments"".""IsActive"", ""View_Device_NonInstruments"".""Id"", ""View_Device_NonInstruments"".""PanelTagId"", pt.""TagName"", ""panelTag"".""TagName"", ""View_Device_NonInstruments"".""IsInstrument"", dvt.""Description"", ""View_Device_NonInstruments"".""NatureOfSignalName"", ""View_Device_NonInstruments"".""ModelDescription"", ""View_Device_NonInstruments"".""ProjectId"";");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
