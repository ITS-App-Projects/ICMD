using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ViewTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW ""View_Tag"" AS
            SELECT
                ""Tag"".""Id"",
                ""Tag"".""TagName"",
                ""Process"".""ProcessName"",
                ""SubProcess"".""SubProcessName"",
                ""Stream"".""StreamName"",
                ""EquipmentCode"".""Code"" AS ""EquipmentCode"",
                ""Tag"".""SequenceNumber"",
                ""Tag"".""EquipmentIdentifier"",
                ""Tag"".""ProjectId""
            FROM ""Tag""
            LEFT JOIN ""Process"" ON ""Process"".""Id"" = ""Tag"".""ProcessId""
            LEFT JOIN ""SubProcess"" ON ""SubProcess"".""Id"" = ""Tag"".""SubProcessId""
            LEFT JOIN ""Stream"" ON ""Stream"".""Id"" = ""Tag"".""StreamId""
            LEFT JOIN ""EquipmentCode"" ON ""EquipmentCode"".""Id"" = ""Tag"".""EquipmentCodeId"";
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
