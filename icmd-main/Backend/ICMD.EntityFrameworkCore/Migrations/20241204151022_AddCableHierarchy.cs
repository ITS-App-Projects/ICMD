using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddCableHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CableHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Instrument = table.Column<bool>(type: "boolean", nullable: false),
                    OriginDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CableHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CableHierarchy_Device_DestinationDeviceId",
                        column: x => x.DestinationDeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CableHierarchy_Device_OriginDeviceId",
                        column: x => x.OriginDeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CableHierarchy_DestinationDeviceId",
                table: "CableHierarchy",
                column: "DestinationDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CableHierarchy_OriginDeviceId",
                table: "CableHierarchy",
                column: "OriginDeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CableHierarchy");
        }
    }
}
