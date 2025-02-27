using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.SubProcess;
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
    public class SubProcessController : BaseController
    {
        private readonly ISubProcessService _subProcessService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Sub process";

        private readonly ChangeLogHelper _changeLogHelper;

        public SubProcessController(IMapper mapper, ISubProcessService subProcessService, ITagService tagService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _subProcessService = subProcessService;
            _mapper = mapper;
            _tagService = tagService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region SubProcess
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<SubProcessInfoDto>> GetAllSubProcess(PagedAndSortedResultRequestDto input)
        {
            IQueryable<SubProcessInfoDto> allSubProcess = _subProcessService.GetAll(s => !s.IsDeleted).Select(s => new SubProcessInfoDto
            {
                Id = s.Id,
                SubProcessName = s.SubProcessName,
                Description = s.Description,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allSubProcess = allSubProcess.Where(s => (!string.IsNullOrEmpty(s.SubProcessName) && s.SubProcessName.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allSubProcess = allSubProcess.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allSubProcess = allSubProcess.Where(input.SearchColumnFilterQuery);

            allSubProcess = allSubProcess.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<SubProcessInfoDto> paginatedData = !isExport ? allSubProcess.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allSubProcess;


            return new PagedResultDto<SubProcessInfoDto>(
               allSubProcess.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<SubProcessInfoDto?> GetSubProcessInfo(Guid id)
        {
            SubProcess? subProcessDetails = await _subProcessService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (subProcessDetails != null)
            {
                return _mapper.Map<SubProcessInfoDto>(subProcessDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditSubProcess(CreateOrEditSubProcessDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateSubProcess(info);
            }
            else
            {
                return await UpdateSubProcess(info);
            }
        }

        private async Task<BaseResponse> CreateSubProcess(CreateOrEditSubProcessDto info)
        {
            if (ModelState.IsValid)
            {
                SubProcess existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.SubProcessName.ToLower().Trim() == info.SubProcessName.ToLower().Trim() && !x.IsDeleted);
                if (existingSubProcess != null)
                    return new BaseResponse(false, ResponseMessages.SubProcessNameAlreadyTaken, HttpStatusCode.Conflict);

                SubProcess subProcessInfo = _mapper.Map<SubProcess>(info);
                subProcessInfo.IsActive = true;
                var response = await _subProcessService.AddAsync(subProcessInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateSubProcess(CreateOrEditSubProcessDto info)
        {
            if (ModelState.IsValid)
            {
                SubProcess subProcessDetails = await _subProcessService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (subProcessDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                SubProcess existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.SubProcessName.ToLower().Trim() == info.SubProcessName.ToLower().Trim() && !x.IsDeleted);
                if (existingSubProcess != null)
                    return new BaseResponse(false, ResponseMessages.SubProcessNameAlreadyTaken, HttpStatusCode.Conflict);

                SubProcess subProcessInfo = _mapper.Map<SubProcess>(info);
                subProcessInfo.CreatedBy = subProcessDetails.CreatedBy;
                subProcessInfo.CreatedDate = subProcessDetails.CreatedDate;
                subProcessInfo.IsActive = subProcessDetails.IsActive;
                var response = _subProcessService.Update(subProcessInfo, subProcessDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteSubProcess(Guid id)
        {
            SubProcess subProcessDetail = await _subProcessService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (subProcessDetail != null)
            {
                bool isChkExist = _tagService.GetAll(s => s.IsActive && !s.IsDeleted && s.SubProcessId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, subProcessDetail);

                subProcessDetail.IsDeleted = true;
                var response = _subProcessService.Update(subProcessDetail, subProcessDetail, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, subProcessDetail);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, subProcessDetail);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkSubProcesses(List<Guid> ids)
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
                    var deleteResponse = await DeleteSubProcess(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as SubProcess;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.SubProcessName,
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
                        Message = $"Failed to delete sub-processes.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted sub-processes. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of sub-processes have not been successfully deleted. \n" +
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
        #endregion


        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<SubProcessInfoDto>> ImportSubProcess([FromForm] FileUploadModel info)
        {
            List<SubProcessInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.TagField2 || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TagField2Headings;

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

                    CreateOrEditSubProcessDto createDto = new()
                    {
                        SubProcessName = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    var importLog = new ImportLogDto
                    {
                        Name = createDto.SubProcessName,
                        Operation = OperationType.Insert,
                    };

                    var helper = new CommonHelper();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
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
                            SubProcess existingSubProcess;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSubProcess == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.SubProcessName.ToLower().Trim() == createDto.SubProcessName.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Sub Process Name is already taken.");
                                        importLog.Items = GetChanges(existingSubProcess, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.SubProcessName.ToLower().Trim() == createDto.SubProcessName.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                SubProcess processInfo = _mapper.Map<SubProcess>(createDto);
                                processInfo.ProjectId = info.ProjectId;

                                if (existingSubProcess != null)
                                {
                                    isUpdate = true;
                                    processInfo.Id = existingSubProcess.Id;
                                    processInfo.CreatedBy = existingSubProcess.CreatedBy;
                                    processInfo.CreatedDate = existingSubProcess.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingSubProcess, createDto);

                                    var response = _subProcessService.Update(processInfo, existingSubProcess, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(processInfo, createDto);

                                    var response = await _subProcessService.AddAsync(processInfo, User.GetUserId());
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
                        importLog.Items = GetChanges(new(), createDto);
                    }

                    SubProcessInfoDto record = _mapper.Map<SubProcessInfoDto>(createDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportSubProcess([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.TagField2 || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TagField2Headings;

            var transaction = await _subProcessService.BeginTransaction();

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

                    CreateOrEditSubProcessDto createDto = new()
                    {
                        SubProcessName = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    ValidationDataDto validationData = new()
                    {
                        Name = createDto.SubProcessName,
                        Operation = OperationType.Insert
                    };

                    var helper = new CommonHelper();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
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
                            SubProcess existingSubProcess;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingSubProcess == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.SubProcessName.ToLower().Trim() == createDto.SubProcessName.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Sub Process Name is already taken.");
                                        validationData.Changes = GetChanges(existingSubProcess, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.SubProcessName.ToLower().Trim() == createDto.SubProcessName.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                SubProcess processInfo = _mapper.Map<SubProcess>(createDto);
                                processInfo.ProjectId = info.ProjectId;

                                if (existingSubProcess != null)
                                {
                                    isUpdate = true;
                                    processInfo.Id = existingSubProcess.Id;
                                    processInfo.CreatedBy = existingSubProcess.CreatedBy;
                                    processInfo.CreatedDate = existingSubProcess.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingSubProcess, createDto);

                                    var response = _subProcessService.Update(processInfo, existingSubProcess, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(processInfo, createDto);
                                    var response = await _subProcessService.AddAsync(processInfo, User.GetUserId());

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
                        validationData.Changes = GetChanges(new(), createDto);
                    }

                    validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    validationData.Message = string.Join(", ", message);
                    validationDataList.Add(validationData);
                }
            }
            await _subProcessService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(SubProcess entity, CreateOrEditSubProcessDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.SubProcessName),
                    NewValue = createDto.SubProcessName,
                    PreviousValue = entity.Id != Guid.Empty ? entity.SubProcessName : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description ?? string.Empty,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description : string.Empty,
                },
            };
            return changes;
        }
    }
}
