using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.SubSystem;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using ICMD.Repository.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class SubSystemController : BaseController
    {
        private readonly ISubSystemService _subSystemService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private readonly ISystemService _systemService;
        private static string ModuleName = "Sub system";

        private readonly ChangeLogHelper _changeLogHelper;

        public SubSystemController(IMapper mapper, ISubSystemService subSystemService, IDeviceService deviceService, CSVImport csvImport, ISystemService systemService,
            ChangeLogHelper changeLogHelper)
        {
            _subSystemService = subSystemService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _systemService = systemService;
            _changeLogHelper = changeLogHelper;
        }

        #region SubSystem
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<SubSystemInfoDto>> GetAllSubSystems(PagedAndSortedResultRequestDto input)
        {
            IQueryable<SubSystemInfoDto> allSystems = _subSystemService.GetAll(s => !s.IsDeleted).Select(s => new SubSystemInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description,
                WorkAreaPack = s.System != null && s.System.WorkAreaPack != null ? s.System.WorkAreaPack.Number : null,
                System = s.System != null ? s.System.Number : "",
                SystemId = s.SystemId,
                WorkAreaPackId = s.System != null && s.System.WorkAreaPack != null ? s.System.WorkAreaPack.Id : null,
                ProjectId = s.System != null && s.System.WorkAreaPack != null ? s.System.WorkAreaPack.ProjectId : null,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allSystems = allSystems.Where(s => (!string.IsNullOrEmpty(s.Number) && s.Number.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.System) && s.System.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.WorkAreaPack) && s.WorkAreaPack.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "workAreaPackId".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allSystems = allSystems.Where(x => !string.IsNullOrEmpty(x.WorkAreaPack) && ids != null && ids.Contains(x.WorkAreaPackId.ToString()));
                    }

                    if (item.FieldName.ToLower() == "systemId".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allSystems = allSystems.Where(x => ids != null && ids.Contains(x.SystemId.ToString()));
                    }

                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allSystems = allSystems.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }
            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allSystems = allSystems.Where(input.SearchColumnFilterQuery);

            allSystems = allSystems.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<SubSystemInfoDto> paginatedData = !isExport ? allSystems.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allSystems;


            return new PagedResultDto<SubSystemInfoDto>(
               allSystems.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<SubSystemInfoDto?> GetSubSystemInfo(Guid id)
        {
            SubSystem? subSystemDetails = await _subSystemService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (subSystemDetails != null)
            {
                SubSystemInfoDto subSystemInfo = _mapper.Map<SubSystemInfoDto>(subSystemDetails);
                subSystemInfo.System = subSystemDetails.System != null ? subSystemDetails.System.Number : "";
                subSystemInfo.WorkAreaPackId = subSystemDetails.System != null && subSystemDetails.System.WorkAreaPack != null ? subSystemDetails.System.WorkAreaPack.Id : null;
                subSystemInfo.WorkAreaPack = subSystemDetails.System != null && subSystemDetails.System.WorkAreaPack != null ? subSystemDetails.System.WorkAreaPack.Number : null;
                subSystemInfo.ProjectId = subSystemDetails.System != null && subSystemDetails.System.WorkAreaPack != null ? subSystemDetails.System.WorkAreaPack.ProjectId : null;
                return subSystemInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditSubSystem(CreateOrEditSubSystemDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateSubSystem(info);
            }
            else
            {
                return await UpdateSubSystem(info);
            }
        }

        private async Task<BaseResponse> CreateSubSystem(CreateOrEditSubSystemDto info)
        {
            if (ModelState.IsValid)
            {
                SubSystem existingSubSystem = await _subSystemService.GetSingleAsync(x => x.SystemId == info.SystemId && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingSubSystem != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                SubSystem subSystemInfo = _mapper.Map<SubSystem>(info);
                subSystemInfo.IsActive = true;
                var response = await _subSystemService.AddAsync(subSystemInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateSubSystem(CreateOrEditSubSystemDto info)
        {
            if (ModelState.IsValid)
            {
                SubSystem subSystemDetails = await _subSystemService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (subSystemDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                SubSystem existingSubSystem = await _subSystemService.GetSingleAsync(x => x.SystemId == info.SystemId && x.Id != info.Id && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingSubSystem != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                SubSystem subSystemInfo = _mapper.Map<SubSystem>(info);
                subSystemInfo.CreatedBy = subSystemDetails.CreatedBy;
                subSystemInfo.CreatedDate = subSystemDetails.CreatedDate;
                subSystemInfo.IsActive = subSystemDetails.IsActive;
                var response = _subSystemService.Update(subSystemInfo, subSystemDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteSubSystem(Guid id)
        {
            SubSystem subSystemDetails = await _subSystemService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (subSystemDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.SubSystemId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, subSystemDetails);

                subSystemDetails.IsDeleted = true;
                var response = _subSystemService.Update(subSystemDetails, subSystemDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, subSystemDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, subSystemDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkSubSystems(List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return new BaseResponse(false, "Empty record was provided", HttpStatusCode.BadRequest);
                }

                List<BaseResponse> result = [];
                List<BulkDeleteLogDto> bulkLog = [];
                foreach (var id in ids)
                {
                    var deleteResponse = await DeleteSubSystem(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as SubSystem;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Number,
                            Status = deleteResponse.IsSucceeded,
                            Message = deleteResponse.Message,
                        });
                    }
                    result.Add(deleteResponse);
                }

                // Record logs
                await _changeLogHelper.CreateBulkDeleteLog(ModuleName, bulkLog);

                if (result.Count != 0 && result.All(r => !r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = false,
                        Message = $"Failed to delete sub-systems.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted sub-systems. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of sub-systems have not been successfully deleted. \n" +
                    $"Success: {result.Where(r => r.IsSucceeded).Count()} \n" +
                    $"Failed: {result.Where(r => !r.IsSucceeded).Count()} \n" +
                    $"Please check logs for more details.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse(false, "Unexpected error occured. Please try again", HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public async Task<List<SubSystemInfoDto>> GetAllSubSystemInfo(Guid systemId)
        {
            List<SubSystemInfoDto> allSubSystems = await _subSystemService.GetAll(s => s.SystemId == systemId && !s.IsDeleted).Select(s => new SubSystemInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description,
                SystemId = s.SystemId
            }).ToListAsync();

            return allSubSystems;
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<SubSystemInfoDto>> ImportSubSystem([FromForm] FileUploadModel info)
        {
            List<SubSystemInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.SubSystem || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.SubSystemHeadings;

            var isEditImport = false;
            if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                isEditImport = true;

            foreach (var columns in typeHeaders)
            {
                var dictionary = new Dictionary<string, string>();
                var editId = Guid.Empty;

                foreach (var item in columns)
                {
                    if (item.Key == FileHeadingConstants.IdHeading)
                    {
                        var isSuccess = Guid.TryParse(item.Value, out editId);
                        if (!isSuccess)
                            editId = Guid.Empty;

                        continue;
                    }

                    dictionary.Add(item.Key, item.Value);
                }

                var keys = dictionary.Keys.ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    bool isSuccess = false;
                    List<string> message = [];

                    string? systemNumber = dictionary[requiredKeys[0]];
                    Core.DBModels.System? system = !string.IsNullOrEmpty(systemNumber) ?
                        await _systemService.GetSingleAsync(x =>
                            x.Number == systemNumber &&
                            !x.IsDeleted && x.WorkAreaPack != null &&
                            x.WorkAreaPack.ProjectId == info.ProjectId &&
                            !x.WorkAreaPack.IsDeleted)
                        : null;

                    CreateOrEditSubSystemDto createDto = new()
                    {
                        Number = dictionary[requiredKeys[1]],
                        Description = dictionary[requiredKeys[2]],
                        SystemId = system?.Id ?? Guid.Empty,
                        Id = Guid.Empty
                    };
                    var importLog = new ImportLogDto
                    {
                        Name = createDto.Number,
                        Operation = OperationType.Insert,
                    };

                    CommonHelper helper = new();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                    isSuccess = validationResponse.Item1;
                    if (!isSuccess) message.AddRange(validationResponse.Item2);

                    if (system == null)
                    {
                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "system"));
                        if (isSuccess) isSuccess = false;
                    }

                    if (isEditImport && editId == Guid.Empty)
                    {
                        isSuccess = false;
                        message.Add("Id is incorrect format.");
                    }

                    if (isSuccess)
                    {
                        bool isUpdate = false;
                        try
                        {
                            SubSystem? existingSubSystem;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingSubSystem = await _subSystemService.GetSingleAsync(x => x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSubSystem == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), createDto, systemNumber);
                                }
                                else
                                {
                                    var existingRecordName = await _subSystemService.GetSingleAsync(x => x.SystemId == createDto.SystemId &&
                                        x.Id != editId &&
                                        x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        importLog.Items = GetChanges(existingSubSystem, createDto, systemNumber);
                                    }
                                }
                            }
                            else
                            {
                                existingSubSystem = await _subSystemService.GetSingleAsync(x => x.SystemId == createDto.SystemId && x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }

                            if (message.Count == 0)
                            {
                                SubSystem model = _mapper.Map<SubSystem>(createDto);
                                if (existingSubSystem != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingSubSystem.Id;
                                    model.CreatedBy = existingSubSystem.CreatedBy;
                                    model.CreatedDate = existingSubSystem.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingSubSystem, createDto, systemNumber);

                                    var response = _subSystemService.Update(model, existingSubSystem, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(model, createDto, systemNumber);

                                    var response = await _subSystemService.AddAsync(model, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName));
                                }
                            }
                        }
                        catch (Exception)
                        {
                            message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", ModuleName));
                        }
                    }
                    else
                    {
                        importLog.Items = GetChanges(new(), createDto, systemNumber);
                    }

                    SubSystemInfoDto record = _mapper.Map<SubSystemInfoDto>(createDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
                    record.System = systemNumber;
                    responseList.Add(record);

                    importLog.Status = record.Status;
                    importLog.Message = record.Message;
                    importLogs.Add(importLog);
                }
            }

            // Record logs
            await _changeLogHelper.CreateImportLogs(ModuleName, importLogs);

            if (responseList.All(x => x.Status == ImportFileRecordStatus.Success))
            {
                return new()
                {
                    IsSucceeded = true,
                    Message = ResponseMessages.ImportFile,
                    Records = responseList
                };
            }
            else if (responseList.All(x => x.Status == ImportFileRecordStatus.Fail))
            {

                return new()
                {
                    IsSucceeded = false,
                    Message = ResponseMessages.FailedImportFile,
                    Records = responseList
                };
            }

            return new()
            {
                IsSucceeded = true,
                IsWarning = true,
                Message = ResponseMessages.SomeFailedImportFile,
                Records = responseList
            };
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportSubSystem([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.SubSystem || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.SubSystemHeadings;
            var transaction = await _subSystemService.BeginTransaction();

            var isEditImport = false;
            if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                isEditImport = true;

            foreach (var columns in typeHeaders)
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

                var keys = dictionary.Keys.ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    bool isSuccess = false;
                    List<string> message = [];

                    string? systemNumber = dictionary[requiredKeys[0]];
                    Core.DBModels.System? system = !string.IsNullOrEmpty(systemNumber) ?
                        await _systemService.GetSingleAsync(x =>
                            x.Number == systemNumber &&
                            !x.IsDeleted && x.WorkAreaPack != null &&
                            x.WorkAreaPack.ProjectId == info.ProjectId &&
                            !x.WorkAreaPack.IsDeleted)
                        : null;

                    CreateOrEditSubSystemDto createDto = new()
                    {
                        Number = dictionary[requiredKeys[1]],
                        Description = dictionary[requiredKeys[2]],
                        SystemId = system?.Id ?? Guid.Empty,
                        Id = Guid.Empty
                    };
                    ValidationDataDto validationData = new()
                    {
                        Name = createDto.Number,
                        Operation = OperationType.Insert
                    };

                    CommonHelper helper = new();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                    isSuccess = validationResponse.Item1;
                    if (!isSuccess) message.AddRange(validationResponse.Item2);

                    if (system == null)
                    {
                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "system"));
                        if (isSuccess) isSuccess = false;

                        validationData.Changes = GetChanges(new(), createDto, systemNumber);
                    }

                    if (isEditImport && editId == Guid.Empty)
                    {
                        isSuccess = false;
                        message.Add("Id is incorrect format.");
                    }

                    if (isSuccess)
                    {
                        bool isUpdate = false;
                        try
                        {
                            SubSystem? existingSubSystem;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingSubSystem = await _subSystemService.GetSingleAsync(x => x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSubSystem == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), createDto, systemNumber);
                                }
                                else
                                {
                                    var existingRecordName = await _subSystemService.GetSingleAsync(x => x.SystemId == createDto.SystemId &&
                                        x.Id != editId &&
                                        x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        validationData.Changes = GetChanges(existingSubSystem, createDto, systemNumber);
                                    }
                                }
                            }
                            else
                            {
                                existingSubSystem = await _subSystemService.GetSingleAsync(x => x.SystemId == createDto.SystemId && x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                SubSystem model = _mapper.Map<SubSystem>(createDto);
                                if (existingSubSystem != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingSubSystem.Id;
                                    model.CreatedBy = existingSubSystem.CreatedBy;
                                    model.CreatedDate = existingSubSystem.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingSubSystem, createDto, systemNumber);

                                    var response = _subSystemService.Update(model, existingSubSystem, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(model, createDto, systemNumber);
                                    var response = await _subSystemService.AddAsync(model, User.GetUserId());

                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName));
                                }
                            }
                        }
                        catch (Exception)
                        {
                            message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", ModuleName));
                        }
                    }

                    validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    validationData.Message = string.Join(", ", message);
                    validationDataList.Add(validationData);
                }
            }
            await _subSystemService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(SubSystem entity, CreateOrEditSubSystemDto createDto, string newSystemNumber)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.System),
                    NewValue = newSystemNumber,
                    PreviousValue = entity.Id != Guid.Empty ? entity.System?.Number ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Number),
                    NewValue = createDto.Number,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Number : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description : string.Empty,
                },
            };
            return changes;
        }
    }
}
