using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SPGetDeviceTypeModelNatureOfSignalAttributeDefinitionIDs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spGetDeviceTypeModelNatureOfSignalAttributeDefinitionIDs""(
	IN ""_DeviceTypeId"" uuid,
	IN ""_DeviceModelId"" uuid,
	IN ""_NatureOfSignalId"" uuid,
	INOUT ""resultData"" refcursor DEFAULT NULL::refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN
	Open ""resultData"" FOR
	SELECT 
		""AttributeDefinition"".""Id"" AS ""AttributeDefinitionId""
	FROM ""AttributeDefinition"" 
	WHERE ""_DeviceModelId"" = ""AttributeDefinition"".""DeviceModelId"" AND ""AttributeDefinition"".""Inherit"" = false

	UNION 
	
	SELECT 
		""AttributeDefinition"".""Id"" AS ""AttributeDefinitionId""
	FROM ""AttributeDefinition"" 
	WHERE ""_DeviceTypeId"" = ""AttributeDefinition"".""DeviceTypeId"" AND ""AttributeDefinition"".""Inherit"" = false

	UNION 
	
	SELECT 
		""AttributeDefinition"".""Id"" AS ""AttributeDefinitionId""
	FROM ""AttributeDefinition"" 
	WHERE ""_NatureOfSignalId"" IS NOT NULL AND (""_NatureOfSignalId"" = ""AttributeDefinition"".""NatureOfSignalId"" AND ""AttributeDefinition"".""Inherit"" = false);
	
	return;
END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spGetDeviceConfigurableAttributeDefinitionIDs""(
	IN ""_DeviceId"" uuid,
	IN ""_ParentsOnly"" boolean DEFAULT false,
	INOUT ""resultData"" refcursor DEFAULT NULL::refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

DECLARE
	""curs1"" refcursor;
	 ""record_data"" RECORD;

BEGIN
 	-- Create a temporary table
    CREATE TEMP TABLE ""configurable_attributes"" (
        ""AttributeDefinitionId"" uuid,
        ""Name"" character varying(255),
        ""ValueType"" character varying(255),
        ""Required"" boolean
    ) on commit drop;
	
	CALL  public.""spGetDeviceConfigurableAttributes""(""_DeviceId"",""curs1"");
	  
	 LOOP
        FETCH ""curs1"" INTO ""record_data"";

        EXIT WHEN NOT FOUND;
        
		 INSERT INTO ""configurable_attributes"" (""AttributeDefinitionId"", ""Name"", ""ValueType"", ""Required"")
         VALUES (record_data.""AttributeDefinitionId"", record_data.""Name"", record_data.""ValueType"", record_data.""Required"");
    END LOOP;

    -- Close the cursor
    CLOSE ""curs1"";
	
	Open ""resultData"" FOR
	SELECT ""AttributeDefinitionId"" FROM ""configurable_attributes"";
	
	return;
END;
$BODY$;");


			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spGetDeviceConfigurableAttributes""(
	IN ""_DeviceId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN
	-- navigate hierarchy from child to parent (top level)
	Open ""resultData"" FOR
	WITH RECURSIVE ""DeviceTree"" (""ParentId"", ""DeviceId"", ""TreeLevel"")
	AS (
		SELECT ""_DeviceId"" as ""ParentId"", NULL::uuid As ""DeviceId"", 0 as ""TreeLevel""
		UNION ALL
		SELECT ""ControlSystemHierarchy"".""ParentDeviceId"", ""Device"".""Id"", ""TreeLevel"" - 1
		FROM ""DeviceTree""
		INNER JOIN ""ControlSystemHierarchy"" ON ""ChildDeviceId"" = ""DeviceTree"".""ParentId""
		INNER JOIN ""Device"" ON ""ControlSystemHierarchy"".""ChildDeviceId"" = ""Device"".""Id""
	)
	-- For each row get the attributes for that device type/model. Configurable attributes are 
	-- identified by the fact that they either device model or type Id in the attribute definition record
	SELECT 
		""def"".""Id"" as ""AttributeDefinitionId"", 
		""def"".""Name"", 
		""def"".""ValueType"",
		""def"".""Required""
	FROM ""DeviceTree"" 
		INNER JOIN ""Device"" ON ""DeviceTree"".""ParentId"" = ""Device"".""Id"" 
		INNER JOIN ""DeviceType"" ON ""DeviceType"".""Id"" = ""Device"".""DeviceTypeId"" 
		INNER JOIN ""AttributeDefinition"" ""def"" ON ""def"".""DeviceTypeId"" = ""DeviceType"".""Id""
		WHERE 
			""def"".""Inherit"" = true
	UNION  
	SELECT 
		""def"".""Id"" as ""AttributeDefinitionId"", 
		""def"".""Name"", 
		""def"".""ValueType"",
		""def"".""Required""
	FROM ""DeviceTree"" 
		INNER JOIN ""Device"" ON ""DeviceTree"".""ParentId"" = ""Device"".""Id"" 
		INNER JOIN ""DeviceModel"" ON ""DeviceModel"".""Id"" = ""Device"".""DeviceModelId"" 
		INNER JOIN ""AttributeDefinition"" ""def"" ON ""def"".""DeviceModelId"" = ""DeviceModel"".""Id""
		WHERE 
			""def"".""Inherit"" = true
	UNION  
	SELECT 
		""def"".""Id"" as ""AttributeDefinitionId"", 
		""def"".""Name"", 
		""def"".""ValueType"",
		""def"".""Required""
	FROM ""DeviceTree"" 
		INNER JOIN ""Device"" ON ""DeviceTree"".""ParentId"" = ""Device"".""Id"" 
		INNER JOIN ""NatureOfSignal"" ON ""NatureOfSignal"".""Id"" = ""Device"".""NatureOfSignalId""
		INNER JOIN ""AttributeDefinition"" ""def"" ON ""def"".""NatureOfSignalId"" = ""NatureOfSignal"".""Id""
		WHERE 
			""def"".""Inherit"" = true AND ""Device"".""NatureOfSignalId"" IS NOT NULL;
			
	return;
END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_Device_Instruments""
 AS
 SELECT ""Device"".""Id"",
    ""DeviceModel"".""Model"",
    ""DeviceModel"".""Description"" AS ""ModelDescription"",
    ""View_Tag"".""TagName"",
    ""DeviceType"".""Type"",
    ""View_Tag"".""ProcessName"",
    ""ServiceZone"".""Zone"",
    ""ServiceBank"".""Bank"",
    ""Device"".""Service"",
    ""NatureOfSignal"".""NatureOfSignalName"",
    ""View_Tag"".""SubProcessName"",
    ""View_Tag"".""StreamName"",
    ""View_Tag"".""EquipmentCode"",
    ""Device"".""ServiceDescription"",
    ""Device"".""LineVesselNumber"",
    ""Device"".""VendorSupply"",
    ""FailState"".""FailStateName"",
    ""Device"".""Revision"",
    ""Device"".""RevisionChanges"",
    ""Device"".""ModifiedBy"",
    ""Device"".""ModifiedDate"",
    ""Device"".""IsDeleted"",
    ""Manufacturer"".""Name"" AS ""Manufacturer"",
    ""Device"".""Variable"",
    ""ServiceTrain"".""Train"",
    ""Device"".""DeviceModelId"",
    ""View_Tag"".""SequenceNumber"",
    ""View_Tag"".""EquipmentIdentifier"",
    ""Device"".""PanelTagId"",
    ""Device"".""SkidTagId"",
    ""Device"".""StandTagId"",
    ""Device"".""JunctionBoxTagId"",
    ""Device"".""DeviceTypeId"",
    ""Device"".""IsInstrument"",
    ""SubSystem"".""Number"" AS ""SubSystem"",
    ""System"".""Number"" AS ""System"",
    ""WorkAreaPack"".""Number"" AS ""WorkAreaPack"",
    ""Device"".""HistoricalLogging"",
    ""Device"".""HistoricalLoggingFrequency"",
    ""Device"".""HistoricalLoggingResolution"",
    ""View_Tag"".""ProjectId"",
    ""Device"".""IsActive""
   FROM ""DeviceModel""
     JOIN ""Manufacturer"" ON ""DeviceModel"".""ManufacturerId"" = ""Manufacturer"".""Id""
     RIGHT JOIN (""ServiceTrain""
     RIGHT JOIN (""DeviceType""
     RIGHT JOIN (""FailState""
     RIGHT JOIN (""System""
     JOIN ""WorkAreaPack"" ON ""System"".""WorkAreaPackId"" = ""WorkAreaPack"".""Id""
     JOIN ""SubSystem"" ON ""System"".""Id"" = ""SubSystem"".""SystemId""
     RIGHT JOIN ""Device"" ON ""Device"".""SubSystemId"" = ""SubSystem"".""Id"") ON ""FailState"".""Id"" = ""Device"".""FailStateId"") ON ""DeviceType"".""Id"" = ""Device"".""DeviceTypeId"") ON ""ServiceTrain"".""Id"" = ""Device"".""ServiceTrainId"") ON ""DeviceModel"".""Id"" = ""Device"".""DeviceModelId""
     JOIN ""View_Tag"" ON ""Device"".""TagId"" = ""View_Tag"".""Id""
     LEFT JOIN ""NatureOfSignal"" ON ""Device"".""NatureOfSignalId"" = ""NatureOfSignal"".""Id""
     LEFT JOIN ""ServiceZone"" ON ""Device"".""ServiceZoneId"" = ""ServiceZone"".""Id""
     LEFT JOIN ""ServiceBank"" ON ""Device"".""ServiceBankId"" = ""ServiceBank"".""Id""
  WHERE ""Device"".""IsInstrument""::bpchar = 'Y'::bpchar OR ""Device"".""IsInstrument""::bpchar = 'B'::bpchar;");


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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
