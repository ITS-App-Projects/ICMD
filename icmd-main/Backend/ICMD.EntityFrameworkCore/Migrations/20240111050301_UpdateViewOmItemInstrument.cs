using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateViewOmItemInstrument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"-- View: public.View_OMItem_InstrumentList

-- DROP VIEW public.""View_OMItem_InstrumentList"";

CREATE OR REPLACE VIEW public.""View_OMItem_InstrumentList""
 AS
 SELECT ""OMItem"".""Id"",
    ""OMItem"".""ItemId"",
    ""OMItem"".""ItemDescription"",
    ""OMItem"".""ParentItemId"",
    ""OMItem"".""AssetTypeId"",
    ""View_InstrumentListLive"".""DeviceId"",
    ""View_InstrumentListLive"".""Process No"",
    ""View_InstrumentListLive"".""Sub Process"",
    ""View_InstrumentListLive"".""StreamName"",
    ""View_InstrumentListLive"".""Equipment Code"",
    ""View_InstrumentListLive"".""Sequence Number"",
    ""View_InstrumentListLive"".""Equipment Identifier"",
    ""View_InstrumentListLive"".""TagName"" AS ""Tag"",
    ""View_InstrumentListLive"".""Instr Parent Tag"",
    ""View_InstrumentListLive"".""Service Description"",
    ""View_InstrumentListLive"".""Line / Vessel Number"",
    ""View_InstrumentListLive"".""Plant"",
    ""View_InstrumentListLive"".""Area"",
    ""View_InstrumentListLive"".""Vendor Supply"",
    ""View_InstrumentListLive"".""Skid Number"",
    ""View_InstrumentListLive"".""Stand Number"",
    ""View_InstrumentListLive"".""Manufacturer"",
    ""View_InstrumentListLive"".""Model Number"",
    ""View_InstrumentListLive"".""Calibrated Range (Min)"",
    ""View_InstrumentListLive"".""Calibrated Range (Max)"",
    ""View_InstrumentListLive"".""CR Units"",
    ""View_InstrumentListLive"".""Process Range (Min)"",
    ""View_InstrumentListLive"".""Process Range (Max)"",
    ""View_InstrumentListLive"".""PR Units"",
    ""View_InstrumentListLive"".""RL / Position"",
    ""View_InstrumentListLive"".""Datasheet Number"",
    ""View_InstrumentListLive"".""Sheet Number"",
    ""View_InstrumentListLive"".""Hook-up Drawing"",
    ""View_InstrumentListLive"".""Termination Diagram"",
    ""View_InstrumentListLive"".""P&Id Number"",
    ""View_InstrumentListLive"".""Layout Drawing"",
    ""View_InstrumentListLive"".""Architectural Drawing"",
    ""View_InstrumentListLive"".""Functional Description Document"",
    ""View_InstrumentListLive"".""Product Procurement Number"",
    ""View_InstrumentListLive"".""Junction Box Number"",
    ""View_InstrumentListLive"".""Nature Of Signal"",
    ""View_InstrumentListLive"".""Fail State"",
    ""View_InstrumentListLive"".""GSD Type"",
    ""View_InstrumentListLive"".""Control Panel Number"",
    ""View_InstrumentListLive"".""PLC Number"",
    ""View_InstrumentListLive"".""PLC Slot Number"",
    ""View_InstrumentListLive"".""Field Panel Number"",
    ""View_InstrumentListLive"".""DP/DP Coupler"",
    ""View_InstrumentListLive"".""DP/PA Coupler"",
    ""View_InstrumentListLive"".""AFD / Hub Number"",
    ""View_InstrumentListLive"".""Rack No"",
    ""View_InstrumentListLive"".""Slot No"",
    ""View_InstrumentListLive"".""Channel No"",
    ""View_InstrumentListLive"".""DP Node Address"",
    ""View_InstrumentListLive"".""PA Node Address"",
    ""View_InstrumentListLive"".""Revision"",
    ""View_InstrumentListLive"".""Revision Changes / Outstanding Comments"",
    ""View_InstrumentListLive"".""Zone"",
    ""View_InstrumentListLive"".""Bank"",
    ""View_InstrumentListLive"".""Service"",
    ""View_InstrumentListLive"".""Variable"",
    ""View_InstrumentListLive"".""Train"",
    ""View_InstrumentListLive"".""Work Area Pack"",
    ""View_InstrumentListLive"".""System Code"",
    ""View_InstrumentListLive"".""SubSystem Code"",
    ""View_InstrumentListLive"".""Historical Logging"",
    ""View_InstrumentListLive"".""Historical Logging Frequency"",
    ""View_InstrumentListLive"".""Historical Logging Resolution"",
    ""View_InstrumentListLive"".""IsInstrument"",

    CASE
            WHEN ""View_InstrumentListLive"".""IsActive"" IS NULL THEN ""OMItem"".""IsActive""
            ELSE ""View_InstrumentListLive"".""IsActive""
      END AS ""IsActive"",
	 CASE
            WHEN ""View_InstrumentListLive"".""IsDeleted"" IS NULL THEN ""OMItem"".""IsDeleted""
            ELSE ""View_InstrumentListLive"".""IsDeleted""
      END AS ""IsDeleted"",
        CASE
            WHEN ""View_InstrumentListLive"".""ProjectId"" IS NULL THEN ""OMItem"".""ProjectId""
            ELSE ""View_InstrumentListLive"".""ProjectId""
        END AS ""ProjectId""
   FROM ""View_InstrumentListLive""
     FULL JOIN ""OMItem"" ON ""OMItem"".""ItemId""::text = ""View_InstrumentListLive"".""TagName""::text;

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
