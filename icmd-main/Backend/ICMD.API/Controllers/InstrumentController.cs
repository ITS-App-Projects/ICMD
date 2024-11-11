using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using ICMD.Repository.ViewService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class InstrumentController : BaseController
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IDeviceService _deviceService;
        private readonly IControlSystemHierarchyService _controlSystemHierarchyService;
        private readonly IMapper _mapper;
        private readonly ViewDeviceInstrumentService _viewDeviceInstrumentService;
        private readonly ViewInstrumentListLiveService _viewInstrumentListLiveService;
        private static string ModuleName = "Device";

        private readonly CSVImport _csvImport;
        private readonly StoredProcedureHelper _storedProcedureHelper;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly ITagService _tagService;
        private readonly IDeviceModelService _deviceModelService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IReferenceDocumentDeviceService _referenceDocumentDeviceService;
        private readonly IFailStateService _failStateService;
        private readonly IZoneService _zoneService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly ISystemService _systemService;
        private readonly ISubSystemService _subSystemService;
        private readonly IBankService _bankService;
        private readonly ITrainService _trainService;
        private readonly INatureOfSignalService _natureOfSignalService;

        public InstrumentController(IMapper mapper, IInstrumentService instrumentService, ViewDeviceInstrumentService viewDeviceInstrumentService, ViewInstrumentListLiveService viewInstrumentListLiveService,
            IDeviceService deviceService, IControlSystemHierarchyService controlSystemHierarchyService,
            CSVImport csvImport, StoredProcedureHelper storedProcedureHelper, IDeviceTypeService deviceTypeService, ITagService tagService,
            IDeviceModelService deviceModelService, IManufacturerService manufacturerService, IReferenceDocumentService referenceDocumentService, IReferenceDocumentDeviceService referenceDocumentDeviceService,
            IFailStateService failStateService, IZoneService zoneService, IWorkAreaPackService workAreaPackService, ISystemService systemService, ISubSystemService subSystemService, IBankService bankService,
            ITrainService trainService, INatureOfSignalService natureOfSignalService)
        {
            _mapper = mapper;
            _instrumentService = instrumentService;
            _viewDeviceInstrumentService = viewDeviceInstrumentService;
            _viewInstrumentListLiveService = viewInstrumentListLiveService;
            _deviceService = deviceService;
            _controlSystemHierarchyService = controlSystemHierarchyService;

            _csvImport = csvImport;
            _storedProcedureHelper = storedProcedureHelper;
            _deviceTypeService = deviceTypeService;
            _tagService = tagService;
            _deviceModelService = deviceModelService;
            _manufacturerService = manufacturerService;
            _referenceDocumentDeviceService = referenceDocumentDeviceService;
            _referenceDocumentService = referenceDocumentService;
            _failStateService = failStateService;
            _zoneService = zoneService;
            _workAreaPackService = workAreaPackService;
            _systemService = systemService;
            _subSystemService = subSystemService;
            _bankService = bankService;
            _trainService = trainService;
            _natureOfSignalService = natureOfSignalService;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ViewInstrumentListLiveDto>> GetAllInstruments(PagedAndSortedResultRequestDto input)
        {
            try
            {
                IQueryable<ViewInstrumentListLiveDto> allInstruments = _viewInstrumentListLiveService.GetAll(x => x.ProjectId == input.ProjectId && x.IsDeleted != true);

                if (!string.IsNullOrEmpty(input.Search))
                {
                    allInstruments = allInstruments.Where(s => (!string.IsNullOrEmpty(s.ProcessNo) && s.ProcessNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StreamName) && s.StreamName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentCode) && s.EquipmentCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentIdentifier) && s.EquipmentIdentifier.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TagName) && s.TagName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.InstrumentParentTag) && s.InstrumentParentTag.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ServiceDescription) && s.ServiceDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.LineVesselNumber) && s.LineVesselNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Plant != null && s.Plant.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (s.Area != null && s.Area.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (s.VendorSupply != null && s.VendorSupply.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SkidNumber) && s.SkidNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StandNumber) && s.StandNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Manufacturer) && s.Manufacturer.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelNumber) && s.ModelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CalibratedRangeMin) && s.CalibratedRangeMin.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CalibratedRangeMax) && s.CalibratedRangeMax.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ProcessRangeMin) && s.ProcessRangeMin.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ProcessRangeMax) && s.ProcessRangeMax.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CRUnits) && s.CRUnits.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PRUnits) && s.PRUnits.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RLPosition) && s.RLPosition.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DatasheetNumber) && s.DatasheetNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SheetNumber) && s.SheetNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.HookUpDrawing) && s.HookUpDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TerminationDiagram) && s.TerminationDiagram.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PIDNumber) && s.PIDNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.LayoutDrawing) && s.LayoutDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitecturalDrawing) && s.ArchitecturalDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.JunctionBoxNumber) && s.JunctionBoxNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NatureOfSignal) && s.NatureOfSignal.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.FailState) && s.FailState.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.GSDType) && s.GSDType.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ControlPanelNumber) && s.ControlPanelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCNumber) && s.PLCNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCSlotNumber) && s.PLCSlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SlotNo) && s.SlotNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ChannelNo) && s.ChannelNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPNodeAddress) && s.DPNodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PANodeAddress) && s.PANodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.FieldPanelNumber) && s.FieldPanelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPDPCoupler) && s.DPDPCoupler.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPPACoupler) && s.DPPACoupler.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.AFDHubNumber) && s.AFDHubNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RackNo) && s.RackNo.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Revision != null && s.Revision.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RevisionChangesOutstandingComments) && s.RevisionChangesOutstandingComments.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Zone) && s.Zone.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Bank) && s.Bank.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Service) && s.Service.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Variable) && s.Variable.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Train) && s.Train.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.WorkAreaPack) && s.WorkAreaPack.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SystemCode) && s.SystemCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubsystemCode) && s.SubsystemCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubProcess) && s.SubProcess.ToLower().Contains(input.Search.ToLower())));

                }

                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0 && !string.IsNullOrEmpty(input.SearchFieldQuery))
                {
                    allInstruments = allInstruments.Where(input.SearchFieldQuery);
                }
                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
                {
                    foreach (var item in input.CustomSearchs)
                    {
                        if (item.FieldName.ToLower() == "type".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                        {
                            int value = Convert.ToInt16(item.FieldValue);
                            if (value == (int)RecordType.Active)
                            {
                                allInstruments = allInstruments.Where(x => x.IsActive);
                            }
                            else if (value == (int)RecordType.Inactive)
                            {
                                allInstruments = allInstruments.Where(x => !x.IsActive);
                            }
                        }
                    }
                }

                if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                    allInstruments = allInstruments.Where(input.SearchColumnFilterQuery);

                allInstruments = allInstruments.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "deviceId" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
                bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
                IQueryable<ViewInstrumentListLiveDto> paginatedData = !isExport ? allInstruments.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allInstruments;

                return new PagedResultDto<ViewInstrumentListLiveDto>(
                   allInstruments.Count(),
                    await paginatedData.ToListAsync()
               );
            }
            catch (Exception ex)
            {
                return new PagedResultDto<ViewInstrumentListLiveDto>(
                   0,
                    new List<ViewInstrumentListLiveDto>()
               );
            }

        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<ImportFileResultDto<Dictionary<string, string>>> ImportInstruments([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                FileType fileType;
                var typeHeaders = _csvImport.ReadFile(info.File, out fileType, true);
                if (typeHeaders != null)
                    return await ImportInstrumentData(typeHeaders, info.ProjectId);
                else
                    return new ImportFileResultDto<Dictionary<string, string>>() { IsSucceeded = false, Message = ResponseMessages.GlobalModelValidationMessage };
            }

            return new ImportFileResultDto<Dictionary<string, string>>() { IsSucceeded = false, Message = "File is invalid."};
        }

        private async Task<ImportFileResultDto<Dictionary<string, string>>> ImportInstrumentData(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            List<Dictionary<string, string>> responseList = [];
            List<string> typeHeaders = [];
            foreach(var dictionary in headerItems)
            {
                if (dictionary.Count == 1) continue;

                var errorExist = false;
                List<string> errorMessage = [];
                CreateOrEditDeviceDto deviceDto = new CreateOrEditDeviceDto();

                typeHeaders.AddRange([.. dictionary.Keys]);

                var deviceTypeRef = dictionary["Device Type"];
                var deviceType = await _deviceTypeService.GetSingleAsync(d => d.Type == deviceTypeRef && !d.IsDeleted, true);
                if (deviceType == null && !errorExist)
                {
                    errorExist = true;
                    errorMessage.Add("Device Type is not found.");
                }

                var tagNameRef = dictionary["Tag"];
                var tag = await _tagService.GetSingleAsync(t => t.TagName == tagNameRef && t.ProjectId == projectId && !t.IsDeleted, true);
                if (tag == null && !errorExist)
                {
                    errorExist = true;
                    errorMessage.Add("Tag is not found.");
                }
                var isInstrumentRef = dictionary["Is Instrument"];

                // Optional Device Model
                var manufacturerRef = dictionary["Manufacturer"];
                var deviceModelRef = dictionary["Model Number"];
                if (!string.IsNullOrEmpty(manufacturerRef) && !string.IsNullOrEmpty(deviceModelRef))
                {
                    var manufacturer = await _manufacturerService.GetSingleAsync(m => m.Name == manufacturerRef && !m.IsDeleted, true);
                    if (manufacturer != null)
                    {
                        var deviceModel = await _deviceModelService.GetSingleAsync(d => d.ManufacturerId == manufacturer.Id && d.Model == deviceModelRef && !d.IsDeleted, true);
                        if (deviceModel != null)
                        {
                            deviceDto.ManufacturerId = manufacturer.Id;
                            deviceDto.DeviceModelId = deviceModel.Id;
                        }
                    }
                }

                // Optional Connection Parent Tag
                var connectionParentTagRef = dictionary["Connection Parent Tag"];
                if (!string.IsNullOrEmpty(connectionParentTagRef))
                {
                    var connectionParentTag = await _tagService.GetSingleAsync(t => t.TagName == connectionParentTagRef && !t.IsDeleted, true);
                    if (connectionParentTag != null)
                        deviceDto.ConnectionParentTagId = connectionParentTag.Id;
                }

                // Optional Instrument Parent Tag
                var instrumentParentTagRef = dictionary["Instrument Parent Tag"];
                if (!string.IsNullOrEmpty(instrumentParentTagRef))
                {
                    var instrumentParentTag = await _tagService.GetSingleAsync(t => t.TagName == instrumentParentTagRef && !t.IsDeleted, true);
                    if (instrumentParentTag != null)
                        deviceDto.InstrumentParentTagId = instrumentParentTag.Id;
                }

                // Optional Reference Document
                var referenceDocumentRef = dictionary["P&ID Number"];
                if (!string.IsNullOrEmpty(referenceDocumentRef))
                {
                    var referenceDocument = await _referenceDocumentService.GetFirstOrDefaultAsync(r => r.DocumentNumber == referenceDocumentRef && r.ProjectId == projectId && !r.IsDeleted, true);
                    if (referenceDocument != null)
                        deviceDto.ReferenceDocumentIds = [referenceDocument.Id];
                }

                // Optional Device Information
                deviceDto.LineVesselNumber = dictionary["Line / Vessel Number"];
                deviceDto.Variable = dictionary["Variable"];
                deviceDto.RevisionChanges = dictionary["Revision Changes / Outstanding Comments"];
                deviceDto.Service = dictionary["Service"];
                deviceDto.ServiceDescription = dictionary["Service Description"];
                var vendorSupplyExist = bool.TryParse(dictionary["Vendor Supply"], out var vendorSupply);
                if (vendorSupplyExist)
                    deviceDto.VendorSupply = vendorSupply;

                var failStateRef = dictionary["Fail State"];
                if (!string.IsNullOrEmpty(failStateRef))
                {
                    var faileState = await _failStateService.GetSingleAsync(f => f.FailStateName == failStateRef && !f.IsDeleted, true);
                    if (faileState != null)
                        deviceDto.FailStateId = faileState.Id;
                }

                var serviceZoneRef = dictionary["Zone"];
                if (!string.IsNullOrEmpty(serviceZoneRef))
                {
                    var serviceZone = await _zoneService.GetSingleAsync(z => z.Zone == serviceZoneRef && z.ProjectId == projectId && !z.IsDeleted, true);
                    if (serviceZone != null)
                        deviceDto.ServiceZoneId = serviceZone.Id;
                }

                // Work Area Pack -> System -> Sub System
                var workAreaPackRef = dictionary["Work Area Pack"];
                var systemCodeRef = dictionary["System Code"];
                var subSystemCodeRef = dictionary["Subsystem Code"];
                if (!string.IsNullOrEmpty(workAreaPackRef))
                {
                    var workAreaPack = await _workAreaPackService.GetSingleAsync(w => w.Number == workAreaPackRef && w.ProjectId == projectId && !w.IsDeleted, true);
                    if (workAreaPack != null && !string.IsNullOrEmpty(systemCodeRef))
                    {
                        deviceDto.WorkAreaPackId = workAreaPack.Id;

                        var systemCode = await _systemService.GetSingleAsync(s => s.Number == systemCodeRef && s.WorkAreaPackId == workAreaPack.Id && !s.IsDeleted, true);
                        if (systemCode != null && !string.IsNullOrEmpty(subSystemCodeRef))
                        {
                            deviceDto.SystemId = systemCode.Id;

                            var subSystemCode = await _subSystemService.GetSingleAsync(s => s.Number == subSystemCodeRef && s.SystemId == systemCode.Id && !s.IsDeleted, true);
                            if (subSystemCode != null)
                            {
                                deviceDto.SubSystemId = subSystemCode.Id;
                            }
                        }
                    }
                }

                var serviceBankRef = dictionary["Bank"];
                if (!string.IsNullOrEmpty(serviceBankRef))
                {
                    var serviceBank = await _bankService.GetSingleAsync(b => b.Bank == serviceBankRef && b.ProjectId == projectId && !b.IsDeleted, true);
                    if (serviceBank != null)
                        deviceDto.ServiceBankId = serviceBank.Id;
                }

                var serviceTrainRef = dictionary["Train"];
                if (!string.IsNullOrEmpty(serviceTrainRef))
                {
                    var serviceTrain = await _trainService.GetSingleAsync(t => t.Train == serviceTrainRef && t.ProjectId == projectId && !t.IsDeleted, true);
                    if (serviceTrain != null)
                        deviceDto.ServiceTrainId = serviceTrain.Id;
                }

                var natureOfSignalRef = dictionary["Nature Of Signal"];
                if (!string.IsNullOrEmpty(natureOfSignalRef))
                {
                    var natureOfSignal = await _natureOfSignalService.GetSingleAsync(t => t.NatureOfSignalName == natureOfSignalRef && !t.IsDeleted, true);
                    if (natureOfSignal != null)
                        deviceDto.NatureOfSignalId = natureOfSignal.Id;
                }

                // Type - TAG
                var skidTagRef = dictionary["Skid Number"];
                if (!string.IsNullOrEmpty(skidTagRef))
                {
                    var skidTag = await _tagService.GetFirstOrDefaultAsync(t => t.TagName == skidTagRef && !t.IsDeleted, true);
                    if (skidTag != null)
                    {
                        deviceDto.SkidTagId = skidTag.Id;
                    }
                }

                var junctionTagRef = dictionary["Junction Box Number"];
                if (!string.IsNullOrEmpty(junctionTagRef))
                {
                    var junctionTag = await _tagService.GetFirstOrDefaultAsync(t => t.TagName == junctionTagRef && !t.IsDeleted, true);
                    if (junctionTag != null)
                    {
                        deviceDto.JunctionBoxTagId = junctionTag.Id;
                    }
                }

                var panelTagRef = dictionary["Field Panel Number"];
                if (!string.IsNullOrEmpty(panelTagRef))
                {
                    var panelTag = await _tagService.GetFirstOrDefaultAsync(t => t.TagName == panelTagRef && !t.IsDeleted, true);
                    if (panelTag != null)
                    {
                        deviceDto.PanelTagId = panelTag.Id;
                    }
                }

                var standTagRef = dictionary["Stand Number"];
                if (!string.IsNullOrEmpty(standTagRef))
                {
                    var standTag = await _tagService.GetFirstOrDefaultAsync(t => t.TagName == standTagRef && !t.IsDeleted, true);
                    if (standTag != null)
                    {
                        deviceDto.StandTagId = standTag.Id;
                    }
                }

                if (!errorExist)
                {
                    // Required Device
                    deviceDto.ProjectId = projectId;
                    deviceDto.DeviceTypeId = deviceType.Id;
                    deviceDto.TagId = tag.Id;
                    deviceDto.IsInstrument = isInstrumentRef ?? "-";

                    Device device = await _deviceService.GetSingleAsync(x => x.TagId == tag.Id && x.IsActive && !x.IsDeleted);
                    if (device == null)
                    {
                        var result = await CreateDevice(deviceDto);
                        if (!result.IsSucceeded)
                        {
                            errorExist = true;
                            errorMessage.Add(result.Message);
                        }
                    }
                    else
                    {
                        deviceDto.Id = device.Id;
                        var result = await EditDevice(deviceDto);
                        if (!result.IsSucceeded)
                        {
                            errorExist = true;
                            errorMessage.Add(result.Message);
                        }
                    }
                }

                Dictionary<string, string> records = dictionary;
                records.Add("Status", errorExist ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success);
                records.Add("Message", string.Join(", ", errorMessage));

                responseList.Add(records);
            }

            return new()
            {
                IsSucceeded = true,
                Headers = typeHeaders,
                Message = ResponseMessages.ImportFile,
                Records = responseList
            };
        }


        public async Task<BaseResponse> CreateDevice(CreateOrEditDeviceDto model)
        {
            try
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

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            catch(Exception ex)
            {
                return new BaseResponse(true, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
        }

        public async Task<BaseResponse> EditDevice(CreateOrEditDeviceDto model)
        {
            try
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

                ControlSystemHierarchy? originalConnectionParent = await _controlSystemHierarchyService.GetAll(x => x.Instrument == false && x.ChildDeviceId == model.Id).FirstOrDefaultAsync();
                Guid? connectionParentTagId = originalConnectionParent != null && originalConnectionParent.ParentDevice != null ? originalConnectionParent.ParentDeviceId : null;

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

                if (response == null)
                    return new BaseResponse(true, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return new BaseResponse(true, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
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
    }
}
