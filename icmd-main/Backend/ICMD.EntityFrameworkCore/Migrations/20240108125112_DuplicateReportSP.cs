using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class DuplicateReportSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spDuplicateDPNodeAddress""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

	CREATE TEMP TABLE ""_Duplicates""(""CouplerTag"" character varying(25) NOT NULL, ""NodeAddress"" character varying) ON COMMIT DROP;

	INSERT INTO ""_Duplicates""
	SELECT ""vw"".""DP/DP Coupler"", ""vw"".""DP Node Address""
	FROM ""View_InstrumentListLive"" ""vw"" 
	WHERE ""vw"".""ProjectId"" = ""_ProjectId"" AND ""vw"".""DP/DP Coupler"" IS NOT NULL AND ""vw"".""DP Node Address"" IS NOT NULL 
	GROUP BY ""vw"".""DP/DP Coupler"", ""vw"".""DP Node Address""
	HAVING COUNT(*)>1;

	Open ""resultData"" FOR
	SELECT ""vw"".*
	FROM ""_Duplicates"" ""dup"" 
		INNER JOIN ""View_InstrumentListLive"" ""vw"" ON ""vw"".""DP/DP Coupler""=""dup"".""CouplerTag"" AND ""vw"".""DP Node Address""=""dup"".""NodeAddress""
	ORDER BY ""vw"".""DP/DP Coupler"", ""vw"".""DP Node Address"", ""vw"".""TagName"";

	RETURN;

END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spDuplicatePANodeAddress""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

CREATE TEMP TABLE ""_Duplicates""(""CouplerTag"" character varying(25) NOT NULL, ""NodeAddress"" character varying) ON COMMIT DROP;

INSERT INTO ""_Duplicates""
	SELECT ""vw"".""DP/PA Coupler"", ""vw"".""PA Node Address""
	FROM ""View_InstrumentListLive"" ""vw"" 
	WHERE ""vw"".""ProjectId"" = ""_ProjectId"" AND ""vw"".""DP/PA Coupler"" IS NOT NULL AND ""vw"".""PA Node Address"" IS NOT NULL 
	GROUP BY ""vw"".""DP/PA Coupler"", ""vw"".""PA Node Address""
	HAVING COUNT(*)>1;

Open ""resultData"" FOR
SELECT ""vw"".*
FROM ""_Duplicates"" ""dup"" 
	INNER JOIN ""View_InstrumentListLive"" ""vw"" ON ""vw"".""DP/PA Coupler""=""dup"".""CouplerTag"" AND ""vw"".""PA Node Address""=""dup"".""NodeAddress""
ORDER BY ""vw"".""DP/PA Coupler"", ""vw"".""PA Node Address"", ""vw"".""TagName"";

RETURN;

END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spDuplicateRackSlotChannels""(
	IN ""_ProjectId"" uuid,
	INOUT ""resultData"" refcursor)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

CREATE TEMP TABLE ""_Duplicates""(""RackNo"" character varying(25), ""Slot"" character varying, ""Channel"" character varying) ON COMMIT DROP;

INSERT INTO ""_Duplicates""
	SELECT ""vw"".""Rack No"", ""vw"".""Slot No"", ""vw"".""Channel No""
	FROM ""View_InstrumentListLive"" ""vw""
	WHERE ""vw"".""ProjectId"" = ""_ProjectId"" AND ""vw"".""Slot No"" IS NOT NULL AND ""vw"".""Channel No"" IS NOT NULL
	GROUP BY ""vw"".""Rack No"", ""vw"".""Slot No"", ""vw"".""Channel No""
	HAVING COUNT(*)>1;

Open ""resultData"" FOR
SELECT ""vw"".*
FROM ""_Duplicates"" ""dup"" 
	INNER JOIN ""View_InstrumentListLive"" ""vw"" ON ""vw"".""Rack No""=""dup"".""RackNo"" AND ""vw"".""Slot No""=""dup"".""Slot"" AND ""vw"".""Channel No""=""dup"".""Channel""
ORDER BY ""vw"".""Rack No"", ""vw"".""Slot No"", ""vw"".""Channel No"", ""vw"".""TagName"";

RETURN;

END; 
$BODY$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
