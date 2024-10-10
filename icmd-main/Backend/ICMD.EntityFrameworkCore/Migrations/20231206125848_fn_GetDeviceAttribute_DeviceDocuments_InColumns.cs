using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class fn_GetDeviceAttribute_DeviceDocuments_InColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.""fn_concat_agg""(
                	text,
                	text)
                    RETURNS text
                    LANGUAGE 'plpgsql'
                    COST 100
                    VOLATILE PARALLEL UNSAFE
                AS $BODY$
                BEGIN
                     IF $1 IS NULL THEN
                        RETURN $2;
                    ELSIF $2 IS NULL THEN
                        RETURN $1;
                    ELSE
                        RETURN $1 || ', ' || $2;
                    END IF;
                END;
                $BODY$;
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE AGGREGATE public.""Aggregate_Concat""(text) (
                    SFUNC = fn_concat_agg,
                    STYPE = text ,
                    FINALFUNC_MODIFY = READ_ONLY,
                    MFINALFUNC_MODIFY = READ_ONLY
                );
            ");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnGetDeviceAttributesInColumns""(
	""_DeviceId"" uuid)
    RETURNS TABLE(""GSDType"" text, ""ControlPanelNumber"" text, ""PLCSlotNumber"" text, ""DPNodeAddress"" text, ""PLCNumber"" text, ""DPDPCoupler"" text, ""AFDHubNumber"" text, ""ChannelNo"" text, ""DPPACoupler"" text, ""PANodeAddress"" text, ""RackNo"" text, ""SlotNo"" text, ""CalibratedRangeMin"" text, ""CalibratedRangeMax"" text, ""CalibratedRangeUnits"" text, ""ProcessRangeMin"" text, ""ProcessRangeMax"" text, ""ProcessRangeUnits"" text, ""RLPosition"" text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


            BEGIN
                RETURN QUERY
                SELECT     
                    MIN(CASE ""Name"" WHEN 'GSD Type' THEN ""Value"" END) AS ""GSDType"", 
                    MIN(CASE ""Name"" WHEN 'Control Panel Number' THEN ""Value"" END) AS ""ControlPanelNumber"", 
                    MIN(CASE ""Name"" WHEN 'PLC Slot Number' THEN ""Value"" END) AS ""PLCSlotNumber"", 
                    MIN(CASE ""Name"" WHEN 'DP Node Address' THEN ""Value"" END) AS ""DPNodeAddress"", 
                    MIN(CASE ""Name"" WHEN 'PLC Number' THEN ""Value"" END) AS ""PLCNumber"", 
                    MIN(CASE ""Name"" WHEN 'DP/DP Coupler' THEN ""Value"" END) AS ""DPDPCoupler"", 
                    MIN(CASE ""Name"" 
                            WHEN 'AFD Number' THEN ""Value"" 
                            WHEN 'DP Hub Number' THEN ""Value""
                        END) AS ""AFDHubNumber"", 
                    MIN(CASE ""Name"" WHEN 'Channel Number' THEN ""Value"" END) AS ""ChannelNo"", 
                    MIN(CASE ""Name"" WHEN 'DP/PA Coupler' THEN ""Value"" END) AS ""DPPACoupler"", 
                    MIN(CASE ""Name"" WHEN 'PA Node Address' THEN ""Value"" END) AS ""PANodeAddress"",
                    
                    MIN(CASE ""Name"" 
                            WHEN 'RIO Rack Number' THEN ""Value"" 
                            WHEN 'VMB Rack Number' THEN ""Value"" 
                        END) AS ""RackNo"", 
                    MIN(CASE ""Name"" WHEN 'Slot Number' THEN ""Value"" END) AS ""SlotNo"", 
                    MIN(CASE ""Name"" WHEN 'Calibrated Range Min' THEN ""Value"" END) AS ""CalibratedRangeMin"", 
                    MIN(CASE ""Name"" WHEN 'Calibrated Range Max' THEN ""Value"" END) AS ""CalibratedRangeMax"", 
                    MIN(CASE ""Name"" WHEN 'Calibrated Range Units' THEN ""Value"" END) AS ""CalibratedRangeUnits"",
                    MIN(CASE ""Name"" WHEN 'Process Range Min' THEN ""Value"" END) AS ""ProcessRangeMin"", 
                    MIN(CASE ""Name"" WHEN 'Process Range Max' THEN ""Value"" END) AS ""ProcessRangeMax"", 
                    MIN(CASE ""Name"" WHEN 'Process Range Units' THEN ""Value"" END) AS ""ProcessRangeUnits"",
                    MIN(CASE ""Name"" WHEN 'RL / Position' THEN ""Value"" END) AS ""RLPosition""
                FROM
                    ""View_AllAttributes"" 
                RIGHT OUTER JOIN ""Device"" ON ""Device"".""Id"" = ""View_AllAttributes"".""Id"" WHERE ""Device"".""Id""=""_DeviceId"";
                
                RETURN;
            END;
            
$BODY$;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.""fnGetDeviceDocumentsInColumns""(
                	""_DeviceId"" uuid)
                    RETURNS TABLE(""DatasheetNumber"" text, ""SheetNumber"" text, ""HookupDrawing"" text, ""TerminationDiagram"" text, ""PIDNumber"" text, ""LayoutDrawing"" text, ""ArchitecturalDrawing"" text, ""FunctionalDescriptionDocument"" text, ""ProductProcurementNumber"" text) 
                    LANGUAGE 'plpgsql'
                    COST 100
                    VOLATILE PARALLEL UNSAFE
                    ROWS 1000
                
                AS $BODY$
                BEGIN
                    RETURN QUERY 
                    SELECT 
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Datasheet' THEN ""DocumentNumber"" END) AS ""DatasheetNumber"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Datasheet' THEN ""Sheet"" END) AS ""SheetNumber"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Hookup Drawing' THEN ""DocumentNumber"" END) AS ""HookupDrawing"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Termination Diagram' THEN ""DocumentNumber"" END) AS ""TerminationDiagram"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'P&Id' THEN ""DocumentNumber"" END) AS ""PIDNumber"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Layout Drawing' THEN ""DocumentNumber"" END) AS ""LayoutDrawing"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Architectural Drawing' THEN ""DocumentNumber"" END) AS ""ArchitecturalDrawing"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Functional Description Document' THEN ""DocumentNumber"" END) AS ""FunctionalDescriptionDocument"",
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'Product Procurement Number' THEN ""DocumentNumber"" END) AS ""ProductProcurementNumber""
                	FROM 
                		""View_AllDocuments""
                	RIGHT JOIN ""Device"" ON ""Device"".""Id""=""View_AllDocuments"".""DeviceId""
                	WHERE
                		""Device"".""Id"" = ""_DeviceId"";
                END;
                $BODY$;
                ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
