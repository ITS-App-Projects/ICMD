using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSparesReport : Migration
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
            ELSE null
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
	COALESCE(CAST(""View_NonInstrumentList"".""No of Slots or Channels"" AS INTEGER), null) AS ""Total Channels"",
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
            ELSE null
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
