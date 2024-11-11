using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateView_NonInstrumentList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DO $$
BEGIN
DROP VIEW public.""View_NonInstrumentList"";
CREATE OR REPLACE VIEW public.""View_NonInstrumentList""
 AS
 SELECT ""View_Device_NonInstruments"".""Id"" AS ""DeviceId"",
    ""View_Device_NonInstruments"".""ProcessName"" AS ""Process No"",
    ""View_Device_NonInstruments"".""SubProcessName"" AS ""Sub Process"",
    ""View_Device_NonInstruments"".""StreamName"",
    ""View_Device_NonInstruments"".""EquipmentCode"" AS ""Equipment Code"",
    ""View_Device_NonInstruments"".""SequenceNumber"" AS ""Sequence Number"",
    ""View_Device_NonInstruments"".""EquipmentIdentifier"" AS ""Equipment Identifier"",
    ""View_Device_NonInstruments"".""TagName"",
    dvt.""Description"" AS ""Device Type"",
    ""View_Device_NonInstruments"".""ServiceDescription"" AS ""Service Description"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Description'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""Description"",
    ""View_Device_NonInstruments"".""NatureOfSignalName"" AS ""Nature of Signal"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'DP Node Address'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""DP Node Address"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'No of Slots/Channels'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""No of Slots or Channels"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""Slot Number"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Number'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""PLC Number"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""PLC Slot Number"",
    ""panelTag"".""TagName"" AS ""Location"",
    ""View_Device_NonInstruments"".""Manufacturer"",
    ""View_Device_NonInstruments"".""Model"" AS ""Model Number"",
    ""View_Device_NonInstruments"".""ModelDescription"" AS ""Model Description"",
	max(
        CASE
            WHEN h.""Instrument"" = false THEN pt.""TagName""
            ELSE NULL::character varying
        END::text) AS ""Connection Parent Tag"",
    max(
        CASE
            WHEN h.""Instrument"" = true THEN pt.""TagName""
            ELSE NULL::character varying
        END::text) AS ""Instr Parent Tag"",
    min(
        CASE ""View_AllDocuments"".""Type""
            WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""DocumentNumber""
            ELSE NULL::character varying
        END::text) AS ""Architecture Drawing"",
    min(
        CASE ""View_AllDocuments"".""Type""
            WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""Sheet""
            ELSE NULL::character varying
        END::text) AS ""Architecture Drawing Sheet"",
    ""View_Device_NonInstruments"".""Revision"",
    ""View_Device_NonInstruments"".""RevisionChanges"",
    ""View_Device_NonInstruments"".""IsInstrument"",
    ""View_Device_NonInstruments"".""IsDeleted"",
    ""View_Device_NonInstruments"".""IsActive"",
    ""View_Device_NonInstruments"".""ProjectId""
   FROM ""View_Device_NonInstruments""
     LEFT JOIN ""View_AllDocuments"" ON ""View_AllDocuments"".""DeviceId"" = ""View_Device_NonInstruments"".""Id""
     LEFT JOIN ""View_AllAttributes"" ON ""View_AllAttributes"".""Id"" = ""View_Device_NonInstruments"".""Id""
     LEFT JOIN ""ControlSystemHierarchy"" h ON h.""ChildDeviceId"" = ""View_Device_NonInstruments"".""Id""
     LEFT JOIN ""Device"" pd ON h.""ParentDeviceId"" = pd.""Id""
     LEFT JOIN ""Tag"" pt ON pd.""TagId"" = pt.""Id""
     LEFT JOIN ""DeviceType"" dvt ON dvt.""Id"" = ""View_Device_NonInstruments"".""DeviceTypeId""
     LEFT JOIN ""Tag"" ""panelTag"" ON ""panelTag"".""Id"" = ""View_Device_NonInstruments"".""PanelTagId""
  GROUP BY ""View_Device_NonInstruments"".""ProcessName"", ""View_Device_NonInstruments"".""SubProcessName"", ""View_Device_NonInstruments"".""StreamName"", ""View_Device_NonInstruments"".""EquipmentCode"", ""View_Device_NonInstruments"".""SequenceNumber"", ""View_Device_NonInstruments"".""EquipmentIdentifier"", ""View_Device_NonInstruments"".""TagName"", ""View_Device_NonInstruments"".""ServiceDescription"", ""View_Device_NonInstruments"".""Manufacturer"", ""View_Device_NonInstruments"".""Model"", ""View_Device_NonInstruments"".""Revision"", ""View_Device_NonInstruments"".""RevisionChanges"", ""View_Device_NonInstruments"".""IsDeleted"", ""View_Device_NonInstruments"".""IsActive"", ""View_Device_NonInstruments"".""Id"", ""View_Device_NonInstruments"".""PanelTagId"", ""panelTag"".""TagName"", ""View_Device_NonInstruments"".""IsInstrument"", dvt.""Description"", ""View_Device_NonInstruments"".""NatureOfSignalName"", ""View_Device_NonInstruments"".""ModelDescription"", ""View_Device_NonInstruments"".""ProjectId"";

