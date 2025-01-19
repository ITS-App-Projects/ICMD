using System.Data;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Dtos.Reference_Document;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using ICMD.EntityFrameworkCore.Database;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using NpgsqlTypes;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTypeService _deviceTyperService;
        private readonly IDeviceModelService _deviceModelService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IFailStateService _failStateService;
        private readonly IZoneService _zoneService;
        private readonly IBankService _bankService;
        private readonly ITrainService _trainService;
        private readonly INatureOfSignalService _natureOfSignalService;
        private readonly IPanelService _panelService;
        private readonly ISkidService _skidService;
        private readonly IStandService _standService;
        private readonly IJunctionBoxService _junctionBoxService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IReferenceDocumentTypeService _referenceDocumentTypeService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly IControlSystemHierarchyService _controlSystemHierarchyService;
        private readonly IReferenceDocumentDeviceService _referenceDocumentDeviceService;
        private readonly CommonMethods _commonMethods;
        private readonly IMapper _mapper;
        private readonly ICMDDbContext _dbContext;
        private readonly IAttributeValueService _attributeValueService;
        private readonly IAttributeDefinitionService _attributeDefinitionService;
        private readonly IDeviceAttributeValueService _deviceAttributeValueService;
        private readonly ITagService _tagService;
        private readonly StoredProcedureHelper _storedProcedureHelper;
        private readonly ChangeLogHelper _changeLogHelper;
        private readonly ICableHierarchyService _cableHierarchyService;
        private static string ModuleName = "Device";
        public DeviceController(IMapper mapper, IDeviceService deviceService, IDeviceTypeService deviceTyperService, IDeviceModelService deviceModelService,
            IManufacturerService manufacturerService, IFailStateService failStateService, IZoneService zoneService, IBankService bankService, ITrainService trainService, INatureOfSignalService natureOfSignalService,
            IPanelService panelService, ISkidService skidService, IStandService standService, IJunctionBoxService junctionBoxService, IReferenceDocumentService referenceDocumentService,
            IReferenceDocumentTypeService referenceDocumentTypeService, IWorkAreaPackService workAreaPackService, IControlSystemHierarchyService controlSystemHierarchyService,
            IReferenceDocumentDeviceService referenceDocumentDeviceService, CommonMethods commonMethods,
            ICMDDbContext dbContext, IAttributeValueService attributeValueService, IAttributeDefinitionService attributeDefinitionService, IDeviceAttributeValueService deviceAttributeValueService, ITagService tagService,
            StoredProcedureHelper storedProcedureHelper, ChangeLogHelper changeLogHelper, ICableHierarchyService cableSystemHierarchyService)
        {
            _mapper = mapper;
            _deviceService = deviceService;
            _deviceTyperService = deviceTyperService;
            _deviceModelService = deviceModelService;
            _manufacturerService = manufacturerService;
            _failStateService = failStateService;
            _zoneService = zoneService;
            _bankService = bankService;
            _trainService = trainService;
            _natureOfSignalService = natureOfSignalService;
            _panelService = panelService;
            _skidService = skidService;
            _standService = standService;
            _junctionBoxService = junctionBoxService;
            _referenceDocumentService = referenceDocumentService;
            _referenceDocumentTypeService = referenceDocumentTypeService;
            _workAreaPackService = workAreaPackService;
            _commonMethods = commonMethods;
            _controlSystemHierarchyService = controlSystemHierarchyService;
            _referenceDocumentDeviceService = referenceDocumentDeviceService;
            _dbContext = dbContext;
            _attributeValueService = attributeValueService;
            _attributeDefinitionService = attributeDefinitionService;
            _deviceAttributeValueService = deviceAttributeValueService;
            _tagService = tagService;
            _storedProcedureHelper = storedProcedureHelper;
            _changeLogHelper = changeLogHelper;
            _cableHierarchyService = cableSystemHierarchyService;
        }
        #region Device
        [HttpGet]
        public async Task<DeviceDropdownInfoDto> DeviceDropdownInfo(Guid projectId, Guid? deviceId)
        {
            DeviceDropdownInfoDto info = new DeviceDropdownInfoDto();
            //DeviceType
            info.DeviceTypes = await _deviceTyperService.GetAll(s => s.IsActive && !s.IsDeleted).OrderBy(a => a.Type)
                .OrderBy(s => s.Type).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = (!string.IsNullOrEmpty(s.Description)) ? s.Type + " - " + s.Description : s.Type
                }).ToListAsync();

            //Tag
            List<Tag> allTags = await _tagService.GetAll(s => s.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();


            List<Stand> allStands = await _standService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            List<Skid> allSkids = await _skidService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && s.IsActive && !s.IsDeleted).ToListAsync();
            List<Panel> allPanels = await _panelService.GetAll(s => s.Tag.ProjectId == projectId && !s.IsDeleted).ToListAsync();
            List<Device> allDevices = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            List<JunctionBox> allJunctionBox = await _junctionBoxService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            info.TagList = (from tg in allTags
                            join st in allStands on tg.Id equals st.TagId into Stand
                            from st in Stand.DefaultIfEmpty()

                            join sk in allSkids on tg.Id equals sk.TagId into Skid
                            from sk in Skid.DefaultIfEmpty()

                            join pn in allPanels on tg.Id equals pn.TagId into Panel
                            from pn in Panel.DefaultIfEmpty()

                            join dv in allDevices on tg.Id equals dv.TagId into Device
                            from dv in Device.DefaultIfEmpty()

                            join jb in allJunctionBox on tg.Id equals jb.TagId into Junction
                            from jb in Junction.DefaultIfEmpty()
                            where Stand.Count() == 0 && Skid.Count() == 0 && Panel.Count() == 0 && Device.Count() == 0 && Junction.Count() == 0
                            select new DropdownInfoDto
                            {
                                Id = tg.Id,
                                Name = tg.TagName,
                            }).ToList();
            info.CableDeviceTagList = allTags.Select(tag => new DropdownInfoDto
            {
                Id = tag.Id,
                Name = tag.TagName
            }).ToList();

            DropdownInfoDto? currentTag = new DropdownInfoDto();
            if (deviceId != null && deviceId != Guid.Empty)
            {
                currentTag = await _deviceService.GetAll(s => s.Id == deviceId && s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).FirstOrDefaultAsync();

                if (currentTag != null)
                    info.TagList.Add(currentTag);
            }

            //Manufacturer
            info.ManufacturerList = _mapper.Map<List<DropdownInfoDto>>(await _manufacturerService.GetAll(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.Name).ToListAsync());

            //FailState
            info.FailStateList = await _failStateService.GetAll(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.FailStateName).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.FailStateName
                }).ToListAsync();

            //Zone
            info.ZoneList = await _zoneService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId)
                .OrderBy(s => s.Zone).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Zone
                }).ToListAsync();


            //Bank
            info.BankList = await _bankService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId)
                .OrderBy(s => s.Bank).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Bank
                }).ToListAsync();

            //Train
            info.TrainList = await _trainService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId)
                .OrderBy(s => s.Train).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Train
                }).ToListAsync();

            //NatureOfSignalList
            info.NatureOfSignalList = await _natureOfSignalService.GetAll(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.NatureOfSignalName).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.NatureOfSignalName
                }).ToListAsync();

            //FieldPanelList
            info.FieldPanelList = allPanels
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToList();

            //Skid
            info.SkidList = allSkids
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToList();

            //Stand
            info.StandList = allStands
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToList();

            //JunctionBox
            info.JunctionBoxList = allJunctionBox
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToList();

            //IsInstrumentList
            info.IsInstrumentOptionsList = _commonMethods.IsInstrumentSelectList();

            //ReferenceDocumentList
            info.ReferenceDocTypeList = await _referenceDocumentTypeService.GetAll(s => s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Type).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Type
                }).ToListAsync();

            //ReferenceDocumentList
            info.WorkAreaPackList = await _workAreaPackService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProjectId == projectId)
                .OrderBy(s => s.Number).Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Number
                }).ToListAsync();

            info.ConnectionTagList = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToListAsync();

            info.InstrumentTagList = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted && (s.IsInstrument == IsInstrumentOption.Yes
            || s.IsInstrument == IsInstrumentOption.Both))
                .OrderBy(s => s.Tag.TagName).Select(s => new DropdownInfoDto
                {
                    Id = s.TagId,
                    Name = s.Tag.TagName
                }).ToListAsync();

            if (currentTag != null)
            {
                var connectionInfo = info.ConnectionTagList.FirstOrDefault(a => a.Id == currentTag.Id);
                if (connectionInfo != null)
                {
                    info.ConnectionTagList.Remove(connectionInfo);
                }

                var instrumentInfo = info.InstrumentTagList.FirstOrDefault(a => a.Id == currentTag.Id);
                if (instrumentInfo != null)
                {
                    info.InstrumentTagList.Remove(instrumentInfo);
                }
            }


            return info;
        }

        [HttpGet]
        public async Task<DeviceInfoDto?> GetDeviceInfo(Guid id)
        {
            Device? deviceDetails = await _deviceService.GetAll(s => !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (deviceDetails != null)
            {
                DeviceInfoDto deviceInfo = _mapper.Map<DeviceInfoDto>(deviceDetails);
                deviceInfo.ManufacturerId = deviceDetails.DeviceModel != null ? deviceDetails.DeviceModel.ManufacturerId : null;
                if (deviceDetails.SubSystem != null)
                {
                    deviceInfo.SystemId = deviceDetails.SubSystem.System != null ? deviceDetails.SubSystem.SystemId : null;
                    if (deviceDetails.SubSystem.System != null)
                    {
                        deviceInfo.WorkAreaPackId = deviceDetails.SubSystem.System.WorkAreaPackId;
                    }
                }
                List<ReferenceDocumentDevice> referenceDocuments = await _referenceDocumentDeviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceId == id).ToListAsync();
                deviceInfo.ReferenceDocumentIds = referenceDocuments.Select(s => s.ReferenceDocumentId).ToList();
                deviceInfo.ReferenceDocumentInfo = _mapper.Map<List<ReferenceDocumentInfoDto>>(referenceDocuments.Select(s => s.ReferenceDocument).ToList());
                foreach (var item in deviceInfo.ReferenceDocumentInfo)
                {
                    item.ReferenceDocumentType = referenceDocuments.Where(s => s.ReferenceDocumentId == item.Id).FirstOrDefault()?.ReferenceDocument?.ReferenceDocumentType?.Type ?? "";
                }

                ControlSystemHierarchy? originalConnectionParent = await _controlSystemHierarchyService.GetAll(x => x.Instrument == false && x.ChildDeviceId == id && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.ConnectionParentTagId = originalConnectionParent != null && originalConnectionParent.ParentDevice != null ? originalConnectionParent.ParentDevice.TagId : null;

                ControlSystemHierarchy? originalInstrumentParent = await _controlSystemHierarchyService.GetAll(x => x.Instrument == true && x.ChildDeviceId == id && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.InstrumentParentTagId = originalInstrumentParent != null && originalInstrumentParent.ParentDevice != null ? originalInstrumentParent.ParentDevice.TagId : null;

                CableHierarchy? originalConnectionCable = await _cableHierarchyService.GetAll(x => x.Instrument == false && x.DestinationDeviceId == id && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.ConnectionCableTagId = originalConnectionCable != null && originalConnectionCable.OriginDevice != null ? originalConnectionCable.OriginDevice.TagId : null;
                CableHierarchy? originalInstrumentCable = await _cableHierarchyService.GetAll(x => x.Instrument == true && x.DestinationDeviceId == id && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.InstrumentCableTagId = originalInstrumentCable != null && originalInstrumentCable.OriginDevice != null ? originalInstrumentCable.OriginDevice.TagId : null;

                List<AttributeDefinitionIdsDto> attributeDefinitions = await GetAttributeDefinitions(deviceDetails.DeviceTypeId, deviceDetails.DeviceModelId, deviceDetails.NatureOfSignalId, deviceInfo.ConnectionParentTagId, deviceDetails.Tag.ProjectId);
                List<AttributeValueDto> attributes = new List<AttributeValueDto>();
                foreach (var item in attributeDefinitions)
                {
                    var attributeValue = (deviceDetails != null) ?
                                         await _attributeValueService.GetAll(a => a.IsActive && !a.IsDeleted && a.DeviceId == id && a.AttributeDefinitionId == item.AttributeDefinitionId).FirstOrDefaultAsync() : null;
                    var attributeDefinitionInfo = await _attributeDefinitionService.GetAll(a => a.IsActive && !a.IsDeleted && a.Id == item.AttributeDefinitionId).FirstOrDefaultAsync();
                    if (attributeDefinitionInfo != null)
                    {
                        var info = new AttributeValueDto()
                        {
                            Id = attributeDefinitionInfo.Id,
                            Name = attributeDefinitionInfo.Name,
                            ValueType = attributeDefinitionInfo.ValueType,
                            Required = attributeDefinitionInfo.Required,
                            Value = attributeValue != null ? attributeValue.Value : ""
                        };
                        attributes.Add(info);
                    }
                }
                deviceInfo.Attributes = attributes;
                return deviceInfo;
            }
            return null;
        }

        [HttpGet]
        public async Task<ViewDeviceInfoDto?> ViewDeviceInfo(Guid id)
        {
            Device? deviceDetails = await _deviceService.GetAll(s => !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (deviceDetails != null)
            {
                List<KeyValueInfoDto> list = _commonMethods.IsInstrumentSelectList();
                ViewDeviceInfoDto deviceInfo = _mapper.Map<ViewDeviceInfoDto>(deviceDetails);
                deviceInfo.Tag = deviceDetails.Tag.TagName;
                deviceInfo.DeviceType = deviceDetails.DeviceType.Type + " " + deviceDetails.DeviceType.Description;
                deviceInfo.IsInstrument = list.FirstOrDefault(a => a.Value == deviceInfo.IsInstrument)?.Key ?? "";
                deviceInfo.Status = (!deviceDetails.IsActive) ? "Inactive" : "Active";
                deviceInfo.ProcessName = deviceDetails.Tag != null && deviceDetails.Tag.Process != null ? deviceDetails.Tag.Process.ProcessName : "";
                deviceInfo.SubProcessName = deviceDetails.Tag != null && deviceDetails.Tag.SubProcess != null ? deviceDetails.Tag.SubProcess.SubProcessName : "";
                deviceInfo.StreamName = deviceDetails.Tag != null && deviceDetails.Tag.Stream != null ? deviceDetails.Tag.Stream.StreamName : "";
                deviceInfo.EquipmentCode = deviceDetails.Tag != null && deviceDetails.Tag.EquipmentCode != null ? deviceDetails.Tag.EquipmentCode.Code : "";
                deviceInfo.SequenceNumber = deviceDetails.Tag != null && deviceDetails.Tag.SequenceNumber != null ? deviceDetails.Tag.SequenceNumber : "";
                deviceInfo.EquipmentIdentifier = deviceDetails.Tag != null && deviceDetails.Tag.EquipmentIdentifier != null ? deviceDetails.Tag.EquipmentIdentifier : "";
                deviceInfo.Area = deviceDetails.ServiceZone != null ? deviceDetails.ServiceZone.Area : 0;
                deviceInfo.SkidTag = deviceDetails.SkidTag != null ? deviceDetails.SkidTag.TagName : "";
                deviceInfo.StandTag = deviceDetails.StandTag != null ? deviceDetails.StandTag.TagName : "";
                deviceInfo.JunctionBoxTag = deviceDetails.JunctionBoxTag != null ? deviceDetails.JunctionBoxTag.TagName : "";
                deviceInfo.Manufacturer = deviceDetails.DeviceModel != null && deviceDetails.DeviceModel.Manufacturer != null ? deviceDetails.DeviceModel.Manufacturer.Name : "";
                deviceInfo.DeviceModel = deviceDetails.DeviceModel != null ? deviceDetails.DeviceModel.Model : "";
                deviceInfo.NatureOfSignal = deviceDetails.NatureOfSignal != null ? deviceDetails.NatureOfSignal.NatureOfSignalName : "";
                deviceInfo.FailState = deviceDetails.FailState != null ? deviceDetails.FailState.FailStateName : "";
                deviceInfo.PanelTag = deviceDetails.PanelTag != null ? deviceDetails.PanelTag.TagName : "";
                deviceInfo.ServiceZone = deviceDetails.ServiceZone != null ? deviceDetails.ServiceZone.Zone : "";
                deviceInfo.ServiceBank = deviceDetails.ServiceBank != null ? deviceDetails.ServiceBank.Bank : "";
                deviceInfo.ServiceTrain = deviceDetails.ServiceTrain != null ? deviceDetails.ServiceTrain.Train : "";
                deviceInfo.SubSystem = deviceDetails.SubSystem != null ? deviceDetails.SubSystem.Number : "";

                if (deviceDetails.SubSystem != null)
                {
                    deviceInfo.System = deviceDetails.SubSystem.System != null ? deviceDetails.SubSystem.System.Number : null;
                    if (deviceDetails.SubSystem.System != null)
                    {
                        deviceInfo.WorkAreaPack = deviceDetails.SubSystem.System.WorkAreaPack.Number;
                    }
                }
                //ConnectionParentTag
                Guid? connectionParentTagId = null;
                ControlSystemHierarchy? originalConnectionParent = await _controlSystemHierarchyService.GetAll(x => x.Instrument == false && x.ChildDeviceId == id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                connectionParentTagId = originalConnectionParent != null && originalConnectionParent.ParentDevice != null ? originalConnectionParent.ParentDevice.TagId : null;
                deviceInfo.ConnectionParentTag = originalConnectionParent != null && originalConnectionParent.ParentDevice != null ? originalConnectionParent.ParentDevice.Tag.TagName : null;

                //InstrumentParentTag
                ControlSystemHierarchy? originalInstrumentParent = await _controlSystemHierarchyService.GetAll(x => x.Instrument == true && x.ChildDeviceId == id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.InstrumentParentTag = originalInstrumentParent != null && originalInstrumentParent.ParentDevice != null ? originalInstrumentParent.ParentDevice.Tag.TagName : null;

                //Origin CableTag
                Guid? connectionCableTagId = null;
                CableHierarchy? originalConnectionCable = await _cableHierarchyService.GetAll(x => x.Instrument == false && x.DestinationDeviceId == id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                connectionCableTagId = originalConnectionCable != null && originalConnectionCable.OriginDevice != null ? originalConnectionCable.OriginDevice.TagId : null;
                deviceInfo.OriginCableTag = originalConnectionCable != null && originalConnectionCable.OriginDevice != null ? originalConnectionCable.OriginDevice.Tag.TagName : null;

                //Destination CableTag
                CableHierarchy? originalInstrumentCable = await _cableHierarchyService.GetAll(x => x.Instrument == true && x.DestinationDeviceId == id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                deviceInfo.DestinationCableTag = originalInstrumentCable != null && originalInstrumentCable.OriginDevice != null ? originalInstrumentCable.OriginDevice.Tag.TagName : null;

                //Attributes
                List<AttributeDefinitionIdsDto> attributeDefinitions = await GetAttributeDefinitions(deviceDetails.DeviceTypeId, deviceDetails.DeviceModelId, deviceDetails.NatureOfSignalId, connectionParentTagId, deviceDetails.Tag.ProjectId);
                List<AttributeValueDto> attributes = new List<AttributeValueDto>();
                foreach (var item in attributeDefinitions)
                {
                    var attributeValue = (deviceDetails != null) ?
                                         await _attributeValueService.GetAll(a => a.IsActive && !a.IsDeleted && a.DeviceId == id && a.AttributeDefinitionId == item.AttributeDefinitionId).FirstOrDefaultAsync() : null;
                    var attributeDefinitionInfo = await _attributeDefinitionService.GetAll(a => a.IsActive && !a.IsDeleted && a.Id == item.AttributeDefinitionId).FirstOrDefaultAsync();
                    if (attributeDefinitionInfo != null)
                    {
                        var info = new AttributeValueDto()
                        {
                            Id = attributeDefinitionInfo.Id,
                            Name = attributeDefinitionInfo.Name,
                            ValueType = attributeDefinitionInfo.ValueType,
                            Required = attributeDefinitionInfo.Required,
                            Value = attributeValue != null ? attributeValue.Value : ""
                        };
                        attributes.Add(info);
                    }
                }
                deviceInfo.Attributes = attributes;

                //ReferenceDocuments
                List<ReferenceDocumentDevice> referenceDocuments = await _referenceDocumentDeviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceId == id).ToListAsync();
                deviceInfo.ReferenceDocumentInfo = _mapper.Map<List<ReferenceDocumentInfoDto>>(referenceDocuments.Select(s => s.ReferenceDocument).ToList());
                foreach (var item in deviceInfo.ReferenceDocumentInfo)
                {
                    item.ReferenceDocumentType = referenceDocuments.Where(s => s.ReferenceDocumentId == item.Id).FirstOrDefault()?.ReferenceDocument?.ReferenceDocumentType?.Type ?? "";
                }
                return deviceInfo;
            }
            return null;
        }

        [HttpPost]
        public async Task<BaseResponse> CreateOrEditDevice(CreateOrEditDeviceDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateDevice(info);
            }
            else
            {
                return await EditDevice(info);
            }
        }

        [HttpPost]
        public async Task<BaseResponse> CreateDevice(CreateOrEditDeviceDto model)
        {
            if (ModelState.IsValid)
            {
                Device isExist = await _deviceService.GetSingleAsync(x => x.TagId == model.TagId && x.IsActive && !x.IsDeleted);
                if (isExist != null)
                    return new BaseResponse(false, ResponseMessages.TagNameAlreadyTaken.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (model.ConnectionParentTagId != null && model.InstrumentParentTagId != null && model.ConnectionParentTagId == model.InstrumentParentTagId)
                    return new BaseResponse(false, ResponseMessages.ParentDeviceNotSame, HttpStatusCode.NoContent);

                Device modelInfo = _mapper.Map<Device>(model);
                Device response = await _deviceService.AddAsync(modelInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(true, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (model.ConnectionParentTagId != null && model.ConnectionParentTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.ConnectionParentTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice != null)
                        await AddControlSystemHierarchy(response.Id, parentDevice.Id, false);
                }


                if (model.InstrumentParentTagId != null && model.InstrumentParentTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.InstrumentParentTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice != null)
                        await AddControlSystemHierarchy(response.Id, parentDevice.Id, true);
                }

                if (model.ConnectionCableTagId != null && model.ConnectionCableTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.ConnectionCableTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice != null)
                        await AddCableSystemHierarchy(response.Id, parentDevice.Id, false);
                }


                if (model.InstrumentCableTagId != null && model.InstrumentCableTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.InstrumentCableTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice != null)
                        await AddCableSystemHierarchy(response.Id, parentDevice.Id, true);
                }


                if (model.ReferenceDocumentIds != null && model.ReferenceDocumentIds.Count() > 0)
                {
                    foreach (var referenceDocumentId in model.ReferenceDocumentIds)
                    {
                        ReferenceDocumentDevice documentDevice = new()
                        {
                            DeviceId = response.Id,
                            ReferenceDocumentId = referenceDocumentId
                        };
                        await _referenceDocumentDeviceService.AddAsync(documentDevice, User.GetUserId());
                    }
                }
                AddUpdateDeleteAttributeInfoDto attributeData = await UpdateAttributes(model.Attributes, response);

                await _changeLogHelper.CreateDeviceChangeLog(new Device(), model, null, null, model.ReferenceDocumentIds, null, attributeData);
                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public async Task<BaseResponse> EditDevice(CreateOrEditDeviceDto model)
        {
            if (ModelState.IsValid)
            {
                Device deviceDetails = await _deviceService.GetSingleAsync(s => s.Id == model.Id && !s.IsDeleted);
                if (deviceDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Device isExist = await _deviceService.GetSingleAsync(x => x.Id != model.Id && x.TagId == model.TagId && x.IsActive && !x.IsDeleted);
                if (isExist != null)
                    return new BaseResponse(false, ResponseMessages.TagNameAlreadyTaken.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (model.ConnectionParentTagId != null && model.ConnectionParentTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.ConnectionParentTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice == null)
                        return new BaseResponse(false, ResponseMessages.EnterValidModule.ToString().Replace("{module}", "connection parent tag"), HttpStatusCode.BadRequest);
                    else
                        await AddControlSystemHierarchy(model.Id, parentDevice.Id, false);

                }
                else
                {
                    var chkConnectionExist = await _controlSystemHierarchyService.GetAll(s => s.ChildDeviceId == model.Id && s.Instrument == false && s.IsActive && !s.IsDeleted).ToListAsync();
                    foreach (var item in chkConnectionExist)
                    {
                        item.IsDeleted = true;
                        _controlSystemHierarchyService.Delete(item);
                    }

                }

                if (model.InstrumentParentTagId != null && model.InstrumentParentTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.InstrumentParentTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice == null)
                        return new BaseResponse(false, ResponseMessages.EnterValidModule.ToString().Replace("{module}", "instrument parent tag"), HttpStatusCode.BadRequest);
                    else
                        await AddControlSystemHierarchy(model.Id, parentDevice.Id, true);
                }
                else
                {
                    var chkParentExist = await _controlSystemHierarchyService.GetAll(s => s.ChildDeviceId == model.Id && s.Instrument == true && s.IsActive && !s.IsDeleted).ToListAsync();
                    foreach (var item in chkParentExist)
                    {
                        item.IsDeleted = true;
                        _controlSystemHierarchyService.Delete(item);
                    }
                }

                if (model.ConnectionCableTagId != null && model.ConnectionCableTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.ConnectionCableTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice == null)
                        return new BaseResponse(false, ResponseMessages.EnterValidModule.ToString().Replace("{module}", "connection cable tag"), HttpStatusCode.BadRequest);
                    else
                        await AddCableSystemHierarchy(model.Id, parentDevice.Id, false);
                }
                else
                {
                    var chkConnectionExist = await _cableHierarchyService.GetAll(s => s.DestinationDeviceId == model.Id && s.Instrument == false && s.IsActive && !s.IsDeleted).ToListAsync();
                    foreach (var item in chkConnectionExist)
                    {
                        item.IsDeleted = true;
                        _cableHierarchyService.Delete(item);
                    }
                }

                if (model.InstrumentCableTagId != null && model.InstrumentCableTagId != Guid.Empty)
                {
                    Device? parentDevice = await _deviceService.GetAll(x => x.Tag.ProjectId == model.ProjectId && x.TagId == model.InstrumentCableTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    if (parentDevice == null)
                        return new BaseResponse(false, ResponseMessages.EnterValidModule.ToString().Replace("{module}", "instrument parent tag"), HttpStatusCode.BadRequest);
                    else
                        await AddCableSystemHierarchy(model.Id, parentDevice.Id, true);
                }
                else
                {
                    var chkParentExist = await _cableHierarchyService.GetAll(s => s.DestinationDeviceId == model.Id && s.Instrument == true && s.IsActive && !s.IsDeleted).ToListAsync();
                    foreach (var item in chkParentExist)
                    {
                        item.IsDeleted = true;
                        _cableHierarchyService.Delete(item);
                    }
                }

                Device modelInfo = _mapper.Map<Device>(model);
                modelInfo.CreatedBy = deviceDetails.CreatedBy;
                modelInfo.CreatedDate = deviceDetails.CreatedDate;
                modelInfo.IsActive = deviceDetails.IsActive;
                var response = _deviceService.Update(modelInfo, deviceDetails, User.GetUserId());

                List<ReferenceDocumentDevice> removeDeviceInfo = await _referenceDocumentDeviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceId == model.Id &&
               !model.ReferenceDocumentIds.Select(a => a).Contains(s.ReferenceDocumentId)).ToListAsync();
                if (removeDeviceInfo.Count() != 0)
                {
                    foreach (var item in removeDeviceInfo)
                    {
                        item.IsDeleted = true;
                        _referenceDocumentDeviceService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                List<Guid>? newRefDocId = new List<Guid>();
                if (model.ReferenceDocumentIds != null && model.ReferenceDocumentIds.Count() > 0)
                {
                    foreach (var referenceDocumentId in model.ReferenceDocumentIds)
                    {
                        ReferenceDocumentDevice? isRefDeviceExist = await _referenceDocumentDeviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceId == model.Id
                        && s.ReferenceDocumentId == referenceDocumentId).FirstOrDefaultAsync();
                        if (isRefDeviceExist == null)
                        {
                            ReferenceDocumentDevice documentDevice = new()
                            {
                                DeviceId = response.Id,
                                ReferenceDocumentId = referenceDocumentId
                            };
                            await _referenceDocumentDeviceService.AddAsync(documentDevice, User.GetUserId());
                            newRefDocId.Add(referenceDocumentId);
                        }
                    }
                }

                AddUpdateDeleteAttributeInfoDto attributeData = await UpdateAttributes(model.Attributes, response);

                if (response == null)
                    return new BaseResponse(true, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);


                await _changeLogHelper.CreateDeviceChangeLog(deviceDetails, model, null, null, newRefDocId, removeDeviceInfo.Select(a => a.ReferenceDocumentId).ToList(), attributeData);
                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public async Task<List<AttributeValueDto>> GetAttributes(DeviceAttributeInfoDto info)
        {
            List<AttributeDefinitionIdsDto> attributeDefinitions = await GetAttributeDefinitions(info.DeviceTypeId, info.DeviceModelId, info.NatureOfSignalId, info.ConnectionParentTagId, info.ProjectId);
            List<AttributeValueDto> attributes = new List<AttributeValueDto>();
            foreach (var item in attributeDefinitions)
            {
                var attributeValue = info.DeviceId != null && info.DeviceId != Guid.Empty ? await _attributeValueService.GetAll(a => a.IsActive && !a.IsDeleted && a.DeviceId == info.DeviceId && a.AttributeDefinitionId == item.AttributeDefinitionId).FirstOrDefaultAsync() : null;
                var attributeDefinitionInfo = await _attributeDefinitionService.GetAll(a => a.IsActive && !a.IsDeleted && a.Id == item.AttributeDefinitionId).FirstOrDefaultAsync();
                if (attributeDefinitionInfo != null)
                {
                    attributes.Add(new AttributeValueDto()
                    {
                        Id = attributeDefinitionInfo.Id,
                        Name = attributeDefinitionInfo.Name,
                        ValueType = attributeDefinitionInfo.ValueType,
                        Required = attributeDefinitionInfo.Required,
                        Value = attributeValue != null ? attributeValue.Value : ""
                    });
                }
            }
            return attributes;
        }
        #endregion

        [HttpGet]
        public async Task<BaseResponse> DeleteDevice(Guid id)
        {
            Device? deviceDetails = await _deviceService.GetAll(s => s.Id == id).FirstOrDefaultAsync();
            if (deviceDetails != null)
            {
                bool allowed = true;
                Device oldDeviceDetails = deviceDetails;

                List<ControlSystemHierarchy> childDevices = await _controlSystemHierarchyService.GetAll(s => s.ParentDeviceId == id && s.IsActive && !s.IsDeleted).ToListAsync();
                List<ControlSystemHierarchy> parentDevices = await _controlSystemHierarchyService.GetAll(s => s.ChildDeviceId == id && s.IsActive && !s.IsDeleted).ToListAsync();
                if (childDevices.Count > 0 && !deviceDetails.IsDeleted)
                {
                    foreach (var item in childDevices)
                    {
                        if (!item.ChildDevice.IsActive)
                        {
                            allowed = false;
                            return new BaseResponse(false, ResponseMessages.DeviceDeactived, HttpStatusCode.InternalServerError, deviceDetails);
                        }
                    }
                }

                if (allowed)
                {
                    deviceDetails.IsDeleted = true;
                    var response = _deviceService.Update(deviceDetails, oldDeviceDetails, User.GetUserId(), true, true);

                    if (response == null)
                        return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, deviceDetails);

                    await _changeLogHelper.CreateActivationChangeLog(true, deviceDetails?.Tag?.TagName ?? "", "Device", ChangeLogOptions.Deleted);
                    return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, deviceDetails);
                }
                return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, deviceDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public async Task<BaseResponse> ActiveInActiveDevice(ActiveInActiveDto info)
        {
            Guid userId = User.GetUserId();
            Device? deviceDetails = await _deviceService.GetAll(s => s.Id == info.Id).FirstOrDefaultAsync();
            if (deviceDetails != null)
            {
                bool allowed = true;
                Device oldDeviceDetails = deviceDetails;

                List<ControlSystemHierarchy> childDevices = await _controlSystemHierarchyService.GetAll(s => s.ParentDeviceId == info.Id && !s.IsDeleted).ToListAsync();
                List<ControlSystemHierarchy> parentDevices = await _controlSystemHierarchyService.GetAll(s => s.ChildDeviceId == info.Id && !s.IsDeleted).ToListAsync();
                if (childDevices.Count > 0 && !info.IsActive)
                {
                    foreach (var item in childDevices)
                    {
                        if (item.ChildDevice.IsActive)
                        {
                            allowed = false;
                            return new BaseResponse(false, ResponseMessages.DeviceDeactived, HttpStatusCode.InternalServerError);
                        }
                    }
                }
                else if (parentDevices.Count > 0 && info.IsActive)
                {
                    foreach (var item in parentDevices)
                        if (!item.ParentDevice.IsActive)
                        {
                            allowed = false;
                            return new BaseResponse(false, ResponseMessages.DeviceActivated, HttpStatusCode.InternalServerError);
                        }
                }
                if (allowed)
                {
                    deviceDetails.IsActive = info.IsActive;
                    var response = _deviceService.Update(deviceDetails, oldDeviceDetails, userId);
                    if (parentDevices != null && parentDevices.Any())
                    {
                        foreach (var item in parentDevices)
                        {
                            item.IsActive = info.IsActive;
                            _controlSystemHierarchyService.Update(item, item, userId);
                        }
                    }

                    if (response == null)
                        return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                    await _changeLogHelper.CreateActivationChangeLog(info.IsActive, deviceDetails?.Tag?.TagName ?? "", "Device", ChangeLogOptions.ActiveDeactive);
                    return new BaseResponse(true, info.IsActive ? ResponseMessages.ModuleActivate.ToString().Replace("{module}", ModuleName) : ResponseMessages.ModuleDeactivate.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK);
                }
                else
                {
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        public async Task<BaseResponse> DeleteBulkInstrumentDevices(List<Guid> deviceIds)
        {
            return await DeleteBulkDevices($"{ModuleName} - Instrument", deviceIds);
        }

        [HttpDelete]
        public async Task<BaseResponse> DeleteBulkNonInstrumentDevices(List<Guid> deviceIds)
        {
            return await DeleteBulkDevices($"{ModuleName} - NonInstrument", deviceIds);
        }

        private async Task<BaseResponse> DeleteBulkDevices(string moduleName, List<Guid> deviceIds)
        {
            try
            {
                if (deviceIds == null || deviceIds.Count == 0)
                {
                    return new BaseResponse(false, "Empty record was provided.", HttpStatusCode.BadRequest);
                }

                List<BaseResponse> result = [];
                List<BulkDeleteLogDto> bulkLog = [];
                foreach (var deviceId in deviceIds)
                {
                    var deleteResponse = await DeleteDevice(deviceId);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as Device;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Tag?.TagName,
                            Status = deleteResponse.IsSucceeded,
                            Message = deleteResponse.Message,
                        });
                    }
                    result.Add(deleteResponse);
                }

                // Record logs
                await _changeLogHelper.CreateBulkDeleteLog(moduleName, bulkLog);

                if (result.Count != 0 && result.All(r => !r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = false,
                        Message = $"Failed to delete devices.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted devices. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of devices have not been successfully deleted. \n" +
                    $"Success: {result.Where(r => r.IsSucceeded).Count()} \n" +
                    $"Failed: {result.Where(r => !r.IsSucceeded).Count()} \n" +
                    $"Please check logs for more details.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse(false, "Unexpected error occured. Please try again.", HttpStatusCode.BadRequest);
            }
        }

        private async Task<bool> AddControlSystemHierarchy(Guid deviceId, Guid parentId, bool isInstrument)
        {
            try
            {

                bool isSuccess = false;
                ControlSystemHierarchy? link = await _controlSystemHierarchyService.GetSingleAsync(x => x.Instrument == isInstrument && x.ChildDeviceId == deviceId && !x.IsDeleted);

                if (link != null && link.ParentDeviceId != parentId)
                {
                    link.IsDeleted = true;
                    var response = _controlSystemHierarchyService.Update(link, link, User.GetUserId(), true, true);
                    link = null;
                }

                if (link == null && parentId != Guid.Empty)
                {
                    link = new ControlSystemHierarchy()
                    {
                        Instrument = isInstrument,
                        ParentDeviceId = parentId,
                        ChildDeviceId = deviceId                        
                    };
                    await _controlSystemHierarchyService.AddAsync(link, User.GetUserId());
                    return isSuccess = true;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> AddCableSystemHierarchy(Guid deviceId, Guid parentId, bool isInstrument)
        {
            try
            {
                bool isSuccess = false;
                CableHierarchy? link = await _cableHierarchyService.GetSingleAsync(x => x.Instrument == isInstrument && x.DestinationDeviceId == deviceId && !x.IsDeleted);

                if (link != null && link.OriginDeviceId != parentId)
                {
                    link.IsDeleted = true;
                    var response = _cableHierarchyService.Update(link, link, User.GetUserId(), true, true);
                    link = null;
                }

                if (link == null && parentId != Guid.Empty)
                {
                    link = new CableHierarchy()
                    {
                        Instrument = isInstrument,
                        OriginDeviceId = parentId,
                        DestinationDeviceId = deviceId
                    };
                    await _cableHierarchyService.AddAsync(link, User.GetUserId());
                    return isSuccess = true;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<AddUpdateDeleteAttributeInfoDto> UpdateAttributes(List<AttributeValueDto>? attributes, Device deviceInfo)
        {
            AddUpdateDeleteAttributeInfoDto attributeData = new AddUpdateDeleteAttributeInfoDto();
            List<AttributeValue> removeValue = await _attributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceId == deviceInfo.Id &&
              attributes != null && !attributes.Select(a => a.Id).Contains(s.AttributeDefinitionId)).ToListAsync();
            if (removeValue.Count() != 0)
            {
                foreach (var item in removeValue)
                {
                    DeviceAttributeValue? deviceAttributeValueInfo = await _deviceAttributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.AttributeValueId == item.Id
                                                          && s.DeviceId == deviceInfo.Id).FirstOrDefaultAsync();
                    if (deviceAttributeValueInfo != null)
                    {
                        deviceAttributeValueInfo.IsDeleted = true;
                        _deviceAttributeValueService.Update(deviceAttributeValueInfo, deviceAttributeValueInfo, User.GetUserId(), true, true);
                    }
                    item.IsDeleted = true;
                    _attributeValueService.Update(item, item, User.GetUserId(), true, true);
                    attributeData.DeletedAttributes.Add(new AttributeDetailsChangeLogDto
                    {
                        Name = item.AttributeDefinition != null ? item.AttributeDefinition.Name : "",
                        OriginalValue = item.Value ?? ""
                    });
                }
            }
            if (attributes != null)
            {
                foreach (var item in attributes)
                {
                    AttributeValue? valueInfo = await _attributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.AttributeDefinitionId == item.Id && s.DeviceId == deviceInfo.Id).FirstOrDefaultAsync();
                    AttributeDefinition? definitionInfo = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == item.Id).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        if (valueInfo != null)
                        {
                            attributeData.ModifiedAttributes.Add(new AttributeDetailsChangeLogDto
                            {
                                Name = definitionInfo?.Name,
                                Value = item.Value ?? "",
                                OriginalValue = valueInfo.Value ?? ""
                            });
                            AttributeValue attributeValueInfo = _mapper.Map<AttributeValue>(valueInfo);
                            attributeValueInfo.CreatedBy = valueInfo.CreatedBy;
                            attributeValueInfo.CreatedDate = valueInfo.CreatedDate;
                            attributeValueInfo.IsActive = valueInfo.IsActive;
                            attributeValueInfo.Value = item.Value;
                            attributeValueInfo.DeviceId = deviceInfo.Id;
                            attributeValueInfo.AttributeDefinitionId = item.Id;
                            _attributeValueService.Update(attributeValueInfo, _mapper.Map<AttributeValue>(valueInfo), User.GetUserId());
                        }
                        else
                        {
                            AttributeValue attrValueInfo = _mapper.Map<AttributeValue>(item);
                            attrValueInfo.DeviceId = deviceInfo.Id;
                            attrValueInfo.AttributeDefinitionId = item.Id;
                            attrValueInfo.Id = Guid.Empty;
                            await _attributeValueService.AddAsync(attrValueInfo, User.GetUserId());

                            DeviceAttributeValue deviceAttributeValue = new DeviceAttributeValue()
                            {
                                DeviceId = deviceInfo.Id,
                                AttributeValueId = attrValueInfo.Id
                            };
                            await _deviceAttributeValueService.AddAsync(deviceAttributeValue, User.GetUserId());
                            attributeData.NewAttributes.Add(new AttributeDetailsChangeLogDto
                            {
                                Name = definitionInfo?.Name,
                                Value = item.Value ?? ""
                            });
                        }
                    }
                    else if (valueInfo != null)
                    {
                        DeviceAttributeValue? deviceAttributeValueInfo = await _deviceAttributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.AttributeValueId == valueInfo.Id
                        && s.DeviceId == deviceInfo.Id).FirstOrDefaultAsync();
                        if (deviceAttributeValueInfo != null)
                        {
                            deviceAttributeValueInfo.IsDeleted = true;
                            _deviceAttributeValueService.Update(deviceAttributeValueInfo, deviceAttributeValueInfo, User.GetUserId(), true, true);
                        }
                        valueInfo.IsDeleted = true;
                        _attributeValueService.Update(valueInfo, valueInfo, User.GetUserId(), true, true);
                    }
                }
            }

            return attributeData;
        }

        private async Task<List<AttributeDefinitionIdsDto>> GetAttributeDefinitions(Guid? deviceTypeId, Guid? deviceModelId, Guid? natureOfSignalId, Guid? connectionParentTagId, Guid? projectId)
        {
            List<AttributeDefinitionIdsDto> attributeDefinitionIds = new List<AttributeDefinitionIdsDto>();
            Device? connectionParent = await _deviceService.GetAll(x => x.Tag.ProjectId == projectId && x.TagId == connectionParentTagId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
            if ((deviceTypeId != Guid.Empty && deviceTypeId != null) || (deviceModelId != null && deviceModelId != Guid.Empty) || (natureOfSignalId != null && natureOfSignalId != Guid.Empty))
            {
                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("_DeviceTypeId", NpgsqlDbType.Uuid) { Value = deviceTypeId ?? Guid.Empty },
                    new NpgsqlParameter("_DeviceModelId", NpgsqlDbType.Uuid) { Value = deviceModelId ?? Guid.Empty },
                    new NpgsqlParameter("_NatureOfSignalId", NpgsqlDbType.Uuid) { Value = natureOfSignalId ?? Guid.Empty }
                };
                attributeDefinitionIds.AddRange(await _storedProcedureHelper.GetAttributeDefinitionIds(@"public.""spGetDeviceTypeModelNatureOfSignalAttributeDefinitionIDs""", parameters));
            }

            if (connectionParent != null)
            {
                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("_DeviceId", NpgsqlDbType.Uuid) { Value = connectionParent != null ? connectionParent.Id : Guid.Empty },
                    new NpgsqlParameter("_ParentsOnly", NpgsqlDbType.Boolean) { Value = false },
                };
                attributeDefinitionIds.AddRange(await _storedProcedureHelper.GetAttributeDefinitionIds(@"public.""spGetDeviceConfigurableAttributeDefinitionIDs""", parameters));
            }
            attributeDefinitionIds = attributeDefinitionIds.DistinctBy(s => s.AttributeDefinitionId).ToList();
            return attributeDefinitionIds;
        }
    }
}
