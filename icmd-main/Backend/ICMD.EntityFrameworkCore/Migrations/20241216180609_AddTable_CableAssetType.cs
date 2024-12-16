using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_CableAssetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CableAssetType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CategoryDescription = table.Column<string>(type: "text", nullable: false),
                    CableType = table.Column<string>(type: "text", nullable: false),
                    CableTypeDescription = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_CableAssetType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CableAssetType_Type",
                table: "CableAssetType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CableAssetType_ProjectId",
                table: "CableAssetType",
                column: "ProjectId");

            var guid = Guid.NewGuid();

            migrationBuilder.Sql(@"INSERT INTO ""MenuItems"" (""Id"", ""MenuName"", ""ControllerName"", ""MenuDescription"", ""Url"", ""Icon"", ""SortOrder"", ""ParentMenuId"", ""IsPermission"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + guid + "', 'Cable Asset Type', 'CableAssetType', 'Cable Asset Type', 'manage-cableassettype', '', '17', '73f42fc6-d697-4c6e-8c48-8c9c67749f9f', 'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');");

            var permissionId1 = Guid.NewGuid();
            var permissionId2 = Guid.NewGuid();
            var permissionId3 = Guid.NewGuid();

            migrationBuilder.Sql(@"INSERT INTO ""MenuPermission"" (""Id"", ""MenuId"", ""RoleId"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + permissionId1 + "', '" + guid + "','ba049ac6-32aa-4f2a-b0b0-7feb3e3b7357','TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""MenuPermission"" (""Id"", ""MenuId"", ""RoleId"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + permissionId2 + "', '" + guid + "','f0c5fd8c-ef7b-41a6-a595-8f71773898f3','TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""MenuPermission"" (""Id"", ""MenuId"", ""RoleId"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + permissionId3 + "', '" + guid + "','839f9cc8-c8c1-481b-9d19-6a173f881f54','TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');");

            migrationBuilder.Sql(@"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId1 + "',1,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId1 + "',2,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId1 + "',3,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId1 + "',4,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId1 + "',5,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId2 + "',5,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');" +
                @"INSERT INTO ""PermissionManagement"" (""Id"", ""MenuPermissionId"", ""Operation"", ""IsGranted"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"")" +
                "VALUES ('" + Guid.NewGuid() + "', '" + permissionId3 + "',5,'TRUE', '00000000-0000-0000-0000-000000000004', 'NOW()', 'TRUE', 'FALSE');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CableAssetType");
        }
    }
}
