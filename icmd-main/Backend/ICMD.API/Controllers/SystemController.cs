using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.System;
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
    public class SystemController : BaseController
    {
        private readonly ISystemService _systemService;
        private readonly ISubSystemService _subSystemService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly IMapper _mapper;
        private static string ModuleName = "System";
        private readonly CSVImport _csvImport;

        private readonly ChangeLogHelper _changeLogHelper;

        public SystemController(IMapper mapper, ISystemService systemService, ISubSystemService subSystemService, IWorkAreaPackService workAreaPackService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _systemService = systemService;
            _mapper = mapper;
            _subSystemService = subSystemService;
            _workAreaPackService = workAreaPackService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region System

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<SystemInfoDto>> GetAllSystems(PagedAndSortedResultRequestDto input)
        {
            IQueryable<SystemInfoDto> allSystems = _systemService.GetAll(s => !s.IsDeleted).Select(s => new SystemInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description,
                WorkAreaPackId = s.WorkAreaPackId,
                WorkAreaPack = s.WorkAreaPack != null ? s.WorkAreaPack.Number : "",
                ProjectId = s.WorkAreaPack != null ? s.WorkAreaPack.ProjectId : null,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allSystems = allSystems.Where(s => (!string.IsNullOrEmpty(s.Number) && s.Number.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.WorkAreaPack) && s.WorkAreaPack.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "workAreaPackId".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allSystems = allSystems.Where(x => ids != null && ids.Contains(x.WorkAreaPackId.ToString()));
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
            IQueryable<SystemInfoDto> paginatedData = !isExport ? allSystems.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allSystems;


            return new PagedResultDto<SystemInfoDto>(
               allSystems.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<SystemInfoDto?> GetSystemInfo(Guid id)
        {
            Core.DBModels.System? systemDetails = await _systemService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (systemDetails != null)
            {
                SystemInfoDto systemInfo = _mapper.Map<SystemInfoDto>(systemDetails);
                systemInfo.WorkAreaPack = systemDetails.WorkAreaPack != null ? systemDetails.WorkAreaPack.Number : "";
                systemInfo.ProjectId = systemDetails.WorkAreaPack != null ? systemDetails.WorkAreaPack.ProjectId : null;
                return systemInfo;
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditSystem(CreateOrEditSystemDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateSystem(info);
            }
            else
            {
                return await UpdateSystem(info);
            }
        }

        private async Task<BaseResponse> CreateSystem(CreateOrEditSystemDto info)
        {
            if (ModelState.IsValid)
            {
                Core.DBModels.System existingSystem = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == info.WorkAreaPackId && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingSystem != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                Core.DBModels.System systemInfo = _mapper.Map<Core.DBModels.System>(info);
                systemInfo.IsActive = true;
                var response = await _systemService.AddAsync(systemInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateSystem(CreateOrEditSystemDto info)
        {
            if (ModelState.IsValid)
            {
                Core.DBModels.System systemDetails = await _systemService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (systemDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Core.DBModels.System existingSystem = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == info.WorkAreaPackId && x.Id != info.Id && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingSystem != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                Core.DBModels.System systemInfo = _mapper.Map<Core.DBModels.System>(info);
                systemInfo.CreatedBy = systemDetails.CreatedBy;
                systemInfo.CreatedDate = systemDetails.CreatedDate;
                systemInfo.IsActive = systemDetails.IsActive;
                var response = _systemService.Update(systemInfo, systemDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteSystem(Guid id)
        {
            Core.DBModels.System systemDetails = await _systemService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (systemDetails != null)
            {
                bool isChkExist = _subSystemService.GetAll(s => s.IsActive && !s.IsDeleted && s.SystemId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.SystemNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError, systemDetails);

                systemDetails.IsDeleted = true;
                var response = _systemService.Update(systemDetails, systemDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, systemDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, systemDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkSystems(List<Guid> ids)
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
                    var deleteResponse = await DeleteSystem(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as Core.DBModels.System;
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
                        Message = $"Failed to delete systems.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted systems. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of systems have not been successfully deleted. \n" +
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
        public async Task<List<SystemInfoDto>> GetAllSystemInfo(Guid projectId, Guid workAreaPackId)
        {
            List<SystemInfoDto> allSysyems = await _systemService.GetAll(s => s.WorkAreaPack != null && s.WorkAreaPack.ProjectId == projectId && !s.IsDeleted).Select(s => new SystemInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description,
                ProjectId = s.WorkAreaPack != null ? s.WorkAreaPack.ProjectId : null,
                WorkAreaPackId = s.WorkAreaPackId
            }).ToListAsync();

            if (workAreaPackId != Guid.Empty)
            {
                allSysyems = allSysyems.Where(s => s.WorkAreaPackId == workAreaPackId).OrderBy(s => s.Number).ToList();
            }

            return allSysyems;
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<SystemInfoDto>> ImportSystem([FromForm] FileUploadModel info)
        {
            List<SystemInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.System || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.SystemHeadings;

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

                    string? workAreaPackNumber = dictionary[requiredKeys[2]];
                    WorkAreaPack? workAreaPack = !string.IsNullOrEmpty(workAreaPackNumber) ? await _workAreaPackService.GetSingleAsync(x => x.Number == workAreaPackNumber && !x.IsDeleted && x.IsActive && x.ProjectId == info.ProjectId) : null;

                    CreateOrEditSystemDto createDto = new()
                    {
                        Number = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        WorkAreaPackId = workAreaPack?.Id ?? Guid.Empty,
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

                    if (workAreaPack == null)
                    {
                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "work area pack"));
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
                            Core.DBModels.System? existingSystem;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingSystem = await _systemService.GetSingleAsync(x => x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSystem == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), createDto, workAreaPackNumber);
                                }
                                else
                                {
                                    var existingRecordName = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == createDto.WorkAreaPackId &&
                                        x.Id != editId &&
                                        x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        importLog.Items = GetChanges(existingSystem, createDto, workAreaPackNumber);
                                    }
                                }
                            }
                            else
                            {
                                existingSystem = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == createDto.WorkAreaPackId && x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                Core.DBModels.System model = _mapper.Map<Core.DBModels.System>(createDto);
                                
                                if (existingSystem != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingSystem.Id;
                                    model.CreatedBy = existingSystem.CreatedBy;
                                    model.CreatedDate = existingSystem.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingSystem, createDto, workAreaPackNumber);

                                    var response = _systemService.Update(model, existingSystem, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(model, createDto, workAreaPackNumber);

                                    var response = await _systemService.AddAsync(model, User.GetUserId());
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
                        importLog.Items = GetChanges(new(), createDto, workAreaPackNumber);
                    }

                    SystemInfoDto record = _mapper.Map<SystemInfoDto>(createDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
                    record.WorkAreaPack = workAreaPackNumber;
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportSystem([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.System || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.SystemHeadings;
            var transaction = await _systemService.BeginTransaction();

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

                    string? workAreaPackNumber = dictionary[requiredKeys[2]];
                    WorkAreaPack? workAreaPack = !string.IsNullOrEmpty(workAreaPackNumber) ? await _workAreaPackService.GetSingleAsync(x => x.Number == workAreaPackNumber && !x.IsDeleted && x.IsActive && x.ProjectId == info.ProjectId) : null;

                    CreateOrEditSystemDto createDto = new()
                    {
                        Number = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        WorkAreaPackId = workAreaPack?.Id ?? Guid.Empty,
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

                    if (workAreaPack == null)
                    {
                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "work area pack"));
                        if (isSuccess) isSuccess = false;

                        validationData.Changes = GetChanges(new(), createDto, workAreaPackNumber);
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
                            Core.DBModels.System? existingSystem;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingSystem = await _systemService.GetSingleAsync(x => x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSystem == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), createDto, workAreaPackNumber);
                                }
                                else
                                {
                                    var existingRecordName = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == createDto.WorkAreaPackId &&
                                        x.Id != editId &&
                                        x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        validationData.Changes = GetChanges(existingSystem, createDto, workAreaPackNumber);
                                    }
                                }
                            }
                            else
                            {
                                existingSystem = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == createDto.WorkAreaPackId && x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                Core.DBModels.System model = _mapper.Map<Core.DBModels.System>(createDto);

                                if (existingSystem != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingSystem.Id;
                                    model.CreatedBy = existingSystem.CreatedBy;
                                    model.CreatedDate = existingSystem.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingSystem, createDto, workAreaPackNumber);

                                    var response = _systemService.Update(model, existingSystem, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(model, createDto, workAreaPackNumber);

                                    var response = await _systemService.AddAsync(model, User.GetUserId());
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
            await _systemService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(Core.DBModels.System entity, CreateOrEditSystemDto createDto, string newWorkAreaPackNumber)
        {
            var changes = new List<ChangesDto>
            {
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
                new() {
                    ItemColumnName = nameof(entity.WorkAreaPack),
                    NewValue = newWorkAreaPackNumber,
                    PreviousValue = entity.Id != Guid.Empty ? entity.WorkAreaPack?.Number ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }
    }
}
