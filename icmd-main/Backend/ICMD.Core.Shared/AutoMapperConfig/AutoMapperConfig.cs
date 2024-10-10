using AutoMapper;
using ICMD.Core.Authorization;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Bank;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.DeviceModel;
using ICMD.Core.Dtos.DeviceType;
using ICMD.Core.Dtos.EquipmentCode;
using ICMD.Core.Dtos.FailState;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Dtos.JunctionBox;
using ICMD.Core.Dtos.Manufacturer;
using ICMD.Core.Dtos.Menu;
using ICMD.Core.Dtos.MetaData;
using ICMD.Core.Dtos.NatureOfSignal;
using ICMD.Core.Dtos.Process;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Dtos.Reference_Document;
using ICMD.Core.Dtos.ReferenceDocumentType;
using ICMD.Core.Dtos.Reports;
using ICMD.Core.Dtos.Role;
using ICMD.Core.Dtos.Stand;
using ICMD.Core.Dtos.Stream;
using ICMD.Core.Dtos.SubProcess;
using ICMD.Core.Dtos.SubSystem;
using ICMD.Core.Dtos.System;
using ICMD.Core.Dtos.Tag;
using ICMD.Core.Dtos.TagDescriptor;
using ICMD.Core.Dtos.TagField;
using ICMD.Core.Dtos.TagType;
using ICMD.Core.Dtos.Train;
using ICMD.Core.Dtos.User;
using ICMD.Core.Dtos.WorkAreaPack;
using ICMD.Core.Dtos.Zone;
using ICMD.Core.ViewDto;

