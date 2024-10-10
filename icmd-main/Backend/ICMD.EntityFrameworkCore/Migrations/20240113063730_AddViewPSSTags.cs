using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddViewPSSTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_PSSTags""
 AS
 WITH unionpart1 AS (
         SELECT sigtype_ext.""Kind"",
            ('Z'::text || t.""TagName""::text) || COALESCE('_'::text || sigtype_ext.""Extension""::text, ''::text) AS ""CBTagNumber"",
            sigtype_ext.""CBVariableType"",
            (('""'::text || t.""TagName""::text) || COALESCE('_'::text || sigtype_ext.""Extension""::text, ''::text)) || '""'::text AS ""PLCTag"",
            sigtype_ext.""PCS7VariableType"",
            sigtype_ext.""Extension"" AS ""SignalExtension"",
            p.""ProcessName"",
            sp.""SubProcessName"",
            st.""StreamName"",
            ec.""Code"" AS ""EquipmentCode"",
            t.""SequenceNumber"",
            t.""EquipmentIdentifier"",
            t.""TagName"",
            min(
                CASE dad.""Name""
                    WHEN 'PLC Number'::text THEN dav.""Value""
                    ELSE NULL::character varying
                END::text) AS ""PLCNumber"",
            sigtype.""NatureOfSignalName"",
            NULL::text AS ""GSDType"",
            ""Manufacturer"".""Name"" AS ""Manufacturer"",
            ""DeviceModel"".""Model"",
            t.""ProjectId""
           FROM ""Tag"" t
             JOIN ""Device"" d ON t.""Id"" = d.""TagId""
             LEFT JOIN ""DeviceModel"" ON d.""DeviceModelId"" = ""DeviceModel"".""Id""
             LEFT JOIN ""Manufacturer"" ON ""DeviceModel"".""ManufacturerId"" = ""Manufacturer"".""Id""
             LEFT JOIN (""AttributeValue"" dav
             JOIN ""DeviceAttributeValue"" map ON dav.""Id"" = map.""AttributeValueId""
             JOIN ""AttributeDefinition"" dad ON dav.""AttributeDefinitionId"" = dad.""Id"") ON d.""Id"" = map.""DeviceId""
             JOIN ""NatureOfSignal"" sigtype ON sigtype.""Id"" = d.""NatureOfSignalId""
             JOIN ""NatureOfSignalSignalExtension"" sigtype_map ON sigtype_map.""NatureOfSignalId"" = sigtype.""Id""
             JOIN ""SignalExtension"" sigtype_ext ON sigtype_ext.""Id"" = sigtype_map.""SignalExtensionId""
             LEFT JOIN ""Process"" p ON p.""Id"" = t.""ProcessId""
             LEFT JOIN ""SubProcess"" sp ON sp.""Id"" = t.""SubProcessId""
             LEFT JOIN ""Stream"" st ON st.""Id"" = t.""StreamId""
             JOIN ""EquipmentCode"" ec ON ec.""Id"" = t.""EquipmentCodeId""
          WHERE d.""NatureOfSignalId"" IS NOT NULL AND (d.""IsInstrument"" = 'Y'::bpchar OR d.""IsInstrument"" = 'B'::bpchar) AND d.""IsDeleted"" = false
          GROUP BY ""Manufacturer"".""Name"", ""DeviceModel"".""Model"", t.""TagName"", t.""SequenceNumber"", t.""EquipmentIdentifier"", p.""ProcessName"", sp.""SubProcessName"", st.""StreamName"", ec.""Code"", sigtype_ext.""Kind"", sigtype_ext.""Extension"", sigtype_ext.""CBVariableType"", sigtype_ext.""PCS7VariableType"", sigtype.""NatureOfSignalName"", t.""ProjectId""
        ), unionpart2 AS (
         SELECT sig.""Kind"",
            ('Z'::text || t.""TagName""::text) || COALESCE('_'::text || sig.""Extension""::text, ''::text) AS ""CBTagNumber"",
            sig.""CBVariableType"",
            (('""'::text || t.""TagName""::text) || COALESCE('_'::text || sig.""Extension""::text, ''::text)) || '""'::text AS ""PLCTag"",
            sig.""PCS7VariableType"",
            sig.""Extension"" AS ""SignalExtension"",
            p.""ProcessName"",
            sp.""SubProcessName"",
            st.""StreamName"",
            ec.""Code"" AS ""EquipmentCode"",
            t.""SequenceNumber"",
            t.""EquipmentIdentifier"",
            t.""TagName"",
            min(
                CASE dad.""Name""
                    WHEN 'PLC Number'::text THEN dav.""Value""
                    ELSE NULL::character varying
                END::text) AS ""PLCNumber"",
            sigtype.""NatureOfSignalName"",
            gsd.""GSDTypeName"",
            ""Manufacturer"".""Name"" AS ""Manufacturer"",
            ""DeviceModel"".""Model"",
            t.""ProjectId""
           FROM ""Tag"" t
             JOIN ""Device"" d ON t.""Id"" = d.""TagId""
             LEFT JOIN ""DeviceType"" dt ON d.""DeviceTypeId"" = dt.""Id""
             LEFT JOIN ""DeviceModel"" ON d.""DeviceModelId"" = ""DeviceModel"".""Id""
             LEFT JOIN ""Manufacturer"" ON ""DeviceModel"".""ManufacturerId"" = ""Manufacturer"".""Id""
             LEFT JOIN (""AttributeValue"" dav
             JOIN ""DeviceAttributeValue"" map ON dav.""Id"" = map.""AttributeValueId""
             JOIN ""AttributeDefinition"" dad ON dav.""AttributeDefinitionId"" = dad.""Id"") ON d.""Id"" = map.""DeviceId""
             LEFT JOIN ""AttributeValue"" mv ON mv.""DeviceModelId"" = d.""DeviceModelId""
             LEFT JOIN ""AttributeDefinition"" md ON md.""Id"" = mv.""AttributeDefinitionId"" AND md.""Name""::text = 'GSD Type'::text
             JOIN ""GSDType"" gsd ON gsd.""GSDTypeName""::text = mv.""Value""::text
             JOIN ""GSDType_SignalExtension"" gsdmap ON gsdmap.""GSDTypeId"" = gsd.""Id""
             JOIN ""SignalExtension"" sig ON sig.""Id"" = gsdmap.""SignalExtensionId""
             JOIN ""NatureOfSignal"" sigtype ON sigtype.""Id"" = d.""NatureOfSignalId""
             LEFT JOIN ""Process"" p ON p.""Id"" = t.""ProcessId""
             LEFT JOIN ""SubProcess"" sp ON sp.""Id"" = t.""SubProcessId""
             LEFT JOIN ""Stream"" st ON st.""Id"" = t.""StreamId""
             JOIN ""EquipmentCode"" ec ON ec.""Id"" = t.""EquipmentCodeId""
          WHERE d.""NatureOfSignalId"" IS NOT NULL AND (d.""IsInstrument"" = 'Y'::bpchar OR d.""IsInstrument"" = 'B'::bpchar) AND d.""IsDeleted"" = false
          GROUP BY ""Manufacturer"".""Name"", ""DeviceModel"".""Model"", t.""TagName"", mv.""Value"", gsd.""GSDTypeName"", t.""SequenceNumber"", t.""EquipmentIdentifier"", p.""ProcessName"", sp.""SubProcessName"", st.""StreamName"", ec.""Code"", sig.""Kind"", sig.""Extension"", sig.""PCS7VariableType"", sig.""CBVariableType"", sigtype.""NatureOfSignalName"", t.""ProjectId""
        )
 SELECT unionpart1.""Kind"",
    unionpart1.""CBTagNumber"",
    unionpart1.""CBVariableType"",
    unionpart1.""PLCTag"",
    unionpart1.""PCS7VariableType"",
    unionpart1.""SignalExtension"",
    unionpart1.""ProcessName"",
    unionpart1.""SubProcessName"",
    unionpart1.""StreamName"",
    unionpart1.""EquipmentCode"",
    unionpart1.""SequenceNumber"",
    unionpart1.""EquipmentIdentifier"",
    unionpart1.""TagName"",
    unionpart1.""PLCNumber"",
    unionpart1.""NatureOfSignalName"",
    unionpart1.""GSDType"",
    unionpart1.""Manufacturer"",
    unionpart1.""Model"",
    unionpart1.""ProjectId""
   FROM unionpart1
UNION
 SELECT unionpart2.""Kind"",
    unionpart2.""CBTagNumber"",
    unionpart2.""CBVariableType"",
    unionpart2.""PLCTag"",
    unionpart2.""PCS7VariableType"",
    unionpart2.""SignalExtension"",
    unionpart2.""ProcessName"",
    unionpart2.""SubProcessName"",
    unionpart2.""StreamName"",
    unionpart2.""EquipmentCode"",
    unionpart2.""SequenceNumber"",
    unionpart2.""EquipmentIdentifier"",
    unionpart2.""TagName"",
    unionpart2.""PLCNumber"",
    unionpart2.""NatureOfSignalName"",
    unionpart2.""GSDTypeName"",
    unionpart2.""Manufacturer"",
    unionpart2.""Model"",
    unionpart2.""ProjectId""
   FROM unionpart2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
