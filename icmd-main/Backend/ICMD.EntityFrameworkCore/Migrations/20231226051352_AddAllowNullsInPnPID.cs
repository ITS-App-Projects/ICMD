using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowNullsInPnPID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PnIdTag_FailState_FailStateId",
                table: "PnIdTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PnIdTag_Skid_SkidId",
                table: "PnIdTag");

            migrationBuilder.AlterColumn<Guid>(
                name: "SkidId",
                table: "PnIdTag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FailStateId",
                table: "PnIdTag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PnIdTag_FailState_FailStateId",
                table: "PnIdTag",
                column: "FailStateId",
                principalTable: "FailState",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PnIdTag_Skid_SkidId",
                table: "PnIdTag",
                column: "SkidId",
                principalTable: "Skid",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PnIdTag_FailState_FailStateId",
                table: "PnIdTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PnIdTag_Skid_SkidId",
                table: "PnIdTag");

            migrationBuilder.AlterColumn<Guid>(
                name: "SkidId",
                table: "PnIdTag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FailStateId",
                table: "PnIdTag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PnIdTag_FailState_FailStateId",
                table: "PnIdTag",
                column: "FailStateId",
                principalTable: "FailState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PnIdTag_Skid_SkidId",
                table: "PnIdTag",
                column: "SkidId",
                principalTable: "Skid",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
