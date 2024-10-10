using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsInTagTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_EquipmentCode_EquipmentCodeId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Process_ProcessId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Stream_StreamId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_SubProcess_SubProcessId",
                table: "Tag");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubProcessId",
                table: "Tag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "StreamId",
                table: "Tag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProcessId",
                table: "Tag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentCodeId",
                table: "Tag",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Field1String",
                table: "Tag",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field2String",
                table: "Tag",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field3String",
                table: "Tag",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field4String",
                table: "Tag",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TagDescriptorId",
                table: "Tag",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TagTypeId",
                table: "Tag",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_TagDescriptorId",
                table: "Tag",
                column: "TagDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_TagTypeId",
                table: "Tag",
                column: "TagTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_EquipmentCode_EquipmentCodeId",
                table: "Tag",
                column: "EquipmentCodeId",
                principalTable: "EquipmentCode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Process_ProcessId",
                table: "Tag",
                column: "ProcessId",
                principalTable: "Process",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Stream_StreamId",
                table: "Tag",
                column: "StreamId",
                principalTable: "Stream",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_SubProcess_SubProcessId",
                table: "Tag",
                column: "SubProcessId",
                principalTable: "SubProcess",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_TagDescriptor_TagDescriptorId",
                table: "Tag",
                column: "TagDescriptorId",
                principalTable: "TagDescriptor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_TagType_TagTypeId",
                table: "Tag",
                column: "TagTypeId",
                principalTable: "TagType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_EquipmentCode_EquipmentCodeId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Process_ProcessId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Stream_StreamId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_SubProcess_SubProcessId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_TagDescriptor_TagDescriptorId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_TagType_TagTypeId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_TagDescriptorId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_TagTypeId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Field1String",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Field2String",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Field3String",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "Field4String",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TagDescriptorId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TagTypeId",
                table: "Tag");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubProcessId",
                table: "Tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StreamId",
                table: "Tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProcessId",
                table: "Tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentCodeId",
                table: "Tag",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_EquipmentCode_EquipmentCodeId",
                table: "Tag",
                column: "EquipmentCodeId",
                principalTable: "EquipmentCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Process_ProcessId",
                table: "Tag",
                column: "ProcessId",
                principalTable: "Process",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Stream_StreamId",
                table: "Tag",
                column: "StreamId",
                principalTable: "Stream",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_SubProcess_SubProcessId",
                table: "Tag",
                column: "SubProcessId",
                principalTable: "SubProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