END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DO $$
BEGIN
DROP VIEW public.""View_NonInstrumentList"";
CREATE OR REPLACE VIEW public.""View_NonInstrumentList""
 AS
 SELECT ""View_Device_NonInstruments"".""Id"" AS ""DeviceId"",
    ""View_Device_NonInstruments"".""ProcessName"" AS ""Process No"",
    ""View_Device_NonInstruments"".""SubProcessName"" AS ""Sub Process"",
    ""View_Device_NonInstruments"".""StreamName"",
    ""View_Device_NonInstruments"".""EquipmentCode"" AS ""Equipment Code"",
    ""View_Device_NonInstruments"".""SequenceNumber"" AS ""Sequence Number"",
    ""View_Device_NonInstruments"".""EquipmentIdentifier"" AS ""Equipment Identifier"",
    ""View_Device_NonInstruments"".""TagName"",
    dvt.""Description"" AS ""Device Type"",
    ""View_Device_NonInstruments"".""ServiceDescription"" AS ""Service Description"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Description'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""Description"",
    ""View_Device_NonInstruments"".""NatureOfSignalName"" AS ""Nature of Signal"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'DP Node Address'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""DP Node Address"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'No of Slots/Channels'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""No of Slots or Channels"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""Slot Number"",
    pt.""TagName"" AS ""Connection Parent"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Number'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""PLC Number"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Slot Number'::text THEN ""View_AllAttributes"".""Value""::text
            ELSE NULL::text
        END) AS ""PLC Slot Number"",
    ""panelTag"".""TagName"" AS ""Location"",
    ""View_Device_NonInstruments"".""Manufacturer"",
    ""View_Device_NonInstruments"".""Model"" AS ""Model Number"",
    ""View_Device_NonInstruments"".""ModelDescription"" AS ""Model Description"",
    min(
        CASE ""View_AllDocuments"".""Type""
            WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""DocumentNumber""
            ELSE NULL::character varying
        END::text) AS ""Architecture Drawing"",
    min(
        CASE ""View_AllDocuments"".""Type""
            WHEN 'Architecture Drawing'::text THEN ""View_AllDocuments"".""Sheet""
            ELSE NULL::character varying
        END::text) AS ""Architecture Drawing Sheet"",
    ""View_Device_NonInstruments"".""Revision"",
    ""View_Device_NonInstruments"".""RevisionChanges"",
    ""View_Device_NonInstruments"".""IsInstrument"",
    ""View_Device_NonInstruments"".""IsDeleted"",
    ""View_Device_NonInstruments"".""IsActive"",
    ""View_Device_NonInstruments"".""ProjectId""
   FROM ""View_Device_NonInstruments""
     LEFT JOIN ""View_AllDocuments"" ON ""View_AllDocuments"".""DeviceId"" = ""View_Device_NonInstruments"".""Id""
     LEFT JOIN ""View_AllAttributes"" ON ""View_AllAttributes"".""Id"" = ""View_Device_NonInstruments"".""Id""
     LEFT JOIN ""ControlSystemHierarchy"" h ON h.""ChildDeviceId"" = ""View_Device_NonInstruments"".""Id"" AND h.""Instrument"" = false
     LEFT JOIN ""Device"" pd ON h.""ParentDeviceId"" = pd.""Id""
     LEFT JOIN ""Tag"" pt ON pd.""TagId"" = pt.""Id""
     LEFT JOIN ""DeviceType"" dvt ON dvt.""Id"" = ""View_Device_NonInstruments"".""DeviceTypeId""
     LEFT JOIN ""Tag"" ""panelTag"" ON ""panelTag"".""Id"" = ""View_Device_NonInstruments"".""PanelTagId""
  GROUP BY ""View_Device_NonInstruments"".""ProcessName"", ""View_Device_NonInstruments"".""SubProcessName"", ""View_Device_NonInstruments"".""StreamName"", ""View_Device_NonInstruments"".""EquipmentCode"", ""View_Device_NonInstruments"".""SequenceNumber"", ""View_Device_NonInstruments"".""EquipmentIdentifier"", ""View_Device_NonInstruments"".""TagName"", ""View_Device_NonInstruments"".""ServiceDescription"", ""View_Device_NonInstruments"".""Manufacturer"", ""View_Device_NonInstruments"".""Model"", ""View_Device_NonInstruments"".""Revision"", ""View_Device_NonInstruments"".""RevisionChanges"", ""View_Device_NonInstruments"".""IsDeleted"", ""View_Device_NonInstruments"".""IsActive"", ""View_Device_NonInstruments"".""Id"", ""View_Device_NonInstruments"".""PanelTagId"", pt.""TagName"", ""panelTag"".""TagName"", ""View_Device_NonInstruments"".""IsInstrument"", dvt.""Description"", ""View_Device_NonInstruments"".""NatureOfSignalName"", ""View_Device_NonInstruments"".""ModelDescription"", ""View_Device_NonInstruments"".""ProjectId"";
END $$;");
        }
    }
}
