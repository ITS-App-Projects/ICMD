using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SparesReportDetailsSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spSparesReportNoSDetail""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

CREATE TEMP TABLE ""_Capacity""(
	""PLC Number"" character varying(25),
	""Rack"" character varying(25),
	""Total Channels"" INT,
	""Used Channels"" INT,
	""Slot Number"" INT,
	""Nature Of Signal"" character varying(10)
) ON COMMIT DROP;

INSERT INTO ""_Capacity""
SELECT
	""View_NonInstrumentList"".""PLC Number"",
	""View_NonInstrumentList"".""Connection Parent"" AS ""Rack"",
	COALESCE(CAST(""View_NonInstrumentList"".""No of Slots or Channels"" AS INTEGER), 0) AS ""Total Channels"",
	count(distinct ""View_DeviceRackAttributesInColumns"".""ChannelNo"") AS ""Used Channels"",
	COALESCE(CAST(""View_NonInstrumentList"".""Slot Number"" AS real), 0) AS ""Slot Number"",
	""View_DeviceRackAttributesInColumns"".""NatureOfSignalName"" AS ""Nature Of Signal""
FROM ""View_NonInstrumentList""
	LEFT OUTER JOIN ""View_DeviceRackAttributesInColumns""
		ON ""View_DeviceRackAttributesInColumns"".""RackNo"" = ""View_NonInstrumentList"".""Connection Parent""
		AND ""View_DeviceRackAttributesInColumns"".""SlotNo"" =   COALESCE(CAST(""View_NonInstrumentList"".""Slot Number"" AS REAL), 0)
	INNER JOIN ""Device"" on ""Device"".""Id""=""View_DeviceRackAttributesInColumns"".""Id""
WHERE ""View_NonInstrumentList"".""ProjectId"" = ""_ProjectId""
	AND ""View_NonInstrumentList"".""IsDeleted"" = false
	AND ""View_NonInstrumentList"".""Device Type""='Slot number in a Rack'
	AND ""Device"".""IsDeleted""=false
	AND ""Device"".""IsInstrument""<>'-'
group by
	""View_DeviceRackAttributesInColumns"".""RackNo"",
	""View_DeviceRackAttributesInColumns"".""NatureOfSignalName"",
	""View_NonInstrumentList"".""PLC Number"",
	""View_NonInstrumentList"".""Connection Parent"",
	""View_NonInstrumentList"".""No of Slots or Channels"",
	""View_NonInstrumentList"".""Slot Number""
order by
	""View_DeviceRackAttributesInColumns"".""RackNo"",
	""View_NonInstrumentList"".""Slot Number"";

INSERT INTO ""_Capacity""
SELECT
	'ALL PLCs',
	'ALL Racks',
	SUM(""_Capacity"".""Total Channels""),
	SUM(""_Capacity"".""Used Channels""),
	NULL,
	""_Capacity"".""Nature Of Signal""
FROM ""_Capacity""
GROUP BY ""_Capacity"".""Nature Of Signal"";

Open ""resultData"" FOR
SELECT * FROM ""_Capacity"";

RETURN;

END;
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_AllAttributes""
 AS
 SELECT cache.""DeviceId"" AS ""Id"",
    def.""Id"" AS ""AttributeDefinitionId"",
    def.""Name"",
    def.""ValueType"",
    val.""Id"" AS ""AttributeValueId"",
    val.""Value""
   FROM ""DeviceAttributeValue"" cache
     JOIN ""AttributeValue"" val ON val.""Id"" = cache.""AttributeValueId""
     JOIN ""AttributeDefinition"" def ON def.""Id"" = val.""AttributeDefinitionId""
	where cache.""IsDeleted""=false;
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
