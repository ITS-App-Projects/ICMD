using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class View_PnID_Device_DocumentReferenceCompare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_PnID_Device_DocumentReferenceCompare""
 AS
 SELECT t.""ProjectId"",
    d.""Id"" AS ""DeviceId"",
    t.""TagName"" as ""Tag"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version"",
    pniddoc.""DocumentNumber"" AS ""PnIdDocumentNumber"",
    pniddoc.""Revision"" AS ""PnIdRevision"",
    pniddoc.""Version"" AS ""PnIdVersion""
   FROM ""Device"" d
     JOIN ""ReferenceDocumentDevice"" map ON map.""DeviceId"" = d.""Id""
     JOIN ""ReferenceDocument"" doc ON doc.""Id"" = map.""ReferenceDocumentId""
     JOIN ""ReferenceDocumentType"" doctype ON doctype.""Id"" = doc.""ReferenceDocumentTypeId"" AND doctype.""Type""::text = 'P&ID'::text
     JOIN ""Tag"" t ON t.""Id"" = d.""TagId""
     JOIN ""PnIdTag"" pid ON pid.""TagId"" = t.""Id""
     JOIN ""ReferenceDocument"" pniddoc ON pniddoc.""Id"" = pid.""DocumentReferenceId""
  WHERE d.""IsDeleted"" = false;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
