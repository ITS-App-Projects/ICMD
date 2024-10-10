using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePnIdTag_SPs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spImportPnIDTags""(
	IN ""_ProjectId"" uuid,
	IN ""_CreatedBy"" uuid)
LANGUAGE 'plpgsql'
AS $BODY$

DECLARE
  ""_Process"" character varying(255);
  ""_Subprocess"" character varying(255);
  ""_Stream"" character varying(50);
  ""_Sequencenumber"" character varying(10);
  ""_Equipmentidentifier"" character varying(10);
  ""_Equipmentcode"" character varying(50);
  ""_Suffix"" character varying(50);
  ""_Tagid"" uuid;
  ""_Tag"" character varying(25);
  ""_Description"" character varying(255);
  ""_Switches"" character varying(100);
  ""_Pipelinetag"" character varying(50);
  ""_Onskid"" character varying(50);
  ""_Failstate"" character varying(255);
  ""_Pnpid"" int;
  ""_Docid"" uuid;
  ""_Docnum"" character varying(255);
  ""_Docver"" character varying(255);
  ""_Docrev"" character varying(255);
  ""_Skidid"" uuid;
  ""_Failstateid"" uuid;
  ""pnid"" refcursor;
  ""_Vdpdocnum"" character varying(255);
  ""_Skidtagid"" uuid;

