using ICMD.Core.Authorization;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.ViewDto;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ICMD.EntityFrameworkCore.Database
{
    public class ICMDDbContext : IdentityDbContext<ICMDUser, ICMDRole, Guid, ICMDUserClaim, ICMDUserRole, ICMDUserLogin, ICMDRoleClaim, ICMDUserToken>
    {
        private readonly IConfiguration _configuration;
        public ICMDDbContext(DbContextOptions<ICMDDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
            //Database.Migrate();
        }

        #region DB Set
        #region _A_
        public virtual DbSet<AttributeDefinition> AttributeDefinition { get; set; }
        public virtual DbSet<AttributeValue> AttributeValue { get; set; }
        #endregion

        #region _B_
        #endregion

        #region _C_
        public virtual DbSet<ChangeLog> ChangeLog { get; set; }
        public virtual DbSet<Cable> Cable { get; set; }
        public virtual DbSet<ControlSystemHierarchy> ControlSystemHierarchy { get; set; }
        public virtual DbSet<CableHierarchy> CableHierarchy { get; set; }
        #endregion

        #region _D_
        public virtual DbSet<DeviceClassification> DeviceClassification { get; set; }
        public virtual DbSet<DeviceType> DeviceType { get; set; }
        public virtual DbSet<DeviceModel> DeviceModel { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceAttributeValue> DeviceAttributeValue { get; set; }
        #endregion

        #region _E_
        public virtual DbSet<EquipmentCode> EquipmentCode { get; set; }
        #endregion

        #region _F_
        public virtual DbSet<FailState> FailState { get; set; }
        #endregion

        #region _G_
        public virtual DbSet<GSDType> GSDType { get; set; }
        public virtual DbSet<GSDType_SignalExtension> GSDType_SignalExtension { get; set; }
        #endregion

        #region _H_
        #endregion

        #region _I_
        public virtual DbSet<ICMDUser> ICMDUser { get; set; }
        public virtual DbSet<ICMDRole> ICMDRole { get; set; }
        public virtual DbSet<ICMDUserRole> ICMDUserRole { get; set; }
        public virtual DbSet<ICMDUserLogin> ICMDUserLogin { get; set; }
        public virtual DbSet<ICMDRoleClaim> ICMDRoleClaim { get; set; }
        public virtual DbSet<ICMDUserClaim> ICMDUserClaim { get; set; }
        public virtual DbSet<ICMDUserToken> ICMDUserToken { get; set; }
        public virtual DbSet<InstrumentListImport> InstrumentListImport { get; set; }
        #endregion

        #region _J_
        public virtual DbSet<JunctionBox> JunctionBox { get; set; }
        #endregion

        #region _K_
        #endregion

        #region _L_
        #endregion

        #region _M_
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<MenuItems> MenuItems { get; set; }
        public virtual DbSet<MenuPermission> MenuPermission { get; set; }
        public virtual DbSet<MetaData> MetaData { get; set; }
        #endregion

        #region _N_
        public virtual DbSet<NonInstrumentListImport> NonInstrumentListImport { get; set; }
        public virtual DbSet<NatureOfSignal> NatureOfSignal { get; set; }
        public virtual DbSet<NatureOfSignalSignalExtension> NatureOfSignalSignalExtension { get; set; }
        #endregion

        #region _O_
        public virtual DbSet<OLMDPTDPR> OLMDPTDPR { get; set; }
        public virtual DbSet<OMServiceDescriptionImport> OMServiceDescriptionImport { get; set; }
        public virtual DbSet<OMItem> OMItem { get; set; }
        #endregion

        #region _P_
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Process> Process { get; set; }
        public virtual DbSet<Panel> Panel { get; set; }
        public virtual DbSet<ProcessLevel> ProcessLevel { get; set; }
        public virtual DbSet<ProcessHierarchy> ProcessHierarchy { get; set; }
        public virtual DbSet<PnIdTag> PnIdTag { get; set; }
        public virtual DbSet<ProjectUser> ProjectUser { get; set; }
        public virtual DbSet<PermissionManagement> PermissionManagement { get; set; }
        #endregion

        #region _Q_
        #endregion

        #region _R_
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<ReferenceDocumentType> ReferenceDocumentType { get; set; }
        public virtual DbSet<ReferenceDocument> ReferenceDocument { get; set; }
        public virtual DbSet<ReferenceDocumentDevice> ReferenceDocumentDevice { get; set; }
        #endregion

        #region _S_
        public virtual DbSet<SSISValveList> SSISValveList { get; set; }
        public virtual DbSet<SSISStandTypes> SSISStandTypes { get; set; }
        public virtual DbSet<SSISInstruments> SSISInstruments { get; set; }
        public virtual DbSet<SSISFittings> SSISFittings { get; set; }
        public virtual DbSet<SSISEquipmentList> SSISEquipmentList { get; set; }
        public virtual DbSet<SSISInstrumentsVsStands110728> SSISInstrumentsVsStands110728 { get; set; }
        public virtual DbSet<SSISStandList> SSISStandList { get; set; }
        public virtual DbSet<SubProcess> SubProcess { get; set; }
        public virtual DbSet<Core.DBModels.System> System { get; set; }
        public virtual DbSet<Core.DBModels.Stream> Stream { get; set; }
        public virtual DbSet<SubSystem> SubSystem { get; set; }
        public virtual DbSet<SignalExtension> SignalExtension { get; set; }
        public virtual DbSet<ServiceBank> ServiceBank { get; set; }
        public virtual DbSet<ServiceTrain> ServiceTrain { get; set; }
        public virtual DbSet<ServiceZone> ServiceZone { get; set; }
        public virtual DbSet<Stand> Stand { get; set; }
        public virtual DbSet<Skid> Skid { get; set; }
        #endregion

        #region _T_
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TagField> TagField { get; set; }
        public virtual DbSet<TagType> TagType { get; set; }
        public virtual DbSet<TagDescriptor> TagDescriptor { get; set; }
        #endregion

        #region _U_
        public virtual DbSet<UIChangeLog> UIChangeLog { get; set; }
        #endregion

        #region _V_
        #endregion

        #region _W_
        public virtual DbSet<WorkAreaPack> WorkAreaPack { get; set; }
        #endregion

        #region _X_
        #endregion

        #region _Y_
        #endregion

        #region _Z_
        #endregion

        #endregion

        #region Views
        public DbSet<ViewTagDto> ViewTag { get; set; }
        public DbSet<ViewDeviceInstrumentsDto> ViewDeviceInstrumentsDto { get; set; }
        public DbSet<ViewAllAttributesDto> ViewAllAttributesDto { get; set; }
        public DbSet<ViewAllDocumentsDto> ViewAllDocumentsDto { get; set; }
        public DbSet<ViewInstrumentListLiveDto> ViewInstrumentLiveDto { get; set; }
        public DbSet<ViewNonInstrumentListDto> ViewNonInstrumentListDto { get; set; }
        public DbSet<ViewCountDPPADevicesDto> ViewCountDPPADevicesDto { get; set; }
        public DbSet<ViewPnIDTagExceptionDto> ViewPnIDTagExceptionDto { get; set; }
        public DbSet<ViewPnIDDeviceDocumentReferenceCompareDto> ViewPnIDDeviceDocumentReferenceCompareDto { get; set; }
        public DbSet<ViewOMItemInstrumentListDto> ViewOMItemInstrumentListDto { get; set; }
        public DbSet<ViewUnassociatedTagsDto> ViewUnassociatedTagsDto { get; set; }
        public DbSet<ViewUnassociatedSkidsDto> ViewUnassociatedSkidsDto { get; set; }
        public DbSet<ViewUnassociatedStandsDto> ViewUnassociatedStandsDto { get; set; }
        public DbSet<ViewUnassociatedJunctionBoxesDto> ViewUnassociatedJunctionBoxesDto { get; set; }
        public DbSet<ViewUnassociatedPanelsDto> ViewUnassociatedPanelsDto { get; set; }
        public DbSet<ViewNatureOfSignalValidationFailuresDto> ViewNatureOfSignalValidationFailuresDto { get; set; }
        public DbSet<ViewPSSTagsDto> ViewPSSTagsDto { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Entity Names
            modelBuilder.Entity<ICMDUser>().ToTable("ICMDUser");
            modelBuilder.Entity<ICMDRole>().ToTable("ICMDRole");
            modelBuilder.Entity<ICMDUserRole>().ToTable("ICMDUserRole");
            modelBuilder.Entity<ICMDUserLogin>().ToTable("ICMDUserLogin");
            modelBuilder.Entity<ICMDRoleClaim>().ToTable("ICMDRoleClaim");
            modelBuilder.Entity<ICMDUserClaim>().ToTable("ICMDUserClaim");
            modelBuilder.Entity<ICMDUserToken>().ToTable("ICMDUserToken");


            modelBuilder.Entity<GSDType>().HasIndex(u => u.GSDTypeName).IsUnique();
            modelBuilder.Entity<ProcessLevel>().HasIndex(u => u.Name).IsUnique();

            modelBuilder.Entity<ViewTagDto>().ToView("View_Tag").HasKey(t => t.Id);
            modelBuilder.Entity<ViewDeviceInstrumentsDto>().ToView("View_Device_Instruments").HasKey(t => t.Id);
            modelBuilder.Entity<ViewAllAttributesDto>().ToView("View_AllAttributes").HasKey(t => t.Id);
            modelBuilder.Entity<ViewAllDocumentsDto>().ToView("View_AllDocuments").HasKey(t => t.DeviceId);
            modelBuilder.Entity<ViewInstrumentListLiveDto>().ToView("View_InstrumentListLive").HasKey(t => t.DeviceId);

            //Non-Instruments
            modelBuilder.Entity<ViewDeviceInstrumentsDto>().ToView("View_Device_NonInstruments").HasKey(t => t.Id);
            modelBuilder.Entity<ViewNonInstrumentListDto>().ToView("View_NonInstrumentList").HasKey(t => t.DeviceId);
            modelBuilder.Entity<ViewCountDPPADevicesDto>().ToView("View_CountDPPADevices").HasNoKey();
            modelBuilder.Entity<ViewPnIDTagExceptionDto>().ToView("View_PnIDTagException").HasNoKey();
            modelBuilder.Entity<ViewPnIDDeviceDocumentReferenceCompareDto>().ToView("View_PnID_Device_DocumentReferenceCompare").HasNoKey();
            modelBuilder.Entity<ViewOMItemInstrumentListDto>().ToView("View_OMItem_InstrumentList").HasNoKey();
            modelBuilder.Entity<ViewUnassociatedTagsDto>().ToView("View_UnassociatedTags").HasKey(a => a.Id);
            modelBuilder.Entity<ViewUnassociatedSkidsDto>().ToView("View_UnassociatedSkids").HasKey(a => a.Id);
            modelBuilder.Entity<ViewUnassociatedStandsDto>().ToView("View_UnassociatedStands").HasKey(a => a.Id);
            modelBuilder.Entity<ViewUnassociatedJunctionBoxesDto>().ToView("View_UnassociatedJunctionBoxes").HasKey(a => a.Id);
            modelBuilder.Entity<ViewUnassociatedPanelsDto>().ToView("View_UnassociatedPanels").HasKey(a => a.Id);
            modelBuilder.Entity<ViewNatureOfSignalValidationFailuresDto>().ToView("View_NatureOfSignalValidationFailures").HasKey(a => a.DeviceId);
            modelBuilder.Entity<ViewPSSTagsDto>().ToView("View_PSSTags").HasNoKey();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DBConnectionString"));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
