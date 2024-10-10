using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SpImportOMServiceDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spImportOMServiceDescriptions""(
	IN ""_CreatedBy"" uuid)
LANGUAGE 'plpgsql'
AS $BODY$

BEGIN

INSERT INTO ""ServiceZone"" (""Id"",""Zone"", ""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") 
	SELECT uuid_in(md5(random()::text || random()::text)::cstring) as ""Id"", ""OMServiceDescriptionImport"".""Area"", ""OMServiceDescriptionImport"".""ProjectId"",
	""_CreatedBy"" as ""CreatedBy"",timezone('utc', now()) as ""CreatedDate"",true as ""IsActive"",false as ""IsDeleted""
	FROM ""OMServiceDescriptionImport""
	WHERE ""OMServiceDescriptionImport"".""Area"" IS NOT NULL and ""OMServiceDescriptionImport"".""Area"" != '' and ""OMServiceDescriptionImport"".""Area"" NOT IN (
		SELECT ""imp"".""Area"" FROM ""OMServiceDescriptionImport"" AS ""imp""
		INNER JOIN ""ServiceZone"" ON ""imp"".""Area"" = ""ServiceZone"".""Zone""
		GROUP BY ""imp"".""Area"")
	GROUP BY ""OMServiceDescriptionImport"".""Area"", ""OMServiceDescriptionImport"".""ProjectId"";

INSERT INTO ""ServiceBank"" (""Id"",""Bank"", ""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") 
	SELECT uuid_in(md5(random()::text || random()::text)::cstring) as ""Id"",""OMServiceDescriptionImport"".""Bank"", ""OMServiceDescriptionImport"".""ProjectId"",
	""_CreatedBy"" as ""CreatedBy"",timezone('utc', now()) as ""CreatedDate"",true,false
	FROM ""OMServiceDescriptionImport""
	WHERE ""OMServiceDescriptionImport"".""Bank"" IS NOT NULL and ""OMServiceDescriptionImport"".""Bank"" != '' and ""OMServiceDescriptionImport"".""Bank"" NOT IN (
		SELECT ""imp"".""Bank"" FROM ""OMServiceDescriptionImport"" AS ""imp""
		INNER JOIN ""ServiceBank"" ON ""imp"".""Bank"" = ""ServiceBank"".""Bank""
		GROUP BY ""imp"".""Bank"")
	GROUP BY ""OMServiceDescriptionImport"".""Bank"", ""OMServiceDescriptionImport"".""ProjectId"";

INSERT INTO ""ServiceTrain"" (""Id"",""Train"", ""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"")  
	SELECT uuid_in(md5(random()::text || random()::text)::cstring) as ""Id"", ""OMServiceDescriptionImport"".""Train"", ""OMServiceDescriptionImport"".""ProjectId"",
	""_CreatedBy"" as ""CreatedBy"",timezone('utc', now()) as ""CreatedDate"",true,false
	FROM ""OMServiceDescriptionImport""
	WHERE ""OMServiceDescriptionImport"".""Train"" IS NOT NULL and ""OMServiceDescriptionImport"".""Train"" != '' and ""OMServiceDescriptionImport"".""Train"" NOT IN (
		SELECT ""imp"".""Train"" FROM ""OMServiceDescriptionImport"" AS ""imp""
		INNER JOIN ""ServiceTrain"" ON ""imp"".""Train"" = ""ServiceTrain"".""Train""
		GROUP BY ""imp"".""Train"")
	GROUP BY ""OMServiceDescriptionImport"".""Train"", ""OMServiceDescriptionImport"".""ProjectId"";

UPDATE ""Device"" SET 
	""ServiceDescription"" = 
	(
		SELECT ""OMServiceDescriptionImport"".""ServiceDescription"" 
		FROM ""OMServiceDescriptionImport"" 
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag"" 
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
	,""ServiceZoneId"" = 
	(
		SELECT ""ServiceZone"".""Id""
		FROM ""ServiceZone"" 
			INNER JOIN ""OMServiceDescriptionImport"" ON ""OMServiceDescriptionImport"".""Area""=""ServiceZone"".""Zone""
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag""
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
	,""ServiceBankId"" = 
	(
		SELECT ""ServiceBank"".""Id""
		FROM ""ServiceBank"" 
			INNER JOIN ""OMServiceDescriptionImport"" ON ""OMServiceDescriptionImport"".""Bank""=""ServiceBank"".""Bank""
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag""
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
	,""ServiceTrainId"" = 
	(
		SELECT ""ServiceTrain"".""Id""
		FROM ""ServiceTrain"" 
			INNER JOIN ""OMServiceDescriptionImport"" ON ""OMServiceDescriptionImport"".""Train""=""ServiceTrain"".""Train""
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag""
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
	,""Service"" = 
	(
		SELECT ""OMServiceDescriptionImport"".""Service"" 
		FROM ""OMServiceDescriptionImport"" 
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag"" 
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
	,""Variable"" = 
	(
		SELECT ""OMServiceDescriptionImport"".""Variable"" 
		FROM ""OMServiceDescriptionImport"" 
			INNER JOIN ""Tag"" ON ""Tag"".""TagName"" = ""OMServiceDescriptionImport"".""Tag"" 
		WHERE ""Tag"".""Id"" = ""Device"".""TagId""
		order by ""OMServiceDescriptionImport"".""CreatedDate"" desc
		limit 1
	)
WHERE ""Device"".""TagId"" IN (
	SELECT ""Tag"".""Id"" FROM ""Tag"" INNER JOIN ""OMServiceDescriptionImport"" ON ""OMServiceDescriptionImport"".""Tag"" = ""Tag"".""TagName""
	WHERE ""OMServiceDescriptionImport"".""ServiceDescription"" IS NOT NULL AND ""Tag"".""ProjectId"" = (SELECT ""ProjectId"" FROM ""OMServiceDescriptionImport""
	                                                                                        LIMIT 1)
) AND ""Device"".""IsDeleted""=false AND ""Device"".""IsActive""=true;
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
