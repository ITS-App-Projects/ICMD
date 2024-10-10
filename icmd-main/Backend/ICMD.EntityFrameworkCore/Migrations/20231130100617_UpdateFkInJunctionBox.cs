﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFkInJunctionBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JunctionBox_ReferenceDocument_ReferenceDocumentId",
                table: "JunctionBox");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceDocumentId",
                table: "JunctionBox",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_JunctionBox_ReferenceDocument_ReferenceDocumentId",
                table: "JunctionBox",
                column: "ReferenceDocumentId",
                principalTable: "ReferenceDocument",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JunctionBox_ReferenceDocument_ReferenceDocumentId",
                table: "JunctionBox");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReferenceDocumentId",
                table: "JunctionBox",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JunctionBox_ReferenceDocument_ReferenceDocumentId",
                table: "JunctionBox",
                column: "ReferenceDocumentId",
                principalTable: "ReferenceDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}