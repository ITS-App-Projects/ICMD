using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJunctionBoxTableFKInDeviceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Tag_JunctionBoxTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Tag_PanelTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Tag_SkidTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Tag_StandTagId",
                table: "Device");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_JunctionBox_JunctionBoxTagId",
                table: "Device",
                column: "JunctionBoxTagId",
                principalTable: "JunctionBox",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Panel_PanelTagId",
                table: "Device",
                column: "PanelTagId",
                principalTable: "Panel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Skid_SkidTagId",
                table: "Device",
                column: "SkidTagId",
                principalTable: "Skid",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Stand_StandTagId",
                table: "Device",
                column: "StandTagId",
                principalTable: "Stand",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_JunctionBox_JunctionBoxTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Panel_PanelTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Skid_SkidTagId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_Stand_StandTagId",
                table: "Device");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_JunctionBoxTagId",
                table: "Device",
                column: "JunctionBoxTagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_PanelTagId",
                table: "Device",
                column: "PanelTagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_SkidTagId",
                table: "Device",
                column: "SkidTagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_StandTagId",
                table: "Device",
                column: "StandTagId",
                principalTable: "Tag",
                principalColumn: "Id");
        }
    }
}
