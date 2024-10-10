using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdINUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ICMDUser",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ICMDUser_ProjectId",
                table: "ICMDUser",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ICMDUser_Project_ProjectId",
                table: "ICMDUser",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ICMDUser_Project_ProjectId",
                table: "ICMDUser");

            migrationBuilder.DropIndex(
                name: "IX_ICMDUser_ProjectId",
                table: "ICMDUser");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ICMDUser");
        }
    }
}
