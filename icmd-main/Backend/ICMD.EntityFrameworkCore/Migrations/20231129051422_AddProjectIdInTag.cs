using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdInTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ProjectId",
                table: "Tag",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Project_ProjectId",
                table: "Tag",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Project_ProjectId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_ProjectId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Tag");
        }
    }
}
