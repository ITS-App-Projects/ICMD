using ICMD.Core.Shared.Interface;
using ICMD.Repository.Service;
using ICMD.Repository.ViewService;

using Microsoft.Extensions.DependencyInjection;

namespace ICMD.Infra.IoC
{
    public static class NativeInjectorBootstrapper
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectUserService, ProjectUserService>();
            services.AddScoped<ITagFieldService, TagFieldService>();
            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddScoped<INonInstrumentService, NonInstrumentService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IWorkAreaPackService, WorkAreaPackService>();
            services.AddScoped<ITrainService, TrainService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<ISubSystemService, SubSystemService>();
            services.AddScoped<IReferenceDocumentTypeService, ReferenceDocumentTypeService>();
            services.AddScoped<IProcessService, ProcessService>();
            services.AddScoped<ISubProcessService, SubProcessService>();
            services.AddScoped<IStreamService, StreamService>();
            services.AddScoped<IReferenceDocumentService, ReferenceDocumentService>();
            services.AddScoped<IAttributeDefinitionService, AttributeDefinitionService>();
            services.AddScoped<IAttributeValueService, AttributeValueService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IJunctionBoxService, JunctionBoxService>();
            services.AddScoped<IPanelService, PanelService>();
            services.AddScoped<ISkidService, SkidService>();
            services.AddScoped<IStandService, StandService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IReferenceDocumentDeviceService, ReferenceDocumentDeviceService>();
            services.AddScoped<IEquipmentCodeService, EquipmentCodeService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IDeviceModelService, DeviceModelService>();
            services.AddScoped<IFailStateService, FailStateService>();
            services.AddScoped<ITagTypeService, TagTypeService>();
            services.AddScoped<ITagDescriptorService, TagDescriptorService>();
            services.AddScoped<IDeviceTypeService, DeviceTypeService>();
            services.AddScoped<INatureOfSignalService, NatureOfSignalService>();
            services.AddScoped<ICableService, CableService>();
            services.AddScoped<IControlSystemHierarchyService, ControlSystemHierarchyService>();
            services.AddScoped<IDeviceAttributeValueService, DeviceAttributeValueService>();
            services.AddScoped<IUIChangeLogService, UIChangeLogService>();
            services.AddScoped<IOMItemService, OMItemService>();
            services.AddScoped<IOMDescriptionImportService, OMDescriptionImportService>();
            services.AddScoped<ISSISEquipmentListService, SSISEquipmentListService>();
            services.AddScoped<ISSISInstrumentsService, SSISInstrumentsService>();
            services.AddScoped<ISSISValveListService, SSISValveListService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IChangeLogService, ChangeLogService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IMenuPermissionService, MenuPermissionService>();
            services.AddScoped<IPermissionManagementService, PermissionManagementService>();
            services.AddScoped<IMetaDataService, MetaDataService>();
            services.AddScoped<ICableHierarchyService, CableHierarchyService>();

            #region Views
            services.AddScoped<TagViewService>();
            services.AddScoped<ViewDeviceInstrumentService>();
            services.AddScoped<ViewAllAttributesService>();
            services.AddScoped<ViewAllDocumentsService>();
            services.AddScoped<ViewInstrumentListLiveService>();
            services.AddScoped<ViewNonInstrumentListService>();
            services.AddScoped<ViewCountDPPADeviceService>();
            services.AddScoped<ViewPnIDTagExceptionService>();
            services.AddScoped<ViewPnIDDeviceDocumentReferenceCompareService>();
            services.AddScoped<ViewOMItemInstrumentListService>();
            services.AddScoped<ViewUnassociatedTagsService>();
            services.AddScoped<ViewUnassociatedSkidsService>();
            services.AddScoped<ViewUnassociatedStandsService>();
            services.AddScoped<ViewUnassociatedJunctionBoxesService>();
            services.AddScoped<ViewUnassociatedPanelsService>();
            services.AddScoped<ViewNatureOfSignalValidationFailuresService>();
            services.AddScoped<ViewPSSTagsService>();
            #endregion
        }
    }
}
