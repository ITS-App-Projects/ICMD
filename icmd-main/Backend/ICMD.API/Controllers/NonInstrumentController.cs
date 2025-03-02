using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using ICMD.Core.ViewDto;
using ICMD.Repository.ViewService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class NonInstrumentController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly ViewNonInstrumentListService _viewNonInstrumentListService;
        private readonly IMapper _mapper;
        private readonly IControlSystemHierarchyService _controlSystemHierarchyService;
        private static string ModuleName = "Device";

        private readonly CSVImport _csvImport;
        private readonly StoredProcedureHelper _storedProcedureHelper;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly ITagService _tagService;
        private readonly IDeviceModelService _deviceModelService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IReferenceDocumentDeviceService _referenceDocumentDeviceService;
        private readonly INatureOfSignalService _natureOfSignalService;

        private readonly ChangeLogHelper _changeLogHelper;

        public NonInstrumentController(IDeviceService deviceService, ViewNonInstrumentListService viewNonInstrumentListService, IMapper mapper, IControlSystemHierarchyService controlSystemHierarchyService,
            CSVImport csvImport, StoredProcedureHelper storedProcedureHelper, IDeviceTypeService deviceTypeService, ITagService tagService,
            IDeviceModelService deviceModelService, IManufacturerService manufacturerService, IReferenceDocumentDeviceService referenceDocumentDeviceService,
            INatureOfSignalService natureOfSignalService, ChangeLogHelper changeLogHelper)
        {
            _deviceService = deviceService;
            _viewNonInstrumentListService = viewNonInstrumentListService;
            _mapper = mapper;
            _controlSystemHierarchyService = controlSystemHierarchyService;

            _csvImport = csvImport;
            _storedProcedureHelper = storedProcedureHelper;
            _deviceTypeService = deviceTypeService;
            _tagService = tagService;
            _deviceModelService = deviceModelService;
            _manufacturerService = manufacturerService;
            _referenceDocumentDeviceService = referenceDocumentDeviceService;
            _natureOfSignalService = natureOfSignalService;
            _changeLogHelper = changeLogHelper;
        }

        #region NonInstruments
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ViewNonInstrumentListDto>> GetAllNonInstruments(PagedAndSortedResultRequestDto input)
        {
            try
            {
                IQueryable<ViewNonInstrumentListDto> allNonInstruments = _viewNonInstrumentListService.GetAll(x => x.ProjectId == input.ProjectId && x.IsDeleted != true);

                if (!string.IsNullOrEmpty(input.Search))
                {
                    allNonInstruments = allNonInstruments.Where(s => (!string.IsNullOrEmpty(s.ProcessNo) && s.ProcessNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StreamName) && s.StreamName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentCode) && s.EquipmentCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SequenceNumber) && s.SequenceNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentIdentifier) && s.EquipmentIdentifier.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TagName) && s.TagName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DeviceType) && s.DeviceType.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ServiceDescription) && s.ServiceDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NatureOfSignal) && s.NatureOfSignal.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPNodeAddress) && s.DPNodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NoOfSlotsChannels) && s.NoOfSlotsChannels.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SlotNumber) && s.SlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ConnectionParentTag) && s.ConnectionParentTag.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCNumber) && s.PLCNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCSlotNumber) && s.PLCSlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Revision != null && s.Revision.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Location) && s.Location.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Manufacturer) && s.Manufacturer.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelDescription) && s.ModelDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelNumber) && s.ModelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitectureDrawing) && s.ArchitectureDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitectureDrawingSheet) && s.ArchitectureDrawingSheet.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RevisionChanges) && s.RevisionChanges.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubProcess) && s.SubProcess.ToLower().Contains(input.Search.ToLower())));
                }

                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0 && !string.IsNullOrEmpty(input.SearchFieldQuery))
                {
                    allNonInstruments = allNonInstruments.Where(input.SearchFieldQuery);
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
                                allNonInstruments = allNonInstruments.Where(x => x.IsActive);
                            }
                            else if (value == (int)RecordType.Inactive)
                            {
                                allNonInstruments = allNonInstruments.Where(x => !x.IsActive);
                            }
                        }
                    }
                }

                if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                    allNonInstruments = allNonInstruments.Where(input.SearchColumnFilterQuery);

                allNonInstruments = allNonInstruments.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "deviceId" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
                bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
                IQueryable<ViewNonInstrumentListDto> paginatedData = !isExport ? allNonInstruments.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allNonInstruments;

                return new PagedResultDto<ViewNonInstrumentListDto>(
                   allNonInstruments.Count(),
                    await paginatedData.ToListAsync()
               );
            }
            catch (Exception ex)
            {
                return new PagedResultDto<ViewNonInstrumentListDto>(
                   0,
                    new List<ViewNonInstrumentListDto>()
               );
            }

        }
        #endregion

        [HttpPost]
        [AuthorizePermission()]
        public async Task<ImportFileResultDto<Dictionary<string, string>>> ImportNonInstruments([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out _, true);
                if (typeHeaders != null)
                    return await ImportNonInstrumentData(typeHeaders, info.ProjectId);
                else
                    return new ImportFileResultDto<Dictionary<string, string>>() { IsSucceeded = false, Message = ResponseMessages.GlobalModelValidationMessage };
            }

            return new ImportFileResultDto<Dictionary<string, string>>() { IsSucceeded = false, Message = "File is invalid." };
        }

        private async Task<ImportFileResultDto<Dictionary<string, string>>> ImportNonInstrumentData(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            List<Dictionary<string, string>> responseList = [];
            List<ImportLogDto> importLogs = [];
            List<string> typeHeaders = [];

            var isEditImport = false;
            if (headerItems.FirstOrDefault() != null && headerItems.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                isEditImport = true;

            foreach (var columns in headerItems)
            {
                var dictionary = new Dictionary<string, string>();
                var editId = Guid.Empty;

                foreach (var item in columns)
                {
                    if (item.Key == FileHeadingConstants.IdHeading)
                    {
                        editId = Guid.Parse(item.Value);
                        continue;
                    }

                    dictionary.Add(item.Key, item.Value);
                }

                if (dictionary.Count == 1) continue;

                var errorExist = false;
                List<string> errorMessage = [];
                CreateOrEditDeviceDto deviceDto = new CreateOrEditDeviceDto();
                typeHeaders.AddRange([.. dictionary.Keys]);
                ImportLogDto importLog = new()
                {
                    Operation = OperationType.Insert
                };
                var changes = new List<ChangesDto>();

                var deviceTypeRef = dictionary["Device Type"];
                var deviceType = await _deviceTypeService.GetSingleAsync(d => d.Type == deviceTypeRef && !d.IsDeleted, true);
                if (deviceType == null && !errorExist)
                {
                    errorExist = true;
                    errorMessage.Add("Device Type is not found.");
                }
                changes.Add(new ChangesDto()
                {
                    ItemColumnName = "Device Type",
                    NewValue = deviceTypeRef,
                });

                var tagNameRef = dictionary["Tag"];
                var tag = await _tagService.GetSingleAsync(t => t.TagName == tagNameRef && t.ProjectId == projectId && !t.IsDeleted, true);
                if (tag == null && !errorExist)
                {
                    errorExist = true;
                    errorMessage.Add("Tag is not found.");
                }
                importLog.Name = tagNameRef;
                var isInstrumentRef = dictionary["Is Instrument"];
                changes.Add(new ChangesDto()
                {
                    ItemColumnName = "Is Instrument",
                    NewValue = isInstrumentRef,
                });

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

                            changes.Add(new ChangesDto()
                            {
                                ItemColumnName = "Manufacturer",
                                NewValue = manufacturerRef,
                            });
                            changes.Add(new ChangesDto()
                            {
                                ItemColumnName = "Model Number",
                                NewValue = deviceModelRef,
                            });
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

                    changes.Add(new ChangesDto()
                    {
                        ItemColumnName = "Connection Parent Tag",
                        NewValue = connectionParentTagRef,
                    });
                }

                // Optional Instrument Parent Tag
                var instrumentParentTagRef = dictionary["Instrument Parent Tag"];
                if (!string.IsNullOrEmpty(instrumentParentTagRef))
                {
                    var instrumentParentTag = await _tagService.GetSingleAsync(t => t.TagName == instrumentParentTagRef && !t.IsDeleted, true);
                    if (instrumentParentTag != null)
                        deviceDto.InstrumentParentTagId = instrumentParentTag.Id;

                    changes.Add(new ChangesDto()
                    {
                        ItemColumnName = "Instrument Parent Tag",
                        NewValue = instrumentParentTagRef,
                    });
                }

                // Optional Device Information
                deviceDto.RevisionChanges = dictionary["Revision Changes"];
                changes.Add(new ChangesDto()
                {
                    ItemColumnName = "Revision Changes",
                    NewValue = deviceDto.RevisionChanges,
                });

                deviceDto.ServiceDescription = dictionary["Service Description"];
                changes.Add(new ChangesDto()
                {
                    ItemColumnName = "Service Description",
                    NewValue = deviceDto.ServiceDescription,
                });

                var natureOfSignalRef = dictionary["Nature Of Signal"];
                if (!string.IsNullOrEmpty(natureOfSignalRef))
                {
                    var natureOfSignal = await _natureOfSignalService.GetSingleAsync(t => t.NatureOfSignalName == natureOfSignalRef && !t.IsDeleted, true);
                    if (natureOfSignal != null)
                        deviceDto.NatureOfSignalId = natureOfSignal.Id;

                    changes.Add(new ChangesDto()
                    {
                        ItemColumnName = "Nature Of Signal",
                        NewValue = natureOfSignalRef,
                    });
                }

                if (!errorExist)
                {
                    // Required Device
                    deviceDto.ProjectId = projectId;
                    deviceDto.DeviceTypeId = deviceType.Id;
                    deviceDto.TagId = tag.Id;
                    deviceDto.IsInstrument = isInstrumentRef ?? "-";

                    Device device;
                    if (isEditImport && editId != Guid.Empty)
                    {
                        importLog.Operation = OperationType.Edit;
                        device = await _deviceService.GetSingleAsync(x =>
                            x.Id == editId &&
                            !x.IsDeleted && x.IsActive);
                        if (device == null)
                        {
                            errorMessage.Add("Record is not found.");
                        }
                        else
                        {
                            var existingRecordName = await _deviceService.GetSingleAsync(x => x.TagId == tag.Id &&
                                x.Id != editId &&
                                !x.IsDeleted && x.IsActive);
                            if (existingRecordName != null)
                            {
                                errorMessage.Add("Tag is already taken.");
                            }
                        }
                    }
                    else
                    {
                        device = await _deviceService.GetSingleAsync(x => x.TagId == tag.Id && x.IsActive && !x.IsDeleted);
                    }

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

                    importLog.Items = changes;
                }

                Dictionary<string, string> records = dictionary;
                records.Add("Status", errorExist ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success);
                records.Add("Message", string.Join(", ", errorMessage));

                responseList.Add(records);

                importLog.Status = errorExist ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                importLog.Message = string.Join(", ", errorMessage);

                importLogs.Add(importLog);
            }

            // Record logs
            await _changeLogHelper.CreateImportLogs(ModuleName, importLogs);

            if (responseList.All(x => x.Where(p => p.Key == "Status")
                .All(p => p.Key == ImportFileRecordStatus.Success)))
            {
                return new()
                {
                    IsSucceeded = true,
                    Headers = typeHeaders,
                    Message = ResponseMessages.ImportFile,
                    Records = responseList
                };
            }
            else if (responseList.All(x => x.Where(p => p.Key == "Status")
                .All(p => p.Key == ImportFileRecordStatus.Fail)))
            {

                return new()
                {
                    IsSucceeded = false,
                    Headers = typeHeaders,
                    Message = ResponseMessages.FailedImportFile,
                    Records = responseList
                };
            }

            return new()
            {
                IsSucceeded = true,
                IsWarning = true,
                Headers = typeHeaders,
                Message = ResponseMessages.SomeFailedImportFile,
                Records = responseList
            };
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportNonInstruments([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                var headerItems = _csvImport.ReadFile(info.File, out _, true);
                if (headerItems != null)
                {
                    var transaction = await _deviceService.BeginTransaction();
                    var projectId = info.ProjectId;

                    var isEditImport = false;
                    if (headerItems.FirstOrDefault() != null && headerItems.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                        isEditImport = true;

                    List<ValidationDataDto> validationDataList = [];
                    List<string> typeHeaders = [];
                    foreach (var columns in headerItems)
                    {
                        var dictionary = new Dictionary<string, string>();
                        var editId = Guid.Empty;

                        foreach (var item in columns)
                        {
                            if (item.Key == FileHeadingConstants.IdHeading)
                            {
                                editId = Guid.Parse(item.Value);
                                continue;
                            }

                            dictionary.Add(item.Key, item.Value);
                        }

                        if (dictionary.Count == 1) continue;

                        var errorExist = false;
                        List<string> errorMessage = [];
                        CreateOrEditDeviceDto deviceDto = new CreateOrEditDeviceDto();
                        typeHeaders.AddRange([.. dictionary.Keys]);
                        ValidationDataDto validationData = new()
                        {
                            Operation = OperationType.Insert
                        };
                        var changes = new List<ChangesDto>();

                        var deviceTypeRef = dictionary["Device Type"];
                        var deviceType = await _deviceTypeService.GetSingleAsync(d => d.Type == deviceTypeRef && !d.IsDeleted, true);
                        if (deviceType == null && !errorExist)
                        {
                            errorExist = true;
                            errorMessage.Add("Device Type is not found.");
                        }
                        changes.Add(new ChangesDto()
                        {
                            ItemColumnName = "Device Type",
                            NewValue = deviceTypeRef,
                        });

                        var tagNameRef = dictionary["Tag"];
                        var tag = await _tagService.GetSingleAsync(t => t.TagName == tagNameRef && t.ProjectId == projectId && !t.IsDeleted, true);
                        if (tag == null && !errorExist)
                        {
                            errorExist = true;
                            errorMessage.Add("Tag is not found.");
                        }
                        validationData.Name = tagNameRef;

                        var isInstrumentRef = dictionary["Is Instrument"];
                        changes.Add(new ChangesDto()
                        {
                            ItemColumnName = "Is Instrument",
                            NewValue = isInstrumentRef,
                        });

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

                                    changes.Add(new ChangesDto()
                                    {
                                        ItemColumnName = "Manufacturer",
                                        NewValue = manufacturerRef,
                                    });
                                    changes.Add(new ChangesDto()
                                    {
                                        ItemColumnName = "Model Number",
                                        NewValue = deviceModelRef,
                                    });
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

                            changes.Add(new ChangesDto()
                            {
                                ItemColumnName = "Connection Parent Tag",
                                NewValue = connectionParentTagRef,
                            });

                        }

                        // Optional Instrument Parent Tag
                        var instrumentParentTagRef = dictionary["Instrument Parent Tag"];
                        if (!string.IsNullOrEmpty(instrumentParentTagRef))
                        {
                            var instrumentParentTag = await _tagService.GetSingleAsync(t => t.TagName == instrumentParentTagRef && !t.IsDeleted, true);
                            if (instrumentParentTag != null)
                                deviceDto.InstrumentParentTagId = instrumentParentTag.Id;

                            changes.Add(new ChangesDto()
                            {
                                ItemColumnName = "Instrument Parent Tag",
                                NewValue = instrumentParentTagRef,
                            });
                        }

                        // Optional Device Information
                        deviceDto.RevisionChanges = dictionary["Revision Changes"];
                        changes.Add(new ChangesDto()
                        {
                            ItemColumnName = "Revision Changes",
                            NewValue = deviceDto.RevisionChanges,
                        });

                        deviceDto.ServiceDescription = dictionary["Service Description"];
                        changes.Add(new ChangesDto()
                        {
                            ItemColumnName = "Service Description",
                            NewValue = deviceDto.ServiceDescription,
                        });

                        var natureOfSignalRef = dictionary["Nature Of Signal"];
                        if (!string.IsNullOrEmpty(natureOfSignalRef))
                        {
                            var natureOfSignal = await _natureOfSignalService.GetSingleAsync(t => t.NatureOfSignalName == natureOfSignalRef && !t.IsDeleted, true);
                            if (natureOfSignal != null)
                                deviceDto.NatureOfSignalId = natureOfSignal.Id;

                            changes.Add(new ChangesDto()
                            {
                                ItemColumnName = "Nature Of Signal",
                                NewValue = natureOfSignalRef,
                            });
                        }

                        if (!errorExist)
                        {
                            // Required Device
                            deviceDto.ProjectId = projectId;
                            deviceDto.DeviceTypeId = deviceType.Id;
                            deviceDto.TagId = tag.Id;
                            deviceDto.IsInstrument = isInstrumentRef ?? "-";

                            Device device;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                device = await _deviceService.GetSingleAsync(x =>
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (device == null)
                                {
                                    errorMessage.Add("Record is not found.");
                                }
                                else
                                {
                                    var existingRecordName = await _deviceService.GetSingleAsync(x => x.TagId == tag.Id &&
                                        x.Id != editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        errorMessage.Add("Tag is already taken.");
                                    }
                                }
                            }
                            else
                            {
                                device = await _deviceService.GetSingleAsync(x => x.TagId == tag.Id && x.IsActive && !x.IsDeleted);
                            }
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
                                validationData.Operation = OperationType.Edit;

                                deviceDto.Id = device.Id;
                                var result = await EditDevice(deviceDto);
                                if (!result.IsSucceeded)
                                {
                                    errorExist = true;
                                    errorMessage.Add(result.Message);
                                }
                            }

                            validationData.Changes = changes;
                        }

                        validationData.Status = errorExist ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                        validationData.Message = string.Join(", ", errorMessage);

                        validationDataList.Add(validationData);
                    }
                    await _deviceService.RollbackTransaction(transaction);

                    return new()
                    {
                        IsSucceeded = true,
                        Headers = typeHeaders,
                        Message = ResponseMessages.ImportFile,
                        Records = validationDataList
                    };
                }
                else
                    return new ImportFileResultDto<ValidationDataDto>() { IsSucceeded = false, Message = ResponseMessages.GlobalModelValidationMessage };
            }

            return new ImportFileResultDto<ValidationDataDto>() { IsSucceeded = false, Message = "File is invalid." };
        }

        private async Task<BaseResponse> CreateDevice(CreateOrEditDeviceDto model)
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
            catch (Exception ex)
            {
                return new BaseResponse(true, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
        }

        private async Task<BaseResponse> EditDevice(CreateOrEditDeviceDto model)
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