BEGIN

	--SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
	--BEGIN  --(EXCEPTION)

		
		IF EXISTS (select * from ""SSISEquipmentList"") THEN 
			delete from ""PnIdTag"" where ""PnIdTag"".""Source""='E';
		END IF;
		IF EXISTS (select * from ""SSISInstruments"") THEN 
			delete from ""PnIdTag"" where ""PnIdTag"".""Source""='I';
		END IF;
		IF EXISTS (select * from ""SSISValveList"") THEN 
			delete from ""PnIdTag"" where ""PnIdTag"".""Source""='V';
		END IF;
		IF EXISTS (select * from ""SSISFittings"") THEN 
			delete from ""PnIdTag"" where ""PnIdTag"".""Source""='F';
		END IF;

		OPEN ""pnid"" for 
			select cast(""l"".""PnPId"" as int) as ""_Pnpid"", ""l"".""Tag"", ""l"".""DWGTitle"",""l"".""Rev"",""l"".""Version"" ,""l"".""Description"",NULL,NULL, 
				""l"".""ProcessNumber"", ""l"".""SubProcess"", ""l"".""Stream"", ""l"".""EquipmentCode"", ""l"".""SequenceNumber"", ""l"".""EquipmentIdentifier"", 
				""l"".""OnSkid"", NULL
			from ""SSISEquipmentList"" ""l""
			union
			select cast(""l"".""PnPId"" as int) as ""_Pnpid"", ""l"".""Tag"",""l"".""DWGTitle"",""l"".""Rev"",""l"".""Version"",""l"".""Description"",""l"".""Switches"",""l"".""PipeLinesTag"" ,
				""l"".""ProcessNumber"", ""l"".""SubProcess"", ""l"".""Stream"", ""l"".""EquipmentCode"", ""l"".""SequenceNumber"", ""l"".""EquipmentIdentifier"", 
				""l"".""OnSkid"", ""l"".""Failure""	
			from ""SSISValveList"" ""l"" 
			union
			select cast(""l"".""PnPId"" as int) as ""_Pnpid"", ""l"".""Tag"",""l"".""DWGTitle"",""l"".""Rev"",""l"".""Version"",""l"".""Description"",NULL,""l"".""PipeLinesTag"" ,
				""l"".""ProcessNumber"", ""l"".""SubProcess"", ""l"".""Stream"", ""l"".""EquipmentCode"", ""l"".""SequenceNumber"", ""l"".""EquipmentIdentifier"", 
				NULL, NULL	
			from ""SSISInstruments"" ""l"" 
			union
			select cast(""l"".""PnPId"" as int) as ""_Pnpid"", ""l"".""Tag"",""l"".""DWGTitle"",""l"".""Rev"",""l"".""Version"",""l"".""Description"",NULL,NULL ,
				""l"".""ProcessNumber"", ""l"".""SubProcess"", ""l"".""Stream"", ""l"".""EquipmentCode"", ""l"".""SequenceNumber"", ""l"".""EquipmentIdentifier"", 
				""l"".""OnSkid"", NULL
			from ""SSISFittings"" ""l"";
		while(1=1) LOOP
			fetch next from ""pnid"" into ""_Pnpid"",""_Tag"",""_Docnum"",""_Docrev"",""_Docver"",""_Description"",""_Switches"",""_Pipelinetag"",
				""_Process"",""_Subprocess"",""_Stream"",""_Equipmentcode"",""_Sequencenumber"",""_Equipmentidentifier"",""_Onskid"",""_Failstate"";
			
			IF Not found THEN EXIT;
			END IF;
		
			IF (substring(""_Tag"" FROM 1 FOR 3) ~ '[0-9][0-9][0-9]') = false THEN
				RAISE WARNING '%', FORMAT('invalid tag %s (skipped)', ""_Tag"");
				continue;
			END IF;
			
			""_Vdpdocnum"" := ""_Docnum"" || COALESCE('-'||""_Docrev"" || COALESCE('-'||""_Docver"",''),'');

			""_Docid"":=null;
			SELECT ""doc"".""Id"" INTO ""_Docid""
			FROM ""ReferenceDocument"" AS ""doc"" 
				INNER JOIN ""ReferenceDocumentType"" AS ""doctype"" ON ""doctype"".""Type"" = 'P&ID'
			WHERE ""doc"".""DocumentNumber"" = ""_Docnum""
				AND COALESCE(""doc"".""Version"", '') = COALESCE(""_Docver"",'')
				AND COALESCE(""doc"".""Revision"",'') = COALESCE(""_Docrev"",'');
				IF ""_Docid"" is null THEN
				
				CALL public.""spCreateReferenceDocument""(""_ReferenceDocumentType"" => 'P&ID',
					""_DocumentNumber"" => ""_Vdpdocnum"",
					""_Version"" => ""_Docver"",
					""_Revision"" => ""_Docrev"",
					""_ProjectId"" => ""_ProjectId"",
					""_ReferenceDocumentId"" => ""_Docid"",
					""_CreatedBy"" => ""_CreatedBy"");
					
			END IF;

			""_Tagid"" := null;
			
			select ""Id"" INTO ""_Tagid"" from ""Tag"" where ""TagName""=""_Tag"";
			
			IF ""_Tagid"" is null THEN
				CALL public.""spCreateTag""(""_Tag"" => ""_Tag"",
					""_Process"" => ""_Process"",
					""_SubProcess"" => ""_Subprocess"",
					""_Stream"" => ""_Stream"",
					""_SequenceNumber"" => ""_Sequencenumber"",
					""_EquipmentIdentifier"" => ""_Equipmentidentifier"",
					""_EquipmentCode"" => ""_Equipmentcode"",
					""_Suffix"" => ""_Suffix"",
					""_CreateMissing"" => true,
					""_Quiet"" => false,
					""_ProjectId"" => ""_ProjectId"",
					""_TagId"" => ""_Tagid"",
										 ""_CreatedBy"" => ""_CreatedBy"");
			END IF;

			""_Skidid"" := null;
			IF ""_Onskid"" is not null and ""_Onskid"" != '' THEN
				select ""Skid"".""Id"" INTO ""_Skidid"" from ""Skid"" inner join ""Tag"" on ""Tag"".""Id""=""Skid"".""TagId"" where ""Tag"".""TagName""=""_Onskid"";
				IF ""_Skidid"" is null THEN
					
				IF (regexp_matches(substring(""_Onskid"", 1, 3), '[0-9][0-9][0-9]') IS NULL) = false THEN
						RAISE WARNING '%', FORMAT('invalid skid tag %s (skipped)', ""_Onskid"");
					else
					RAISE NOTICE '_Onskid: %', ""_Onskid"";
						CALL public.""spCreateSkid""(""_SkidTag"" => ""_Onskid"",
							""_EquipmentCode"" => 'K',
							""_Quiet"" => 'true',
							""_SkidId"" => ""_Skidid"",
							""_ProjectId"" => ""_ProjectId"",
							""_TagId"" => ""_Skidtagid"",
							""_CreatedBy"" => ""_CreatedBy"");
					END IF;
				END IF;
			END IF;
		
	
			""_Failstateid"" := null;
			IF ""_Failstate"" is not null and ""_Failstate"" != '' THEN
				select ""Id"" INTO ""_Failstateid"" from ""FailState"" where ""FailState"".""FailStateName""=""_Failstate"";
				IF ""_Failstateid"" is null THEN
					""_Failstateid"" := uuid_in(md5(random()::text || random()::text)::cstring);
					insert into ""FailState""(""Id"",""FailStateName"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") values (
						""_Failstateid"",""_Failstate"",""_CreatedBy"",timezone('utc', now()),true,false);
				END IF;
			END IF;

			IF NOT EXISTS(select ""Id"" from ""PnIdTag"" where ""TagId""=""_Tagid"") THEN
			
				insert into ""PnIdTag""(""Id"", ""PnPId"", ""DocumentReferenceId"", ""Description"", ""Switches"", ""PipelineTag"", ""TagId"", ""SkidId"", ""FailStateId"",
									 ""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"")
				values (uuid_in(md5(random()::text || random()::text)::cstring), ""_Pnpid"", ""_Docid"", ""_Description"", ""_Switches"", ""_Pipelinetag"", ""_Tagid"", ""_Skidid"", ""_Failstateid"",
					  ""_CreatedBy"",timezone('utc', now()),true,false);
			END IF;
		END LOOP;
		close ""pnid"";

		delete from ""SSISEquipmentList"";
		delete from ""SSISFittings"";
		delete from ""SSISInstruments"";
		delete from ""SSISValveList"";

		-- COMMIT;
	--EXCEPTION WHEN OTHERS THEN
		--RAISE NOTICE '% %', SQLERRM, SQLSTATE;
		--CALL public.""usp_GetErrorInfo""();
		 --ROLLBACK;
		-- COMMIT;
	--END;
	

	--RETURN;
