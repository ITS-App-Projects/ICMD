using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SparesReportFunctionSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spSparesReport""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

CREATE TEMP TABLE ""_Capacity""(""Total Channels"" int, ""Used Channels"" int, ""Rack"" character varying(25), ""PLC Number"" character varying(25), ""Nature of Signal"" character varying(10)) ON COMMIT DROP;

INSERT INTO ""_Capacity""
	SELECT 
		SUM(CASE 
            WHEN ""No of Slots or Channels"" ~ E'^\\d+$' THEN CAST(""No of Slots or Channels"" AS INTEGER)
            ELSE 0
        END) AS ""Total Channels"",
		0 AS ""Used Channels"",
		""View_NonInstrumentList"".""Connection Parent"" AS ""Rack"", 
		""View_NonInstrumentList"".""PLC Number"",
		""View_NonInstrumentList"".""Nature of Signal""
	FROM ""View_NonInstrumentList"" 
	WHERE ""ProjectId"" = ""_ProjectId""
	AND ""View_NonInstrumentList"".""Nature of Signal"" NOT IN ('DP', 'PA')
	AND ""Device Type"" = 'Slot number in a Rack'
	AND ""View_NonInstrumentList"".""IsDeleted"" = false
	GROUP BY 
		""View_NonInstrumentList"".""Connection Parent"", 
		""View_NonInstrumentList"".""PLC Number"",
		""View_NonInstrumentList"".""Nature of Signal"";

--RAISE WARNING 'Nature', ""Nature of Signal"";
UPDATE ""_Capacity""
	SET ""Used Channels"" = public.""fnSlotChannelsInUseByRack""(""Rack"", ""Nature of Signal"", ""_ProjectId"")
	where ""Rack"" is not null and ""Nature of Signal"" is not null;

Open ""resultData"" FOR
SELECT * FROM ""_Capacity"";

RETURN;

END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnSlotChannelsInUseByRack""(
	""_RackNo"" character varying,
	""_NatureOfSignal"" character varying,
	""_ProjectId"" uuid)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE
   ""_Cnt"" INT;

BEGIN
	-- first find all devices that have rack numbers matching (to reduce search space)
	CREATE TEMP TABLE ""_Candidate""(""DeviceId"" uuid ) ON COMMIT DROP;
	
	INSERT INTO ""_Candidate"" (""DeviceId"") (
		SELECT ""vd"".""Id""
		FROM ""View_Device"" ""vd""
		WHERE ""vd"".""ProjectId"" = ""_ProjectId""
			AND ""vd"".""IsInstrument"" <> '-'
			AND ""vd"".""IsDeleted"" = false 
			AND (""vd"".""NatureOfSignalName"" = ""_NatureOfSignal"")
		INTERSECT
		SELECT ""Id"" FROM ""View_DeviceRackAttributesInColumns""  ""vdr""
			WHERE ""vdr"".""ProjectId"" = ""_ProjectId""
				AND ""vdr"".""RackNo"" = ""_RackNo""
				AND ""vdr"".""ChannelNo"" IS NOT NULL
	);

	SELECT COUNT(distinct ""vdr"".""ChannelNo"") INTO ""_Cnt""
	FROM ""View_DeviceRackAttributesInColumns"" ""vdr""
	INNER JOIN ""_Candidate"" ON ""_Candidate"".""DeviceId""=""vdr"".""Id"";
	
	Drop table ""_Candidate"";
	RETURN ""_Cnt"";
END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_DeviceRackAttributesInColumns""
 AS
 SELECT ""Device"".""Id"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Slot Number'::text THEN ""View_AllAttributes"".""Value""::real
            ELSE NULL::real
        END) AS ""PLCSlotNumber"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'PLC Number'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""PLCNumber"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Channel Number'::text THEN ""View_AllAttributes"".""Value""::real
            ELSE NULL::real
        END) AS ""ChannelNo"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'RIO Rack Number'::text THEN ""View_AllAttributes"".""Value""
            WHEN 'VMB Rack Number'::text THEN ""View_AllAttributes"".""Value""
            ELSE NULL::character varying
        END::text) AS ""RackNo"",
    min(
        CASE ""View_AllAttributes"".""Name""
            WHEN 'Slot Number'::text THEN ""View_AllAttributes"".""Value""::real
            ELSE NULL::real
        END) AS ""SlotNo"",
    ""NatureOfSignal"".""NatureOfSignalName"",
    ""Tag"".""ProjectId""
   FROM ""View_AllAttributes""
     RIGHT JOIN ""Device"" ON ""Device"".""Id"" = ""View_AllAttributes"".""Id""
     JOIN ""Tag"" ON ""Tag"".""Id"" = ""Device"".""TagId""
     LEFT JOIN ""NatureOfSignal"" ON ""Device"".""NatureOfSignalId"" = ""NatureOfSignal"".""Id""
  GROUP BY ""Device"".""Id"", ""Tag"".""ProjectId"", ""NatureOfSignal"".""NatureOfSignalName"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