namespace ICMD.Core.Shared.AutoMapperConfig
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            //ICMDUser
            CreateMap<CreateOrEditUserDto, ICMDUser>().ReverseMap();
            CreateMap<UpdateUserDto, ICMDUser>().ReverseMap();
            CreateMap<ICMDUser, UserInfoDto>().ReverseMap();
            CreateMap<ICMDUser, UserDropdownDto>().ReverseMap();


            //ICMDUserRole
            CreateMap<RoleDropDownDto, ICMDRole>().ReverseMap();

            //Project
            CreateMap<CreateOrEditProjectDto, Project>().ReverseMap();
            CreateMap<UserAuthorizationDto, ProjectUser>().ReverseMap();
            CreateMap<Project, ProjectInfoDto>().ReverseMap();
            CreateMap<Project, DropdownInfoDto>().ReverseMap();

            //Instrument
            CreateMap<InstrumentListDto, InstrumentListImport>().ReverseMap();

            //Bank
            CreateMap<CreateOrEditBankDto, ServiceBank>().ReverseMap();
            CreateMap<ServiceBank, BankInfoDto>().ReverseMap();
            CreateMap<CreateOrEditBankDto, BankInfoDto>().ReverseMap();

            //WorkAreaPack
            CreateMap<CreateOrEditWorkAreaPackDto, WorkAreaPack>().ReverseMap();
            CreateMap<WorkAreaPack, WorkAreaPackInfoDto>().ReverseMap();
            CreateMap<CreateOrEditWorkAreaPackDto, WorkAreaPackInfoDto>().ReverseMap();

            //Train
            CreateMap<CreateOrEditTrainDto, ServiceTrain>().ReverseMap();
            CreateMap<ServiceTrain, TrainInfoDto>().ReverseMap();
            CreateMap<CreateOrEditTrainDto, TrainInfoDto>().ReverseMap();

            //Zone
            CreateMap<CreateOrEditZoneDto, ServiceZone>().ReverseMap();
            CreateMap<ServiceZone, ZoneInfoDto>().ReverseMap();
            CreateMap<CreateOrEditZoneDto, ZoneInfoDto>().ReverseMap();

            //System
            CreateMap<CreateOrEditSystemDto, Core.DBModels.System>().ReverseMap();
            CreateMap<Core.DBModels.System, SystemInfoDto>().ReverseMap();
            CreateMap<CreateOrEditSystemDto, SystemInfoDto>().ReverseMap();

            //DocumentType
            CreateMap<CreateOrEditReferenceDocumentType, ReferenceDocumentType>().ReverseMap();
            CreateMap<ReferenceDocumentType, TypeInfoDto>().ReverseMap();
            CreateMap<CreateOrEditReferenceDocumentType, TypeInfoDto>().ReverseMap();

            //SubSystem
            CreateMap<CreateOrEditSubSystemDto, SubSystem>().ReverseMap();
            CreateMap<SubSystem, SubSystemInfoDto>().ReverseMap();
            CreateMap<CreateOrEditSubSystemDto, SubSystemInfoDto>().ReverseMap();

            //Process
            CreateMap<CreateOrEditProcessDto, Process>().ReverseMap();
            CreateMap<Process, ProcessInfoDto>().ReverseMap();
            CreateMap<CreateOrEditProcessDto, ProcessInfoDto>().ReverseMap();

            //SubProcess
            CreateMap<CreateOrEditSubProcessDto, SubProcess>().ReverseMap();
            CreateMap<SubProcess, SubProcessInfoDto>().ReverseMap();
            CreateMap<CreateOrEditSubProcessDto, SubProcessInfoDto>().ReverseMap();

            //Stream
            CreateMap<CreateOrEditStreamDto, Core.DBModels.Stream>().ReverseMap();
            CreateMap<Core.DBModels.Stream, StreamInfoDto>().ReverseMap();
            CreateMap<CreateOrEditStreamDto, StreamInfoDto>().ReverseMap();

            //ReferenceDocument
            CreateMap<CreateOrEditReferenceDocumentDto, ReferenceDocument>().ReverseMap();
            CreateMap<ReferenceDocument, ReferenceDocumentInfoDto>().ReverseMap();
            CreateMap<CreateOrEditReferenceDocumentDto, ReferenceDocumentInfoDto>().ReverseMap();

            //EquipmentCode
            CreateMap<CreateOrEditEquipmentCodeDto, EquipmentCode>().ReverseMap();
            CreateMap<EquipmentCode, EquipmentCodeInfoDto>().ReverseMap();
            CreateMap<CreateOrEditEquipmentCodeDto, EquipmentCodeInfoDto>().ReverseMap();

            //Manufacturer
            CreateMap<CreateOrEditManufacturerDto, Manufacturer>().ReverseMap();
            CreateMap<Manufacturer, ManufacturerInfoDto>().ReverseMap();
            CreateMap<DropdownInfoDto, Manufacturer>().ReverseMap();
            CreateMap<CreateOrEditManufacturerDto, ManufacturerInfoDto>().ReverseMap();


            //FailState
            CreateMap<CreateOrEditFailStateDto, FailState>().ReverseMap();
            CreateMap<FailState, FailStateInfoDto>().ReverseMap();
            CreateMap<CreateOrEditFailStateDto, FailStateInfoDto>().ReverseMap();

            //TagType
            CreateMap<CreateOrEditTagTypeDto, TagType>().ReverseMap();
            CreateMap<TagType, TagTypeInfoDto>().ReverseMap();
            CreateMap<TagDescriptor, TagTypeInfoDto>().ReverseMap();
            CreateMap<CreateOrEditTagTypeDto, TagTypeInfoDto>().ReverseMap();

            //TagDescriptor
            CreateMap<CreateOrEditTagDescriptorDto, TagDescriptor>().ReverseMap();
            CreateMap<CreateOrEditTagDescriptorDto, TagTypeInfoDto>().ReverseMap();

            //DeviceModel
            CreateMap<CreateOrEditDeviceModelDto, DeviceModel>().ReverseMap();
            CreateMap<DeviceModel, DeviceModelInfoDto>().ReverseMap();
            CreateMap<CreateOrEditDeviceModelDto, DeviceModelListDto>().ReverseMap();

            //DeviceType
            CreateMap<CreateOrEditDeviceTypeDto, DeviceType>().ReverseMap();
            CreateMap<DeviceType, DeviceTypeInfoDto>().ReverseMap();
            CreateMap<CreateOrEditDeviceTypeDto, DeviceTypeListDto>().ReverseMap();

            //NatureOfSignal
            CreateMap<CreateOrEditNatureOfSignalDto, NatureOfSignal>().ReverseMap();
            CreateMap<CreateOrEditNatureOfSignalDto, NatureOfSignalListDto>().ReverseMap();
            CreateMap<CreateOrEditNatureOfSignalDto, NatureOfSignalExportDto>()
                .ForMember(destination => destination.Name,
                    opt => opt.MapFrom(source => source.NatureOfSignalName));

            //TagField
            CreateMap<TagFieldInfoDto, TagField>().ReverseMap();
            CreateMap<ProjectTagFieldInfoDto, TagField>().ReverseMap();

            //Tag
            CreateMap<TagInfoDto, Tag>().ReverseMap();
            CreateMap<CreateOrEditTagDto, Tag>().ReverseMap();
            CreateMap<SourceDataInfoDto, TagType>().ReverseMap();
            CreateMap<SourceDataInfoDto, TagDescriptor>().ReverseMap();
            CreateMap<SourceDataInfoDto, TagDescriptor>().ReverseMap();

            //Attributes
            CreateMap<AttributesDto, AttributeDefinition>().ReverseMap();
            CreateMap<AttributesDto, AttributeValue>().ReverseMap();
            CreateMap<AttributeValueDto, AttributeValue>().ReverseMap();

            //JunctionBox
            CreateMap<CreateOrEditJunctionBoxDto, JunctionBox>().ReverseMap();
            CreateMap<CreateOrEditJunctionBoxDto, JunctionBoxListDto>().ReverseMap();

            //Panel
            CreateMap<CreateOrEditJunctionBoxDto, Panel>().ReverseMap();

            //Skid
            CreateMap<CreateOrEditJunctionBoxDto, Skid>().ReverseMap();

            //Stand
            CreateMap<CreateOrEditStandDto, Stand>().ReverseMap();
            CreateMap<CreateOrEditStandDto, JunctionBoxListDto>().ReverseMap();

            //Device
            CreateMap<CreateOrEditDeviceDto, Device>().ReverseMap();
            CreateMap<Device, DeviceInfoDto>().ReverseMap();
            CreateMap<Device, ViewDeviceInfoDto>().ReverseMap();

            //Reports
            CreateMap<ReportInfoDto, Report>().ReverseMap();
            CreateMap<DPPADevicesDto, ViewCountDPPADevicesDto>().ReverseMap();

            //Menu
            CreateMap<MenuInfoDto, MenuItems>().ReverseMap();
            CreateMap<MenuInfoDto, MenuItems>().ReverseMap();
            //CreateMap<MainMenuInfoDto, MenuItems>().ReverseMap();
            CreateMap<RoleMenuPermissionDto, MenuPermission>().ReverseMap();
            CreateMap<MainMenuDto, MenuItems>().ReverseMap();
            CreateMap<SubMenuDto, MenuItems>().ReverseMap();
            CreateMap<CreateOrEditMenuDto, MenuItems>().ReverseMap();
            CreateMap<MenuItems, MenuForViewDto>().ReverseMap();
            CreateMap<MenuItemDto, MenuItems>().ReverseMap();

            
            //CreateMap<CreateOrEditPermissionDto, Permissions>().ReverseMap();
            //CreateMap<PermissionForViewDto, Permissions>().ReverseMap();
            CreateMap<MenuPermissionDto, MenuPermission>().ReverseMap();

            CreateMap<PermissionByMenuRoleDto, PermissionManagement>().ReverseMap();

            //MetaData
            CreateMap<MetaDataDto, MetaData>().ReverseMap();
            CreateMap<CreateMetaDataDto, MetaData>().ReverseMap();
        }
    }
}
