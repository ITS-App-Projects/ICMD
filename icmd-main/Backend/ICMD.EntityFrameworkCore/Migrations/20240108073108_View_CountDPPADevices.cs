using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class View_CountDPPADevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_CountDPPADevices""
             AS
             SELECT ""View_InstrumentListLive"".""ProjectId"",
                ""View_InstrumentListLive"".""PLC Number"" AS ""PLC_Number"",
                ""View_InstrumentListLive"".""PLC Slot Number"" AS ""PLC_Slot_Number"",
                    CASE
                        WHEN ""View_InstrumentListLive"".""DP/DP Coupler"" IS NOT NULL THEN ""View_InstrumentListLive"".""DP/DP Coupler""
                        ELSE ""View_InstrumentListLive"".""DP/PA Coupler""
                    END AS ""DP_or_PA_Coupler"",
                ""View_InstrumentListLive"".""AFD / Hub Number"" AS ""AFD___Hub_Number"",
                    CASE
                        WHEN ""View_InstrumentListLive"".""DP/DP Coupler"" IS NOT NULL THEN count(*)
                        ELSE 0::bigint
                    END AS ""No__of_DP_Devices"",
                    CASE
                        WHEN ""View_InstrumentListLive"".""DP/DP Coupler"" IS NULL THEN count(*)
                        ELSE 0::bigint
                    END AS ""No__of_PA_Devices""
               FROM ""View_InstrumentListLive""
              WHERE ""View_InstrumentListLive"".""PLC Number"" IS NOT NULL AND ""View_InstrumentListLive"".""PLC Slot Number"" IS NOT NULL AND ""View_InstrumentListLive"".""DP/DP Coupler"" IS NOT NULL AND ""View_InstrumentListLive"".""AFD / Hub Number"" IS NOT NULL OR ""View_InstrumentListLive"".""PLC Number"" IS NOT NULL AND ""View_InstrumentListLive"".""PLC Slot Number"" IS NOT NULL AND ""View_InstrumentListLive"".""AFD / Hub Number"" IS NOT NULL AND ""View_InstrumentListLive"".""DP/PA Coupler"" IS NOT NULL
              GROUP BY ""View_InstrumentListLive"".""ProjectId"", ""View_InstrumentListLive"".""PLC Number"", ""View_InstrumentListLive"".""PLC Slot Number"", ""View_InstrumentListLive"".""DP/DP Coupler"", ""View_InstrumentListLive"".""DP/PA Coupler"", ""View_InstrumentListLive"".""AFD / Hub Number"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
