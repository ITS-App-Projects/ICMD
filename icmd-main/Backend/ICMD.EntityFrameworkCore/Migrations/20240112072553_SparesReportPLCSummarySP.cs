using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SparesReportPLCSummarySP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnSlotChannelsInUseByPLC""(
	""_PLC"" character varying,
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
	CREATE TEMP TABLE ""_Candidate""(""DeviceId"" uuid PRIMARY KEY) ON COMMIT DROP;
	
	INSERT INTO ""_Candidate"" (""DeviceId"") (
		SELECT ""vd"".""Id""
		FROM ""View_Device"" ""vd""
		WHERE   
			""vd"".""ProjectId"" = ""_ProjectId""
			AND ""vd"".""IsInstrument"" <> '-'
			AND ""vd"".""IsDeleted"" = false 
			AND (""vd"".""NatureOfSignalName"" = ""_NatureOfSignal"")
		INTERSECT
		SELECT ""Id"" FROM ""View_DeviceRackAttributesInColumns"" ""vdr""
			WHERE	(""vdr"".""PLCNumber"" = ""_PLC"") AND ""vdr"".""ChannelNo"" IS NOT NULL
	);

	SELECT COUNT(""vdr"".""ChannelNo"") INTO ""_Cnt""
	FROM ""View_DeviceRackAttributesInColumns"" ""vdr""
	INNER JOIN ""_Candidate"" ON ""_Candidate"".""DeviceId""=""vdr"".""Id"";
	DROP table ""_Candidate"";
	RETURN ""_Cnt"";
END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spSparesReportPLCSummary""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$
	
BEGIN

CREATE TEMP TABLE ""_Capacity""(""Total Channels"" INT, ""Used Channels"" INT, ""PLC Number"" character varying(25), ""Nature of Signal"" character varying(10)) ON COMMIT DROP;

INSERT INTO ""_Capacity""
	SELECT 
		SUM(CASE 
            WHEN ""No of Slots or Channels"" ~ E'^\\d+$' THEN CAST(""No of Slots or Channels"" AS INTEGER)
            ELSE 0
        END) AS ""Total Channels"",
		0::int AS ""Used Channels"",
		""View_NonInstrumentList"".""PLC Number"",
		""View_NonInstrumentList"".""Nature of Signal""
	FROM ""View_NonInstrumentList"" 
	WHERE ""ProjectId"" = ""_ProjectId""
	AND ""View_NonInstrumentList"".""Nature of Signal"" NOT IN ('DP', 'PA')
	AND ""Device Type"" = 'Slot number in a Rack'
	AND ""View_NonInstrumentList"".""IsDeleted"" = false
	GROUP BY 
		""View_NonInstrumentList"".""PLC Number"",
		""View_NonInstrumentList"".""Nature of Signal"";
		
	UPDATE ""_Capacity""
 	SET ""Used Channels"" = public.""fnSlotChannelsInUseByPLC""(""PLC Number"", ""Nature of Signal"", ""_ProjectId"")
	where ""PLC Number"" is not null and ""Nature of Signal"" is not null;

 	INSERT INTO ""_Capacity""
 	SELECT SUM(""Total Channels""), SUM(""Used Channels""), 'Total For All PLCs', ""Nature of Signal""
 	FROM ""_Capacity"" 
 	GROUP BY ""Nature of Signal"";
 
	Open ""resultData"" FOR
	 SELECT * FROM ""_Capacity"";
	
	return;
END;
$BODY$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
