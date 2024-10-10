using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeviceColumnsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_DeviceModel_DeviceModelId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_FailState_FailStateId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_NatureOfSignal_NatureOfSignalId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ProcessLevel_ProcessLevelId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceBank_ServiceBankId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceTrain_ServiceTrainId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceZone_ServiceZoneId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_SubSystem_SubSystemId",
                table: "Device");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "SubSystemId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "StandTagId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SkidTagId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceZoneId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceTrainId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceBankId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProcessLevelId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "PanelTagId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "JunctionBoxTagId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FailStateId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "Device",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_DeviceModel_DeviceModelId",
                table: "Device",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_FailState_FailStateId",
                table: "Device",
                column: "FailStateId",
                principalTable: "FailState",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_NatureOfSignal_NatureOfSignalId",
                table: "Device",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ProcessLevel_ProcessLevelId",
                table: "Device",
                column: "ProcessLevelId",
                principalTable: "ProcessLevel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceBank_ServiceBankId",
                table: "Device",
                column: "ServiceBankId",
                principalTable: "ServiceBank",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceTrain_ServiceTrainId",
                table: "Device",
                column: "ServiceTrainId",
                principalTable: "ServiceTrain",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceZone_ServiceZoneId",
                table: "Device",
                column: "ServiceZoneId",
                principalTable: "ServiceZone",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_SubSystem_SubSystemId",
                table: "Device",
                column: "SubSystemId",
                principalTable: "SubSystem",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_DeviceModel_DeviceModelId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_FailState_FailStateId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_NatureOfSignal_NatureOfSignalId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ProcessLevel_ProcessLevelId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceBank_ServiceBankId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceTrain_ServiceTrainId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_ServiceZone_ServiceZoneId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Device_SubSystem_SubSystemId",
                table: "Device");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "SubSystemId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StandTagId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SkidTagId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceZoneId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceTrainId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceBankId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProcessLevelId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PanelTagId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NatureOfSignalId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JunctionBoxTagId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FailStateId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceModelId",
                table: "Device",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_DeviceModel_DeviceModelId",
                table: "Device",
                column: "DeviceModelId",
                principalTable: "DeviceModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_FailState_FailStateId",
                table: "Device",
                column: "FailStateId",
                principalTable: "FailState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_NatureOfSignal_NatureOfSignalId",
                table: "Device",
                column: "NatureOfSignalId",
                principalTable: "NatureOfSignal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ProcessLevel_ProcessLevelId",
                table: "Device",
                column: "ProcessLevelId",
                principalTable: "ProcessLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceBank_ServiceBankId",
                table: "Device",
                column: "ServiceBankId",
                principalTable: "ServiceBank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceTrain_ServiceTrainId",
                table: "Device",
                column: "ServiceTrainId",
                principalTable: "ServiceTrain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_ServiceZone_ServiceZoneId",
                table: "Device",
                column: "ServiceZoneId",
                principalTable: "ServiceZone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_SubSystem_SubSystemId",
                table: "Device",
                column: "SubSystemId",
                principalTable: "SubSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_JunctionBoxTagId",
                table: "Device",
                column: "JunctionBoxTagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_PanelTagId",
                table: "Device",
                column: "PanelTagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_SkidTagId",
                table: "Device",
                column: "SkidTagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Tag_StandTagId",
                table: "Device",
                column: "StandTagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
