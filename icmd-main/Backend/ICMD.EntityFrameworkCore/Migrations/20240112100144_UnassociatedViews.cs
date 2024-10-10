using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UnassociatedViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_UnassociatedTags""
 AS
 SELECT ""Tag"".""Id"",
    ""Tag"".""TagName"",
    ""Tag"".""ProcessId"",
    ""Tag"".""SubProcessId"",
    ""Tag"".""StreamId"",
    ""Tag"".""SequenceNumber"",
    ""Tag"".""EquipmentIdentifier"",
    ""Tag"".""EquipmentCodeId"",
    ""Tag"".""ModifiedDate"",
    ""Tag"".""ModifiedBy"",
    ""Tag"".""ProjectId"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version""
   FROM ""Tag""
     LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
     LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
	 where ""Tag"".""IsDeleted""=false
EXCEPT (
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""Device"" ON ""Device"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
			 where ""Tag"".""IsDeleted""=false
        UNION
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""Skid"" ON ""Skid"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
			 where ""Tag"".""IsDeleted""=false
        UNION
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""Stand"" ON ""Stand"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
			where ""Tag"".""IsDeleted""=false
        UNION
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""JunctionBox"" ON ""JunctionBox"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
			where ""Tag"".""IsDeleted""=false
        UNION
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""Cable"" ON ""Cable"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
 			where ""Tag"".""IsDeleted""=false
        UNION
         SELECT ""Tag"".""Id"",
            ""Tag"".""TagName"",
            ""Tag"".""ProcessId"",
            ""Tag"".""SubProcessId"",
            ""Tag"".""StreamId"",
            ""Tag"".""SequenceNumber"",
            ""Tag"".""EquipmentIdentifier"",
            ""Tag"".""EquipmentCodeId"",
            ""Tag"".""ModifiedDate"",
            ""Tag"".""ModifiedBy"",
            ""Tag"".""ProjectId"",
            doc.""DocumentNumber"",
            doc.""Revision"",
            doc.""Version""
           FROM ""Tag""
             JOIN ""Panel"" ON ""Panel"".""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""PnIdTag"" pid ON pid.""TagId"" = ""Tag"".""Id""
             LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = pid.""DocumentReferenceId""
			where ""Tag"".""IsDeleted""=false
);");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_UnassociatedSkids""
 AS
 SELECT ""Skid"".""Id"",
    ""Skid"".""TagId"",
    ""Skid"".""Type"",
    ""Skid"".""Description"",
    ""Skid"".""ModifiedDate"",
    ""Skid"".""ModifiedBy"",
    ""Skid"".""IsDeleted"",
    ""Skid"".""ReferenceDocumentId"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version"",
    ""Tag"".""TagName"",
    ""Tag"".""ProjectId""
   FROM ""Skid""
     LEFT JOIN ""Tag"" ON ""Tag"".""Id"" = ""Skid"".""TagId""
     LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = ""Skid"".""ReferenceDocumentId""
  WHERE NOT (""Skid"".""TagId"" IN ( SELECT ""Device"".""SkidTagId""
           FROM ""Device""
          WHERE ""Device"".""SkidTagId"" IS NOT NULL
          GROUP BY ""Device"".""SkidTagId"")) AND ""Skid"".""IsDeleted"" = false;");


            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_UnassociatedStands""
 AS
 SELECT ""Stand"".""Id"",
    ""Stand"".""TagId"",
    ""Stand"".""ReferenceDocumentId"",
    ""Stand"".""Type"",
    ""Stand"".""Area"",
    ""Stand"".""Description"",
    ""Stand"".""ModifiedDate"",
    ""Stand"".""ModifiedBy"",
    ""Stand"".""IsDeleted"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version"",
    ""Tag"".""TagName"",
    ""Tag"".""ProjectId""
   FROM ""Stand""
     LEFT JOIN ""Tag"" ON ""Tag"".""Id"" = ""Stand"".""TagId""
     LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = ""Stand"".""ReferenceDocumentId""
  WHERE NOT (""Stand"".""TagId"" IN ( SELECT ""Device"".""StandTagId""
           FROM ""Device""
          WHERE ""Device"".""StandTagId"" IS NOT NULL
          GROUP BY ""Device"".""StandTagId"")) AND ""Stand"".""IsDeleted"" = false;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_UnassociatedJunctionBoxes""
 AS
 SELECT ""JunctionBox"".""Id"",
    ""JunctionBox"".""TagId"",
    ""JunctionBox"".""Type"",
    ""JunctionBox"".""Description"",
    ""JunctionBox"".""ModifiedDate"",
    ""JunctionBox"".""ModifiedBy"",
    ""JunctionBox"".""IsDeleted"",
    ""JunctionBox"".""ReferenceDocumentId"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version"",
	""Tag"".""TagName"",
	""Tag"".""ProjectId""
   FROM ""JunctionBox""
     LEFT JOIN ""Tag"" ON ""Tag"".""Id"" = ""JunctionBox"".""TagId""
     LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = ""JunctionBox"".""ReferenceDocumentId""
  WHERE NOT (""JunctionBox"".""TagId"" IN ( SELECT ""Device"".""JunctionBoxTagId""
           FROM ""Device""
          WHERE ""Device"".""JunctionBoxTagId"" IS NOT NULL
          GROUP BY ""Device"".""JunctionBoxTagId"")) AND ""JunctionBox"".""IsDeleted"" = false;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_UnassociatedPanels""
 AS
 SELECT ""Panel"".""Id"",
    ""Panel"".""TagId"",
    ""Panel"".""Type"",
    ""Panel"".""Description"",
    ""Panel"".""ModifiedDate"",
    ""Panel"".""ModifiedBy"",
    ""Panel"".""IsDeleted"",
    ""Panel"".""ReferenceDocumentId"",
    doc.""DocumentNumber"",
    doc.""Revision"",
    doc.""Version"",
	""Tag"".""TagName"",
	""Tag"".""ProjectId""
   FROM ""Panel""
     LEFT JOIN ""Tag"" ON ""Tag"".""Id"" = ""Panel"".""TagId""
     LEFT JOIN ""ReferenceDocument"" doc ON doc.""Id"" = ""Panel"".""ReferenceDocumentId""
  WHERE NOT (""Panel"".""TagId"" IN ( SELECT ""Device"".""PanelTagId""
           FROM ""Device""
          WHERE ""Device"".""PanelTagId"" IS NOT NULL
          GROUP BY ""Device"".""PanelTagId"")) AND ""Panel"".""IsDeleted"" = false;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