END; 
$BODY$;");


			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spCreateReferenceDocument""(
	IN ""_ReferenceDocumentType"" character varying,
	IN ""_DocumentNumber"" character varying,
	IN ""_ProjectId"" uuid,
	IN ""_URL"" character varying DEFAULT NULL::character varying,
	IN ""_Description"" character varying DEFAULT NULL::character varying,
	IN ""_Version"" character varying DEFAULT NULL::character varying,
	IN ""_Revision"" character varying DEFAULT NULL::character varying,
	IN ""_Date"" date DEFAULT NULL::date,
	IN ""_Sheet"" character varying DEFAULT NULL::character varying,
	IN ""_CreatedBy"" uuid DEFAULT NULL::uuid,
	INOUT ""_ReferenceDocumentId"" uuid DEFAULT NULL::uuid)
LANGUAGE 'plpgsql'
AS $BODY$

DECLARE
  ""_DocTypeId"" uuid;
  ""_NewDocumentNumber"" character varying(100);
  ""_IsVDPDocumentNumber"" boolean = false;
  ""_NewRevision"" character varying(50);
  ""_NewVersion"" character varying(50);

BEGIN
	SELECT ""Id"" INTO ""_DocTypeId"" FROM ""ReferenceDocumentType"" WHERE ""Type""=""_ReferenceDocumentType"";
	IF ""_DocTypeId"" IS NULL THEN
		RAISE EXCEPTION '%', FORMAT('unknown document type %s', ""_ReferenceDocumentType"");
	END IF;
	
	SELECT 
		""ref"".""DocType"" || '-' || ""ref"".""Originator"" || '-' || ""ref"".""Function"" || '-' || ""ref"".""Area"" || '-' || ""ref"".""SubArea"" || '-' || ""ref"".""Zone"" || '-' || ""ref"".""DocNumber"",
		""ref"".""Revision"",
		""ref"".""Version"",
		""ref"".""IsVDPDocumentNumber"" INTO ""_NewDocumentNumber"", ""_NewRevision"", ""_NewVersion"", ""_IsVDPDocumentNumber""
	FROM public.""fnParseDocumentReference""(""_DocumentNumber"", false) AS ref;

	IF ""_IsVDPDocumentNumber"" = true THEN
		RAISE NOTICE '%', ""_NewDocumentNumber"" || COALESCE(', ' || ""_NewRevision"", '') || COALESCE(', ' || ""_NewVersion"", '') || COALESCE(', VDP: ' || CAST(""_IsVDPDocumentNumber"" as VARCHAR(10)), '');
		""_ReferenceDocumentId"":=uuid_in(md5(random()::text || random()::text)::cstring);
		INSERT INTO ""ReferenceDocument""(
			""Id"",
			""ReferenceDocumentTypeId"", 
			""DocumentNumber"", 
			""URL"", 
			""Description"", 
			""Version"", 
			""Revision"", 
			""Date"", 
			""Sheet"",
			""IsVDPDocumentNumber"",
			""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"")
		VALUES (
			""_ReferenceDocumentId"",
			""_DocTypeId"", 
			""_NewDocumentNumber"", 
			""_URL"", 
			""_Description"", 
			""_NewVersion"",
			""_NewRevision"",
			""_Date"",
			""_Sheet"",
			 (CASE WHEN ""_IsVDPDocumentNumber"" THEN true ELSE false END),""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
	ELSE
		RAISE NOTICE '%', ""_DocumentNumber"" || COALESCE(', ' || ""_Revision"", '') || COALESCE(', ' || ""_Version"", '') || COALESCE(', ' || ""_Sheet"", '') || COALESCE(', VDP: ' || CAST(""_IsVDPDocumentNumber"" as VARCHAR(10)), '');
		""_ReferenceDocumentId"":=uuid_in(md5(random()::text || random()::text)::cstring);
		INSERT INTO ""ReferenceDocument"" (
			""Id"",
			""ReferenceDocumentTypeId"", 
			""DocumentNumber"", 
			""URL"", 
			""Description"", 
			""Version"", 
			""Revision"", 
			""Date"", 
			""Sheet"",
			""IsVDPDocumentNumber"",""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"")
		VALUES (
			""_ReferenceDocumentId"",
			""_DocTypeId"",
			""_DocumentNumber"",
			""_URL"",
			""_Description"",
			""_Version"",
			""_Revision"",
			""_Date"",
			""_Sheet"",
			 (CASE WHEN ""_IsVDPDocumentNumber"" THEN true ELSE false END),""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
			 
	END IF;
	--RETURN;
END; 
$BODY$;");


			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spCreateTag""(
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
		 INSERT INTO ""Stream""(""Id"",""StreamName"",""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES (
				""_StreamId"",
				""_Stream"",""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;

	
	IF ""_Process"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_ProcessId"" FROM ""Process"" WHERE ""ProcessName""=""_Process"";
		IF ""_ProcessId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('Process not found %s', ""_Process"");
		 ELSEIF ""_ProcessId"" IS NULL THEN
		 	""_ProcessId"" := uuid_in(md5(random()::text || random()::text)::cstring);
			INSERT INTO ""Process""(""Id"",""ProcessName"",""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES
			(""_ProcessId"", ""_Process"",""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
		END IF;
	END IF;

	IF ""_SubProcess"" IS NOT NULL THEN
		SELECT ""Id"" INTO ""_SubProcessId"" FROM ""SubProcess"" WHERE ""SubProcessName""=""_SubProcess"";
		IF ""_SubProcessId"" IS NULL AND ""_CreateMissing""=false THEN
			RAISE WARNING '%', FORMAT('SubProcess not found %s', ""_SubProcess"");
		 ELSEIF ""_SubProcessId"" IS NULL THEN
		 	""_SubProcessId"" := uuid_in(md5(random()::text || random()::text)::cstring);
			INSERT INTO ""SubProcess""(""Id"",""SubProcessName"",""ProjectId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES 
			(""_SubProcessId"",
			 ""_SubProcess"",""_ProjectId"",""_CreatedBy"",timezone('utc', now()),true,false);
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
					""_EquipmentCode"",""CreatedBy"",timezone('utc', now()),true,false);
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


			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spCreateSkid""(
	IN ""_SkidTag"" character varying,
	IN ""_ProjectId"" uuid,
	IN ""_EquipmentCode"" character varying DEFAULT 'K'::character varying,
	IN ""_Quiet"" boolean DEFAULT false,
	IN ""_CreatedBy"" uuid DEFAULT NULL::uuid,
	INOUT ""_SkidId"" uuid DEFAULT NULL::uuid,
	INOUT ""_TagId"" uuid DEFAULT NULL::uuid)
LANGUAGE 'plpgsql'
AS $BODY$

DECLARE
  ""_DeProcess"" character varying(255);
  ""_DeSubProcess"" character varying(255);
  ""_DeStream"" character varying(50);
  ""_DeSequenceNumber"" character varying(10);
  ""_DeEquipmentIdentifier"" character varying(10);
  ""_DeSuffix"" character varying(10);

BEGIN
	IF NOT EXISTS (SELECT ""Id"" FROM ""Tag"" WHERE ""TagName""=""_SkidTag"") THEN
		CALL public.""spDeComposeTagGivenEquipmentCode""(""_Tag"" => ""_SkidTag"",
			""_EquipmentCode"" => ""_EquipmentCode"",
			""_Process"" => ""_DeProcess"",
			""_SubProcess"" => ""_DeSubProcess"",
			""_Stream"" => ""_DeStream"",
			""_SequenceNumber"" => ""_DeSequenceNumber"",
			""_EquipmentIdentifier"" => ""_DeEquipmentIdentifier"",
			""_TagSuffix"" => ""_DeSuffix"");

		CALL public.""spCreateTag""(""_Tag"" => ""_SkidTag"",
			""_Process"" => ""_DeProcess"",
			""_SubProcess"" => ""_DeSubProcess"",
			""_Stream"" => ""_DeStream"",
			""_SequenceNumber"" =>""_DeSequenceNumber"",
			""_EquipmentIdentifier"" =>""_DeEquipmentIdentifier"",
			""_EquipmentCode"" => ""_EquipmentCode"",
			""_ProjectId"" => ""_ProjectId"",
			""_Suffix"" => ""_DeSuffix"",
			""_CreatedBy"" => ""_CreatedBy"",
			""_Quiet"" => ""_Quiet"",
			""_TagId"" => ""_TagId"");
	END IF;
	SELECT ""Id"" INTO ""_TagId"" FROM ""Tag"" WHERE ""TagName""=""_SkidTag"";

	INSERT INTO ""Skid""(""Id"",""TagId"",""CreatedBy"",""CreatedDate"",""IsActive"",""IsDeleted"") VALUES (
		uuid_in(md5(random()::text || random()::text)::cstring),
		""_TagId"",
	""_CreatedBy"",timezone('utc', now()),true,false) RETURNING ""Id"" INTO ""_SkidId"";
END; 
$BODY$;");

			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""spDeComposeTagGivenEquipmentCode""(
	IN ""_Tag"" character varying,
	IN ""_EquipmentCode"" character varying,
	INOUT ""_Process"" character varying,
	INOUT ""_SubProcess"" character varying,
	INOUT ""_Stream"" character varying,
	INOUT ""_SequenceNumber"" character varying,
	INOUT ""_EquipmentIdentifier"" character varying,
	INOUT ""_TagSuffix"" character varying)
LANGUAGE 'plpgsql'
AS $BODY$


BEGIN
	""_Process"" := SUBSTRING(""_Tag"", 1, 3);
	""_SubProcess"" := SUBSTRING(""_Tag"", 4, 1);
	""_Stream"" := SUBSTRING(""_Tag"", 5, 1);
	""_SequenceNumber"" := SUBSTRING(""_Tag"", 6 + OCTET_LENGTH(""_EquipmentCode"")/2, 3);
	""_EquipmentIdentifier"" := SUBSTRING(""_Tag"", 9 + OCTET_LENGTH(""_EquipmentCode"")/2, 1);
	""_TagSuffix"" := SUBSTRING(""_Tag"", 10 + OCTET_LENGTH(""_EquipmentCode"")/2, 10);
END; 
$BODY$;");


			migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE public.""usp_GetErrorInfo""(
	)
LANGUAGE 'plpgsql'
AS $BODY$


DECLARE
    ""_sqlstate"" character varying;
	""_severity"" character varying;
    ""_message"" character varying;
	""_line"" character varying;
	""_procedure"" character varying;
BEGIN
    BEGIN
         
     EXCEPTION WHEN OTHERS THEN
        ""_sqlstate"" := SQLSTATE;
		""_message"" := SQLERRM;
		""_severity"" := SQLSTATE;
		""_line"" := PG_EXCEPTION_LINE;
		SELECT ""ProName"" INTO ""_procedure""
		FROM pg_stat_activity
		WHERE pg_backend_pid() = pg_stat_activity.pid;
    END;

    RAISE NOTICE 'ErrorNumber: %', ""_sqlstate"";
	RAISE NOTICE 'ErrorSeverity: %', ""_severity"";
	RAISE NOTICE 'ErrorState: %', substring(""_sqlstate"" FROM 3 FOR 2);
	RAISE NOTICE 'ErrorLine: %', ""_line"";
	RAISE NOTICE 'ErrorMessage: %', ""_message"";
	RAISE NOTICE 'ErrorProcedure: %', ""_procedure"";
   
    
END 
$BODY$;");


			migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnParseDocumentReference""(
	""_DocumentReference"" character varying,
	""_SplitMultipleReferences"" boolean)
    RETURNS TABLE(""DocumentReference"" character varying, ""DocType"" character varying, ""Originator"" character varying, ""Function"" character varying, ""Area"" character varying, ""SubArea"" character varying, ""Zone"" character varying, ""DocNumber"" character varying, ""Revision"" character varying, ""Version"" character varying, ""IsVDPDocumentNumber"" boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    ""parts"" text[];
    result_record record;
BEGIN
    IF ""_DocumentReference"" IS NULL OR ""_DocumentReference"" = '' THEN
        -- If the document reference is empty or NULL, set all fields to NULL.
       RETURN QUERY
    		 SELECT ""_DocumentReference"" as ""DocumentReference"", 
			 null::character varying as ""DocType"", 
			 null::character varying as ""Originator"", 
			 null::character varying as ""Function"", 
			 null::character varying as ""Area"", 
			 null::character varying as ""SubArea"",
			 null::character varying as ""Zone"", 
			 null::character varying as ""DocNumber"", 
			 null::character varying as ""Revision"", 
			 null::character varying as ""Version"", 
			 false as ""IsVDPDocumentNumber"";
    ELSE
        ""parts"" := string_to_array(""_DocumentReference"", '-');
        
        ""DocumentReference"" := ""_DocumentReference"";
        ""DocType"" := parts[1];
        ""Originator"" := parts[2];
        ""Function"" := parts[3];
        ""Area"" := parts[4];
        ""SubArea"" := parts[5];
        ""Zone"" := parts[6];
        ""DocNumber"" := parts[7];
        ""Revision"" := parts[8];
        ""Version"" := parts[9];

        -- Check if DocNumber is empty and set IsVDPDocumentNumber accordingly.
        IF ""DocNumber"" IS NULL OR ""DocNumber"" = '' THEN
            ""IsVDPDocumentNumber"" := FALSE;
        ELSE
            ""IsVDPDocumentNumber"" := TRUE;
        END IF;

        -- If DocNumber is empty, set all fields to NULL.
        IF ""DocNumber"" IS NULL OR ""DocNumber"" = '' THEN
             RETURN QUERY
    		 SELECT ""_DocumentReference"" as ""DocumentReference"", 
			 null::character varying as ""DocType"", 
			 null::character varying as ""Originator"", 
			 null::character varying as ""Function"", 
			 null::character varying as ""Area"", 
			 null::character varying as ""SubArea"",
			 null::character varying as ""Zone"", 
			 null::character varying as ""DocNumber"", 
			 null::character varying as ""Revision"", 
			 null::character varying as ""Version"", 
			 ""IsVDPDocumentNumber"" as ""IsVDPDocumentNumber"";
        ELSE
            RETURN QUERY
    SELECT
        ""DocumentReference"" as ""DocumentReference"",
        ""DocType"" as ""DocType"",
        ""Originator"" as ""Originator"",
        ""Function"" as ""Function"",
        ""Area"" as ""Area"",
        ""SubArea"" as ""SubArea"",
        ""Zone"" as ""Zone"",
        ""DocNumber"" as ""DocNumber"",
        ""Revision"" as ""Revision"",
        ""Version"" as ""Version"",
        ""IsVDPDocumentNumber"" as ""IsVDPDocumentNumber"";
        END IF;
    END IF;

   
END;
$BODY$;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
