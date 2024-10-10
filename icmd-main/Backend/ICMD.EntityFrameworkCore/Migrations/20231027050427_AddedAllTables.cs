using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddedAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Context = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CotextId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OriginalValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    NewValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_ChangeLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceClassification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Classification = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_DeviceClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_DeviceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descriptor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_EquipmentCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FailState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FailStateName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_FailState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GSDType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GSDTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_GSDType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentListImport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessNo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(50)", maxLength: 30, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Tag = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    ServiceDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ModelNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CalibratedRangeMin = table.Column<float>(type: "real", nullable: true),
                    CalibratedRangeMax = table.Column<float>(type: "real", nullable: true),
                    ProcessRangeMin = table.Column<float>(type: "real", nullable: true),
                    ProcessRangeMax = table.Column<float>(type: "real", nullable: true),
                    DataSheetNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SheetNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HookupDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TerminationDiagram = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PIDNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NatureOfSignal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    GSDType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ControlPanelNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    PLCNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    PLCSlotNumber = table.Column<int>(type: "integer", nullable: true),
                    FieldPanelNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    DPDPCoupler = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    DPPACoupler = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    AFDHubNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    RackNo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    SlotNo = table.Column<int>(type: "integer", nullable: true),
                    SlotNoExt = table.Column<int>(type: "integer", nullable: true),
                    ChannelNo = table.Column<int>(type: "integer", nullable: true),
                    ChannelNoExt = table.Column<int>(type: "integer", nullable: true),
                    DPNodeAddress = table.Column<int>(type: "integer", nullable: true),
                    PANodeAddress = table.Column<int>(type: "integer", nullable: true),
                    Revision = table.Column<int>(type: "integer", nullable: false),
                    RevisionChanges = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Service = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Variable = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Train = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Units = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Bank = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstparentTag = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    Plant = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubPlantArea = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VendorSupply = table.Column<bool>(type: "boolean", nullable: true),
                    SkidNo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    RLPosition = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LayoutDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ArchitectureDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    JunctionBox = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    FailState = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStand = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    WorkPackage = table.Column<int>(type: "integer", nullable: true),
                    SystemNo = table.Column<int>(type: "integer", nullable: true),
                    SubSystemNo = table.Column<int>(type: "integer", nullable: true),
                    LineVesselNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FunctionalDescDoc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcurementPkgNum = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_InstrumentListImport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", nullable: true),
                    Comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NatureOfSignal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NatureOfSignalName = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
                    table.PrimaryKey("PK_NatureOfSignal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonInstrumentListImport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessNo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DeviceType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NatureOfSignal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DPNodeAddress = table.Column<int>(type: "integer", nullable: true),
                    NoSlotsChannels = table.Column<int>(type: "integer", nullable: true),
                    ConnectionParent = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PLCNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PLCSlotNumber = table.Column<int>(type: "integer", nullable: true),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ModelNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ModelDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ArchitecturalDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ArchitecturalDrawingSheet = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Revision = table.Column<int>(type: "integer", nullable: true),
                    RevisionChanges = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_NonInstrumentListImport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OLMDPTDPR",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    No = table.Column<double>(type: "double precision", nullable: true),
                    OLMDPTDPRDeviceTag = table.Column<Guid>(type: "uuid", nullable: false),
                    PLCSlotNo = table.Column<double>(type: "double precision", nullable: true),
                    DeviceType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DevicePhysicalLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PLCNo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_OLMDPTDPR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OMItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    ItemDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ParentItemId = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    AssetTypeId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_OMItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OMServiceDescriptionImport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    ServiceDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Stream = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bank = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Service = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Variable = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Train = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_OMServiceDescriptionImport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OMServiceDescriptionImport_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Process",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Process", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Process_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessLevel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_ProcessLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ProjectUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUser_ICMDUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ICMDUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceDocumentType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_ReferenceDocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    URL = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bank = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ServiceBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceBank_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTrain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Train = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ServiceTrain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTrain_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceZone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Area = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ServiceZone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceZone_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignalExtension",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Extension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CBVariableType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PCS7VariableType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Kind = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
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
                    table.PrimaryKey("PK_SignalExtension", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISEquipmentList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PnPId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcessNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DWGTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PipingClass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OnSkid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Function = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TrackingNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SSISEquipmentList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISFittings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PnPId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcessNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DWGTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PipingClass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OnSkid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_SSISFittings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISInstruments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PnPId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcessNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OnEquipment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OnSkid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FluidCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PipeLinesTag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Size = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DWGTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    To = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    From = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TrackingNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SSISInstruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISInstrumentsVsStands110728",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DF = table.Column<double>(type: "double precision", nullable: true),
                    PLCNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcessNo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Function = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DatasheetNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SheetNumber = table.Column<double>(type: "double precision", nullable: true),
                    GeneralArrangement = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TerminationDiagram = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PIDNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HubNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VendorSkid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StandReqd = table.Column<double>(type: "double precision", nullable: true),
                    HookupDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandTAG = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AncillaryPlate = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OldHOOKUP_172011 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OldStandTAG_172011 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OldStandTYPE_172011 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PDMSSTANDQUERYSTANDPRESENT = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PDMSALLINSTRU = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Working = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Working2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    F32 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_SSISInstrumentsVsStands110728", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISStandList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Item = table.Column<double>(type: "double precision", nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandTag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Qrea = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    QTY = table.Column<double>(type: "double precision", nullable: true),
                    DrawingReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Figure = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AFD1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AFD2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AFD3 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AFDPlates = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DPH1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DPH2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DPHPlates = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instrument1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instrument2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instrument3 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instrument4 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ReasonsForChange = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ChangeBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    F23 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    F24 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    F25 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    F26 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_SSISStandList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISStandTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Item = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstrumentStandType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VendorReferenceDrawing = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    QTY = table.Column<double>(type: "double precision", nullable: true),
                    ProjectDrawingReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_SSISStandTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SSISValveList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PnPId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProcessNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SubProcess = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Stream = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SequenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DWGTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Rev = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Size = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FluidCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PipeLinesTag = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PipingClass = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ClassName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OnSkid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Failure = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Switches = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    From = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    To = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Accessories = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DesignTemp = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NominalPressure = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ValveSpecNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PNRating = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TrackingNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SSISValveList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stream",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Stream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stream_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubProcess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubProcessName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SubProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProcess_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UIChangeLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Tag = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    PLCNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Changes = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_UIChangeLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkAreaPack",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_WorkAreaPack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkAreaPack_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_DeviceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceModel_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildProcessLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentProcessLevelId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ProcessHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessHierarchy_ProcessLevel_ChildProcessLevelId",
                        column: x => x.ChildProcessLevelId,
                        principalTable: "ProcessLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessHierarchy_ProcessLevel_ParentProcessLevelId",
                        column: x => x.ParentProcessLevelId,
                        principalTable: "ProcessLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    URL = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Revision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sheet = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsVDPDocumentNumber = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceDocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ReferenceDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceDocument_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferenceDocument_ReferenceDocumentType_ReferenceDocumentTy~",
                        column: x => x.ReferenceDocumentTypeId,
                        principalTable: "ReferenceDocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GSDType_SignalExtension",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GSDTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignalExtensionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_GSDType_SignalExtension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GSDType_SignalExtension_GSDType_GSDTypeId",
                        column: x => x.GSDTypeId,
                        principalTable: "GSDType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GSDType_SignalExtension_SignalExtension_SignalExtensionId",
                        column: x => x.SignalExtensionId,
                        principalTable: "SignalExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NatureOfSignalSignalExtension",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NatureOfSignalId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignalExtensionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_NatureOfSignalSignalExtension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NatureOfSignalSignalExtension_NatureOfSignal_NatureOfSignal~",
                        column: x => x.NatureOfSignalId,
                        principalTable: "NatureOfSignal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NatureOfSignalSignalExtension_SignalExtension_SignalExtensi~",
                        column: x => x.SignalExtensionId,
                        principalTable: "SignalExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagName = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    SequenceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EquipmentIdentifier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ProcessId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubProcessId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentCodeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_EquipmentCode_EquipmentCodeId",
                        column: x => x.EquipmentCodeId,
                        principalTable: "EquipmentCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tag_Process_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Process",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tag_Stream_StreamId",
                        column: x => x.StreamId,
                        principalTable: "Stream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tag_SubProcess_SubProcessId",
                        column: x => x.SubProcessId,
                        principalTable: "SubProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    WorkAreaPackId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_System", x => x.Id);
                    table.ForeignKey(
                        name: "FK_System_WorkAreaPack_WorkAreaPackId",
                        column: x => x.WorkAreaPackId,
                        principalTable: "WorkAreaPack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ValueType = table.Column<string>(type: "character varying(20)", maxLength: 255, nullable: true),
                    Inherit = table.Column<bool>(type: "boolean", nullable: false),
                    Private = table.Column<bool>(type: "boolean", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NatureOfSignalId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_AttributeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDefinition_DeviceModel_DeviceModelId",
                        column: x => x.DeviceModelId,
                        principalTable: "DeviceModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeDefinition_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeDefinition_NatureOfSignal_NatureOfSignalId",
                        column: x => x.NatureOfSignalId,
                        principalTable: "NatureOfSignal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OriginDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DestDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Length = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CableRoute = table.Column<string>(type: "character varying(255)", maxLength: 50, nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestTagId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Cable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cable_Tag_DestTagId",
                        column: x => x.DestTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cable_Tag_OriginTagId",
                        column: x => x.OriginTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cable_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JunctionBox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_JunctionBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JunctionBox_ReferenceDocument_ReferenceDocumentId",
                        column: x => x.ReferenceDocumentId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JunctionBox_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Panel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Panel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Panel_ReferenceDocument_ReferenceDocumentId",
                        column: x => x.ReferenceDocumentId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Panel_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skid",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Skid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skid_ReferenceDocument_ReferenceDocumentId",
                        column: x => x.ReferenceDocumentId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Skid_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Area = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ReferenceDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Stand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stand_ReferenceDocument_ReferenceDocumentId",
                        column: x => x.ReferenceDocumentId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stand_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubSystem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_SubSystem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubSystem_System_SystemId",
                        column: x => x.SystemId,
                        principalTable: "System",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PnIdTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Switches = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PipelineTag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PnPId = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkidId = table.Column<Guid>(type: "uuid", nullable: false),
                    FailStateId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_PnIdTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PnIdTag_FailState_FailStateId",
                        column: x => x.FailStateId,
                        principalTable: "FailState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PnIdTag_ReferenceDocument_DocumentReferenceId",
                        column: x => x.DocumentReferenceId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PnIdTag_Skid_SkidId",
                        column: x => x.SkidId,
                        principalTable: "Skid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PnIdTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceDescription = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LineVesselNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    VendorSupply = table.Column<bool>(type: "boolean", nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HistoricalLogging = table.Column<bool>(type: "boolean", nullable: true),
                    HistoricalLoggingFrequency = table.Column<double>(type: "double precision", nullable: true),
                    HistoricalLoggingResolution = table.Column<double>(type: "double precision", nullable: true),
                    Revision = table.Column<int>(type: "integer", nullable: false),
                    RevisionChanges = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Service = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Variable = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsInstrument = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTrainId = table.Column<Guid>(type: "uuid", nullable: false),
                    NatureOfSignalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PanelTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkidTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    StandTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    JunctionBoxTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    FailStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubSystemId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_DeviceModel_DeviceModelId",
                        column: x => x.DeviceModelId,
                        principalTable: "DeviceModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_FailState_FailStateId",
                        column: x => x.FailStateId,
                        principalTable: "FailState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_NatureOfSignal_NatureOfSignalId",
                        column: x => x.NatureOfSignalId,
                        principalTable: "NatureOfSignal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_ProcessLevel_ProcessLevelId",
                        column: x => x.ProcessLevelId,
                        principalTable: "ProcessLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_ServiceBank_ServiceBankId",
                        column: x => x.ServiceBankId,
                        principalTable: "ServiceBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_ServiceTrain_ServiceTrainId",
                        column: x => x.ServiceTrainId,
                        principalTable: "ServiceTrain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_ServiceZone_ServiceZoneId",
                        column: x => x.ServiceZoneId,
                        principalTable: "ServiceZone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_SubSystem_SubSystemId",
                        column: x => x.SubSystemId,
                        principalTable: "SubSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Tag_JunctionBoxTagId",
                        column: x => x.JunctionBoxTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Tag_PanelTagId",
                        column: x => x.PanelTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Tag_SkidTagId",
                        column: x => x.SkidTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Tag_StandTagId",
                        column: x => x.StandTagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Revision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Requirement = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ItemNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IncludeInDatasheet = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    NatureOfSignalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_AttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeValue_AttributeDefinition_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeValue_DeviceModel_DeviceModelId",
                        column: x => x.DeviceModelId,
                        principalTable: "DeviceModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeValue_DeviceType_DeviceTypeId",
                        column: x => x.DeviceTypeId,
                        principalTable: "DeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeValue_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeValue_NatureOfSignal_NatureOfSignalId",
                        column: x => x.NatureOfSignalId,
                        principalTable: "NatureOfSignal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlSystemHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Instrument = table.Column<bool>(type: "boolean", nullable: false),
                    ParentDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ControlSystemHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlSystemHierarchy_Device_ChildDeviceId",
                        column: x => x.ChildDeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControlSystemHierarchy_Device_ParentDeviceId",
                        column: x => x.ParentDeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceDocumentDevice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ReferenceDocumentDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceDocumentDevice_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferenceDocumentDevice_ReferenceDocument_ReferenceDocument~",
                        column: x => x.ReferenceDocumentId,
                        principalTable: "ReferenceDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceAttributeValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_DeviceAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceAttributeValue_AttributeValue_AttributeValueId",
                        column: x => x.AttributeValueId,
                        principalTable: "AttributeValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceAttributeValue_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinition_DeviceModelId",
                table: "AttributeDefinition",
                column: "DeviceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinition_DeviceTypeId",
                table: "AttributeDefinition",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinition_NatureOfSignalId",
                table: "AttributeDefinition",
                column: "NatureOfSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_AttributeDefinitionId",
                table: "AttributeValue",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_DeviceId",
                table: "AttributeValue",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_DeviceModelId",
                table: "AttributeValue",
                column: "DeviceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_DeviceTypeId",
                table: "AttributeValue",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_NatureOfSignalId",
                table: "AttributeValue",
                column: "NatureOfSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_Cable_DestTagId",
                table: "Cable",
                column: "DestTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Cable_OriginTagId",
                table: "Cable",
                column: "OriginTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Cable_TagId",
                table: "Cable",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlSystemHierarchy_ChildDeviceId",
                table: "ControlSystemHierarchy",
                column: "ChildDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlSystemHierarchy_ParentDeviceId",
                table: "ControlSystemHierarchy",
                column: "ParentDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceModelId",
                table: "Device",
                column: "DeviceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceTypeId",
                table: "Device",
                column: "DeviceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_FailStateId",
                table: "Device",
                column: "FailStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_JunctionBoxTagId",
                table: "Device",
                column: "JunctionBoxTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_NatureOfSignalId",
                table: "Device",
                column: "NatureOfSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_PanelTagId",
                table: "Device",
                column: "PanelTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ProcessLevelId",
                table: "Device",
                column: "ProcessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ServiceBankId",
                table: "Device",
                column: "ServiceBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ServiceTrainId",
                table: "Device",
                column: "ServiceTrainId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ServiceZoneId",
                table: "Device",
                column: "ServiceZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_SkidTagId",
                table: "Device",
                column: "SkidTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_StandTagId",
                table: "Device",
                column: "StandTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_SubSystemId",
                table: "Device",
                column: "SubSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_TagId",
                table: "Device",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceAttributeValue_AttributeValueId",
                table: "DeviceAttributeValue",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceAttributeValue_DeviceId",
                table: "DeviceAttributeValue",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModel_ManufacturerId",
                table: "DeviceModel",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCode_Code",
                table: "EquipmentCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GSDType_GSDTypeName",
                table: "GSDType",
                column: "GSDTypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GSDType_SignalExtension_GSDTypeId",
                table: "GSDType_SignalExtension",
                column: "GSDTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GSDType_SignalExtension_SignalExtensionId",
                table: "GSDType_SignalExtension",
                column: "SignalExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_JunctionBox_ReferenceDocumentId",
                table: "JunctionBox",
                column: "ReferenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_JunctionBox_TagId",
                table: "JunctionBox",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturer_Name",
                table: "Manufacturer",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NatureOfSignal_NatureOfSignalName",
                table: "NatureOfSignal",
                column: "NatureOfSignalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NatureOfSignalSignalExtension_NatureOfSignalId",
                table: "NatureOfSignalSignalExtension",
                column: "NatureOfSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_NatureOfSignalSignalExtension_SignalExtensionId",
                table: "NatureOfSignalSignalExtension",
                column: "SignalExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_OMServiceDescriptionImport_ProjectId",
                table: "OMServiceDescriptionImport",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Panel_ReferenceDocumentId",
                table: "Panel",
                column: "ReferenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Panel_TagId",
                table: "Panel",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PnIdTag_DocumentReferenceId",
                table: "PnIdTag",
                column: "DocumentReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_PnIdTag_FailStateId",
                table: "PnIdTag",
                column: "FailStateId");

            migrationBuilder.CreateIndex(
                name: "IX_PnIdTag_SkidId",
                table: "PnIdTag",
                column: "SkidId");

            migrationBuilder.CreateIndex(
                name: "IX_PnIdTag_TagId",
                table: "PnIdTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Process_ProcessName",
                table: "Process",
                column: "ProcessName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Process_ProjectId",
                table: "Process",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessHierarchy_ChildProcessLevelId",
                table: "ProcessHierarchy",
                column: "ChildProcessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessHierarchy_ParentProcessLevelId",
                table: "ProcessHierarchy",
                column: "ParentProcessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessLevel_Name",
                table: "ProcessLevel",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_ProjectId",
                table: "ProjectUser",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_UserId",
                table: "ProjectUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocument_ProjectId",
                table: "ReferenceDocument",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocument_ReferenceDocumentTypeId",
                table: "ReferenceDocument",
                column: "ReferenceDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocumentDevice_DeviceId",
                table: "ReferenceDocumentDevice",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocumentDevice_ReferenceDocumentId",
                table: "ReferenceDocumentDevice",
                column: "ReferenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDocumentType_Type",
                table: "ReferenceDocumentType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBank_Bank",
                table: "ServiceBank",
                column: "Bank",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBank_ProjectId",
                table: "ServiceBank",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTrain_ProjectId",
                table: "ServiceTrain",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_ProjectId",
                table: "ServiceZone",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceZone_Zone",
                table: "ServiceZone",
                column: "Zone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skid_ReferenceDocumentId",
                table: "Skid",
                column: "ReferenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Skid_TagId",
                table: "Skid",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Stand_ReferenceDocumentId",
                table: "Stand",
                column: "ReferenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Stand_TagId",
                table: "Stand",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Stream_ProjectId",
                table: "Stream",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Stream_StreamName",
                table: "Stream",
                column: "StreamName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProcess_ProjectId",
                table: "SubProcess",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubSystem_SystemId",
                table: "SubSystem",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_System_Number",
                table: "System",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_System_WorkAreaPackId",
                table: "System",
                column: "WorkAreaPackId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_EquipmentCodeId",
                table: "Tag",
                column: "EquipmentCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ProcessId",
                table: "Tag",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_StreamId",
                table: "Tag",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_SubProcessId",
                table: "Tag",
                column: "SubProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_TagName",
                table: "Tag",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkAreaPack_Number",
                table: "WorkAreaPack",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkAreaPack_ProjectId",
                table: "WorkAreaPack",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cable");

            migrationBuilder.DropTable(
                name: "ChangeLog");

            migrationBuilder.DropTable(
                name: "ControlSystemHierarchy");

            migrationBuilder.DropTable(
                name: "DeviceAttributeValue");

            migrationBuilder.DropTable(
                name: "DeviceClassification");

            migrationBuilder.DropTable(
                name: "GSDType_SignalExtension");

            migrationBuilder.DropTable(
                name: "InstrumentListImport");

            migrationBuilder.DropTable(
                name: "JunctionBox");

            migrationBuilder.DropTable(
                name: "NatureOfSignalSignalExtension");

            migrationBuilder.DropTable(
                name: "NonInstrumentListImport");

            migrationBuilder.DropTable(
                name: "OLMDPTDPR");

            migrationBuilder.DropTable(
                name: "OMItem");

            migrationBuilder.DropTable(
                name: "OMServiceDescriptionImport");

            migrationBuilder.DropTable(
                name: "Panel");

            migrationBuilder.DropTable(
                name: "PnIdTag");

            migrationBuilder.DropTable(
                name: "ProcessHierarchy");

            migrationBuilder.DropTable(
                name: "ProjectUser");

            migrationBuilder.DropTable(
                name: "ReferenceDocumentDevice");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "SSISEquipmentList");

            migrationBuilder.DropTable(
                name: "SSISFittings");

            migrationBuilder.DropTable(
                name: "SSISInstruments");

            migrationBuilder.DropTable(
                name: "SSISInstrumentsVsStands110728");

            migrationBuilder.DropTable(
                name: "SSISStandList");

            migrationBuilder.DropTable(
                name: "SSISStandTypes");

            migrationBuilder.DropTable(
                name: "SSISValveList");

            migrationBuilder.DropTable(
                name: "Stand");

            migrationBuilder.DropTable(
                name: "UIChangeLog");

            migrationBuilder.DropTable(
                name: "AttributeValue");

            migrationBuilder.DropTable(
                name: "GSDType");

            migrationBuilder.DropTable(
                name: "SignalExtension");

            migrationBuilder.DropTable(
                name: "Skid");

            migrationBuilder.DropTable(
                name: "AttributeDefinition");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "ReferenceDocument");

            migrationBuilder.DropTable(
                name: "DeviceModel");

            migrationBuilder.DropTable(
                name: "DeviceType");

            migrationBuilder.DropTable(
                name: "FailState");

            migrationBuilder.DropTable(
                name: "NatureOfSignal");

            migrationBuilder.DropTable(
                name: "ProcessLevel");

            migrationBuilder.DropTable(
                name: "ServiceBank");

            migrationBuilder.DropTable(
                name: "ServiceTrain");

            migrationBuilder.DropTable(
                name: "ServiceZone");

            migrationBuilder.DropTable(
                name: "SubSystem");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "ReferenceDocumentType");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropTable(
                name: "System");

            migrationBuilder.DropTable(
                name: "EquipmentCode");

            migrationBuilder.DropTable(
                name: "Process");

            migrationBuilder.DropTable(
                name: "Stream");

            migrationBuilder.DropTable(
                name: "SubProcess");

            migrationBuilder.DropTable(
                name: "WorkAreaPack");
        }
    }
}
