using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFunction_fnGetDeviceDocumentsInColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fnGetDeviceDocumentsInColumns""(
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
                		public.""Aggregate_Concat"" (CASE ""Type"" WHEN 'P&ID' THEN ""DocumentNumber"" END) AS ""PIDNumber"",
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
                
$BODY$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
