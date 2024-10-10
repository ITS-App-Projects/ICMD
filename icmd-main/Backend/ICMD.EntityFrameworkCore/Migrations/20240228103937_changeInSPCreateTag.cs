using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class changeInSPCreateTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE public.""spCreateTag""(
	IN ""_Tag"" character varying,
	IN ""_Process"" character varying,
	IN ""_SubProcess"" character varying,
	IN ""_Stream"" character varying,
	IN ""_SequenceNumber"" character varying,
	IN ""_EquipmentIdentifier"" character varying,
	IN ""_EquipmentCode"" character varying,
	IN ""_ProjectId"" uuid,
	IN ""_Suffix"" character varying DEFAULT NULL::character varying,
	IN ""_CreateMissing"" boolean DEFAULT false,
	IN ""_Quiet"" boolean DEFAULT false,
	IN ""_CreatedBy"" uuid DEFAULT NULL::uuid,
	INOUT ""_TagId"" uuid DEFAULT NULL::uuid)
LANGUAGE 'plpgsql'
AS $BODY$



DECLARE
  ""_DeProcess"" character varying(255);
  ""_DeSubProcess"" character varying(255);
  ""_DeStream"" character varying(50);
  ""_DeSequenceNumber"" character varying;
  ""_DeEquipmentIdentifier"" character varying;
  ""_DeSuffix"" character varying;
  ""_EquipmentCodeId"" uuid;
  ""_StreamId"" uuid;
  ""_ProcessId"" uuid;
  ""_SubProcessId"" uuid;

BEGIN

	CALL ""spDeComposeTagGivenEquipmentCode""(""_Tag"" => ""_Tag"",
		""_EquipmentCode"" => ""_EquipmentCode"",
		""_Process"" => ""_DeProcess"",
		""_SubProcess"" => ""_DeSubProcess"",
		""_Stream"" => ""_DeStream"",
		""_SequenceNumber"" => ""_DeSequenceNumber"",
		""_EquipmentIdentifier"" => ""_DeEquipmentIdentifier"",
		""_TagSuffix"" => ""_DeSuffix"");

	-- sometimes can't distinguish between suffix and equipment identifier (and some tags don't have the latter),
	-- so combine into one field for comparison

	""_DeSuffix"" := ""_DeEquipmentIdentifier"" || ""_DeSuffix"";
	""_Suffix"" := ""_EquipmentIdentifier"" || ""_Suffix"";

	IF (""_Process"" IS NOT NULL AND ""_Process""	<> ""_DeProcess"") OR
	   (""_SubProcess"" IS NOT NULL AND ""_SubProcess"" <> ""_DeSubProcess"") OR
	  (""_Stream"" IS NOT NULL AND ""_Stream"" <> ""_DeStream"") OR
	   (""_SequenceNumber"" IS NOT NULL AND ""_SequenceNumber"" <> ""_SequenceNumber"") OR
	   (""_Suffix"" IS NOT NULL AND ""_Suffix"" <> ""_DeSuffix"") THEN
		RAISE NOTICE '%',
			COALESCE(""_DeProcess"",'') || ',' ||
			COALESCE(""_SubProcess"",'') || ',' ||
			COALESCE(""_Stream"",'') || ',' ||
			COALESCE(""_EquipmentCode"",'') || ',' ||
			COALESCE(""_SequenceNumber"",'') || ',' ||
			COALESCE(""_Suffix"", '') || ' != ' ||
			COALESCE(""_DeProcess"",'') || ',' ||
			COALESCE(""_DeSubProcess"",'') || ',' ||
			COALESCE(""_DeStream"",'') || ',' ||
			COALESCE(""_EquipmentCode"",'') || ',' ||
			COALESCE(""_DeSequenceNumber"",'') || ',' ||
			COALESCE(""_DeSuffix"", '');

		RAISE WARNING '%', FORMAT('Warning: Tag validation failed: %s', ""_Tag"");
	END IF;

	""_EquipmentCodeId"" := NULL;
	""_StreamId"" := NULL;
	""_ProcessId"" := NULL;
	""_SubProcessId"" := NULL;

	IF ""_Stream"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_StreamId"" FROM ""Stream"" WHERE ""StreamName""=""_Stream"";
		IF ""_StreamId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('Stream not found %s', ""_Stream"");
		 ELSEIF ""_StreamId"" IS NULL THEN
		 ""_StreamId"" := uuid_in(md5(random()::text || random()::text)::cstring);
		 INSERT INTO ""Stream""(""Id"",""StreamName"",""Description"", ""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES (
				""_StreamId"",
				""_Stream"",'',""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;


	IF ""_Process"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_ProcessId"" FROM ""Process"" WHERE ""ProcessName""=""_Process"";
		IF ""_ProcessId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('Process not found %s', ""_Process"");
		 ELSEIF ""_ProcessId"" IS NULL THEN
		 	""_ProcessId"" := uuid_in(md5(random()::text || random()::text)::cstring);
			INSERT INTO ""Process""(""Id"",""ProcessName"",""Description"",""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES
			(""_ProcessId"", ""_Process"",'',""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;

	IF ""_SubProcess"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_SubProcessId"" FROM ""SubProcess"" WHERE ""SubProcessName""=""_SubProcess"";
		IF ""_SubProcessId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('SubProcess not found %s', ""_SubProcess"");
		 ELSEIF ""_SubProcessId"" IS NULL THEN
		 	""_SubProcessId"" := uuid_in(md5(random()::text || random()::text)::cstring);
			INSERT INTO ""SubProcess""(""Id"",""SubProcessName"",""Description"", ""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES
			(""_SubProcessId"",
			 ""_SubProcess"",'',""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;

	IF ""_EquipmentCode"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_EquipmentCodeId"" FROM ""EquipmentCode"" WHERE ""Code""=""_EquipmentCode"";
		IF ""_EquipmentCodeId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('EquipmentCode not found %s', ""_EquipmentCode"");
		 ELSEIF ""_EquipmentCodeId"" IS NULL THEN
		   ""_EquipmentCodeId"" := uuid_in(md5(random()::text || random()::text)::cstring);
			INSERT INTO ""EquipmentCode""(""Id"",""Code"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"")
			VALUES (""_EquipmentCodeId"",
					""_EquipmentCode"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;

	IF ""_Quiet"" = false THEN RAISE NOTICE '%', COALESCE(""_Tag"", '(null) Tag');
	END IF;

	""_TagId"" := uuid_in(md5(random()::text || random()::text)::cstring);

	INSERT INTO ""Tag"" (""Id"",""TagName"", ""ProcessId"", ""SubProcessId"", ""StreamId"", ""SequenceNumber"", ""EquipmentIdentifier"", ""EquipmentCodeId"",
					   ""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"",""ProjectId"") VALUES (
		""_TagId"",
		""_Tag"",
		""_ProcessId"",
		""_SubProcessId"",
		""_StreamId"",
		""_SequenceNumber"",
		""_EquipmentIdentifier"",
		""_EquipmentCodeId"",
		""_CreatedBy"",timezone('utc', now()),true,false,""_ProjectId"");
END;
$BODY$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
