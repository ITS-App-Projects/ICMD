using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFKFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_DeviceModel_DeviceModelId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_DeviceType_DeviceTypeId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_NatureOfSignal_NatureOfSignalId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_DeviceModel_DeviceModelId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_DeviceType_DeviceTypeId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_Device_DeviceId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_NatureOfSignal_NatureOfSignalId",
                table: "AttributeValue");

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "AttributeValue",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceTypeId",
                table: "AttributeValue",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "AttributeValue",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "AttributeValue",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceTypeId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_DeviceModel_DeviceModelId",
                table: "AttributeDefinition",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_DeviceType_DeviceTypeId",
                table: "AttributeDefinition",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_NatureOfSignal_NatureOfSignalId",
                table: "AttributeDefinition",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_DeviceModel_DeviceModelId",
                table: "AttributeValue",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_DeviceType_DeviceTypeId",
                table: "AttributeValue",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_Device_DeviceId",
                table: "AttributeValue",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_NatureOfSignal_NatureOfSignalId",
                table: "AttributeValue",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_DeviceModel_DeviceModelId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_DeviceType_DeviceTypeId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_NatureOfSignal_NatureOfSignalId",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_DeviceModel_DeviceModelId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_DeviceType_DeviceTypeId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_Device_DeviceId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_NatureOfSignal_NatureOfSignalId",
                table: "AttributeValue");

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "AttributeValue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceTypeId",
                table: "AttributeValue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "AttributeValue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "AttributeValue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceTypeId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "AttributeDefinition",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_DeviceModel_DeviceModelId",
                table: "AttributeDefinition",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_DeviceType_DeviceTypeId",
                table: "AttributeDefinition",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinition_NatureOfSignal_NatureOfSignalId",
                table: "AttributeDefinition",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_DeviceModel_DeviceModelId",
                table: "AttributeValue",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_DeviceType_DeviceTypeId",
                table: "AttributeValue",
                column: "DeviceTypeId",
                principalTable: "DeviceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_Device_DeviceId",
                table: "AttributeValue",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_NatureOfSignal_NatureOfSignalId",
                table: "AttributeValue",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
