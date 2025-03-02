using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.WorkAreaPack;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.API.Helpers;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.Bank;
using ICMD.Repository.Service;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class WorkAreaPackController : BaseController
    {
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly ISystemService _systemService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Work area pack";

        private readonly ChangeLogHelper _changeLogHelper;

        public WorkAreaPackController(IMapper mapper, IWorkAreaPackService workAreaPackService, ISystemService systemService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _workAreaPackService = workAreaPackService;
            _mapper = mapper;
            _systemService = systemService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region WorkAreaPack
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<WorkAreaPackInfoDto>> GetAllWorkAreaPacks(PagedAndSortedResultRequestDto input)
        {
            IQueryable<WorkAreaPackInfoDto> allWorkaAreas = _workAreaPackService.GetAll(s => !s.IsDeleted).Select(s => new WorkAreaPackInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allWorkaAreas = allWorkaAreas.Where(s => (!string.IsNullOrEmpty(s.Number) && s.Number.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allWorkaAreas = allWorkaAreas.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allWorkaAreas = allWorkaAreas.Where(input.SearchColumnFilterQuery);

            allWorkaAreas = allWorkaAreas.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<WorkAreaPackInfoDto> paginatedData = !isExport ? allWorkaAreas.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allWorkaAreas;


            return new PagedResultDto<WorkAreaPackInfoDto>(
               allWorkaAreas.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<WorkAreaPackInfoDto?> GetWorkAreaPackInfo(Guid id)
        {
            WorkAreaPack? workAreaDetails = await _workAreaPackService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (workAreaDetails != null)
            {
                return _mapper.Map<WorkAreaPackInfoDto>(workAreaDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditWorkAreaPack(CreateOrEditWorkAreaPackDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateWorkAreaPack(info);
            }
            else
            {
                return await UpdateWorkAreaPack(info);
            }
        }

        private async Task<BaseResponse> CreateWorkAreaPack(CreateOrEditWorkAreaPackDto info)
        {
            if (ModelState.IsValid)
            {
                WorkAreaPack existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingWorkArea != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                WorkAreaPack workAreaInfo = _mapper.Map<WorkAreaPack>(info);
                workAreaInfo.IsActive = true;
                var response = await _workAreaPackService.AddAsync(workAreaInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateWorkAreaPack(CreateOrEditWorkAreaPackDto info)
        {
            if (ModelState.IsValid)
            {
                WorkAreaPack workAreaDetails = await _workAreaPackService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (workAreaDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                WorkAreaPack existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.Number.ToLower().Trim() == info.Number.ToLower().Trim() && !x.IsDeleted);
                if (existingWorkArea != null)
                    return new BaseResponse(false, ResponseMessages.NumberAlreadyTaken, HttpStatusCode.Conflict);

                WorkAreaPack workAreaInfo = _mapper.Map<WorkAreaPack>(info);
                workAreaInfo.CreatedBy = workAreaDetails.CreatedBy;
                workAreaInfo.CreatedDate = workAreaDetails.CreatedDate;
                workAreaInfo.IsActive = workAreaDetails.IsActive;
                var response = _workAreaPackService.Update(workAreaInfo, workAreaDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteWorkAreaPack(Guid id)
        {
            WorkAreaPack workAreaDetails = await _workAreaPackService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (workAreaDetails != null)
            {
                bool isChkExist = _systemService.GetAll(s => s.IsActive && !s.IsDeleted && s.WorkAreaPackId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.WorkAreaPackNotDelete, HttpStatusCode.InternalServerError, workAreaDetails);

                workAreaDetails.IsDeleted = true;
                var response = _workAreaPackService.Update(workAreaDetails, workAreaDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, workAreaDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, workAreaDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkWorkAreaPacks(List<Guid> ids)
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
                    var deleteResponse = await DeleteWorkAreaPack(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as WorkAreaPack;
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
                        Message = $"Failed to delete work area packs.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted work area packs. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of work area packs have not been successfully deleted. \n" +
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
        public async Task<List<WorkAreaPackInfoDto>> GetAllWorkAreaPackInfo(Guid projectId)
        {
            List<WorkAreaPackInfoDto> allWorkaAreas = await _workAreaPackService.GetAll(s => s.ProjectId == projectId && !s.IsDeleted).Select(s => new WorkAreaPackInfoDto
            {
                Id = s.Id,
                Number = s.Number,
                Description = s.Description ?? "",
                ProjectId = s.ProjectId,
            }).ToListAsync();

            return _mapper.Map<List<WorkAreaPackInfoDto>>(allWorkaAreas);
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<WorkAreaPackInfoDto>> ImportWorkAreaPack([FromForm] FileUploadModel info)
        {
            List<WorkAreaPackInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.WorkAreaPack || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.WorkAreaPackHeadings;

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

                    CreateOrEditWorkAreaPackDto workAreaPackDto = new()
                    {
                        Number = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    var importLog = new ImportLogDto
                    {
                        Name = workAreaPackDto.Number,
                        Operation = OperationType.Insert
                    };

                    var helper = new CommonHelper();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(workAreaPackDto);
                    isSuccess = validationResponse.Item1;

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
                            WorkAreaPack existingWorkArea;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingWorkArea == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), workAreaPackDto);
                                }
                                else
                                {
                                    var existingRecordName = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                         x.Number.ToLower().Trim() == workAreaPackDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        importLog.Items = GetChanges(existingWorkArea, workAreaPackDto);
                                    }
                                }
                            }
                            else
                            {
                                existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Number.ToLower().Trim() == workAreaPackDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                WorkAreaPack workAreaInfo = _mapper.Map<WorkAreaPack>(workAreaPackDto);
                                workAreaInfo.ProjectId = info.ProjectId;

                                if (existingWorkArea != null)
                                {
                                    isUpdate = true;
                                    workAreaInfo.Id = existingWorkArea.Id;
                                    workAreaInfo.CreatedBy = existingWorkArea.CreatedBy;
                                    workAreaInfo.CreatedDate = existingWorkArea.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(workAreaInfo, workAreaPackDto);

                                    var response = _workAreaPackService.Update(workAreaInfo, existingWorkArea, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(workAreaInfo, workAreaPackDto);

                                    var response = await _workAreaPackService.AddAsync(workAreaInfo, User.GetUserId());

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
                        message.AddRange(validationResponse.Item2);
                        importLog.Items = GetChanges(new(), workAreaPackDto);
                    }

                    WorkAreaPackInfoDto record = _mapper.Map<WorkAreaPackInfoDto>(workAreaPackDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
                    responseList.Add(record);

                    importLog.Message = record.Message;
                    importLog.Status = record.Status;
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateWorkAreaPack([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = new();

            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.WorkAreaPack || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.WorkAreaPackHeadings;

            var transaction = await _workAreaPackService.BeginTransaction();

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

                    CreateOrEditWorkAreaPackDto workAreaPackDto = new()
                    {
                        Number = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    ValidationDataDto validationData = new()
                    {
                        Operation = OperationType.Insert,
                        Name = workAreaPackDto.Number
                    };

                    var helper = new CommonHelper();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(workAreaPackDto);
                    isSuccess = validationResponse.Item1;

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
                            WorkAreaPack existingWorkArea;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingWorkArea == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), workAreaPackDto);
                                }
                                else
                                {
                                    var existingRecordName = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.Number.ToLower().Trim() == workAreaPackDto.Number.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Number is already taken.");
                                        validationData.Changes = GetChanges(existingWorkArea, workAreaPackDto);
                                    }
                                }
                            }
                            else
                            {
                                existingWorkArea = await _workAreaPackService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Number.ToLower().Trim() == workAreaPackDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                WorkAreaPack workAreaInfo = _mapper.Map<WorkAreaPack>(workAreaPackDto);
                                workAreaInfo.ProjectId = info.ProjectId;

                                if (existingWorkArea != null)
                                {
                                    isUpdate = true;
                                    workAreaInfo.Id = existingWorkArea.Id;
                                    workAreaInfo.CreatedBy = existingWorkArea.CreatedBy;
                                    workAreaInfo.CreatedDate = existingWorkArea.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingWorkArea, workAreaPackDto);

                                    var response = _workAreaPackService.Update(workAreaInfo, existingWorkArea, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(workAreaInfo, workAreaPackDto);

                                    var response = await _workAreaPackService.AddAsync(workAreaInfo, User.GetUserId());
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
                        message.AddRange(validationResponse.Item2);
                        validationData.Changes = GetChanges(new(), workAreaPackDto);
                    }

                    validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    validationData.Message = string.Join(", ", message);
                    validationDataList.Add(validationData);
                }
            }

            await _workAreaPackService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(WorkAreaPack entity, CreateOrEditWorkAreaPackDto createDto)
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
                    NewValue = createDto.Description ?? string.Empty,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description : string.Empty,
                }
            };
            return changes;
        }
    }
}
