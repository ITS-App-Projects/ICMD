using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqKeyInMasterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkAreaPack_Number",
                table: "WorkAreaPack");

            migrationBuilder.DropIndex(
                name: "IX_Tag_TagName",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_System_Number",
                table: "System");

            migrationBuilder.DropIndex(
                name: "IX_Stream_StreamName",
                table: "Stream");

            migrationBuilder.DropIndex(
                name: "IX_ServiceZone_Zone",
                table: "ServiceZone");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBank_Bank",
                table: "ServiceBank");

            migrationBuilder.DropIndex(
                name: "IX_ReferenceDocumentType_Type",
                table: "ReferenceDocumentType");

            migrationBuilder.DropIndex(
                name: "IX_Process_ProcessName",
                table: "Process");

            migrationBuilder.DropIndex(
                name: "IX_NatureOfSignal_NatureOfSignalName",
                table: "NatureOfSignal");

            migrationBuilder.DropIndex(
                name: "IX_Manufacturer_Name",
                table: "Manufacturer");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentCode_Code",
                table: "EquipmentCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkAreaPack_Number",
                table: "WorkAreaPack",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_TagName",
                table: "Tag",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_System_Number",
                table: "System",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stream_StreamName",
                table: "Stream",
                column: "StreamName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_Zone",
                table: "ServiceZone",
                column: "Zone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBank_Bank",
                table: "ServiceBank",
                column: "Bank",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocumentType_Type",
                table: "ReferenceDocumentType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Process_ProcessName",
                table: "Process",
                column: "ProcessName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NatureOfSignal_NatureOfSignalName",
                table: "NatureOfSignal",
                column: "NatureOfSignalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturer_Name",
                table: "Manufacturer",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCode_Code",
                table: "EquipmentCode",
                column: "Code",
                unique: true);
        }
    }
}
