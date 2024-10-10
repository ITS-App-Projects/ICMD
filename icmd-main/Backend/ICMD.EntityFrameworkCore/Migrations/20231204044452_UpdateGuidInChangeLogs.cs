using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuidInChangeLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CotextId",
                table: "ChangeLog",
                newName: "ContextId");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "EntityId",
            //    table: "ChangeLog",
            //    type: "uuid",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "integer");

            migrationBuilder.DropColumn(
               name: "EntityId",
               table: "ChangeLog");

            migrationBuilder.AddColumn<Guid>(
                           name: "EntityId",
                           table: "ChangeLog",
                           type: "uuid",
                           nullable: false,
                           defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContextId",
                table: "ChangeLog",
                newName: "CotextId");

            migrationBuilder.AlterColumn<int>(
                name: "EntityId",
                table: "ChangeLog",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
