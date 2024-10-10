using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFKInPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Panel_ReferenceDocument_ReferenceDocumentId",
                table: "Panel");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceDocumentId",
                table: "Panel",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Panel_ReferenceDocument_ReferenceDocumentId",
                table: "Panel",
                column: "ReferenceDocumentId",
                principalTable: "ReferenceDocument",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Panel_ReferenceDocument_ReferenceDocumentId",
                table: "Panel");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceDocumentId",
                table: "Panel",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Panel_ReferenceDocument_ReferenceDocumentId",
                table: "Panel",
                column: "ReferenceDocumentId",
                principalTable: "ReferenceDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
