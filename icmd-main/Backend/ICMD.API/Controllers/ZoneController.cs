using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Dtos.Zone;
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
    public class ZoneController : BaseController
    {
        private readonly IZoneService _zoneService;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Zone";

        private readonly ChangeLogHelper _changeLogHelper;

        public ZoneController(IMapper mapper, IZoneService zoneService, IDeviceService deviceService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _zoneService = zoneService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region Zone
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ZoneInfoDto>> GetAllZones(PagedAndSortedResultRequestDto input)
        {
            try
            {
                IQueryable<ZoneInfoDto> allZones = _zoneService.GetAll(s => !s.IsDeleted).Select(s => new ZoneInfoDto
                {
                    Id = s.Id,
                    Zone = s.Zone,
                    Description = s.Description,
                    Area = s.Area != null ? s.Area.ToString() : null,
                    ProjectId = s.ProjectId,
                });

                if (!string.IsNullOrEmpty(input.Search))
                {
                    allZones = allZones.Where(s => (!string.IsNullOrEmpty(s.Zone) && s.Zone.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Area != null && s.Area.ToLower().Contains(input.Search.ToLower())));
                }

                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
                {
                    foreach (var item in input.CustomSearchs)
                    {
                        if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                        {
                            var ids = item.FieldValue?.Split(",");
                            allZones = allZones.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                        }
                    }
                }
                if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                    allZones = allZones.Where(input.SearchColumnFilterQuery);

                allZones = allZones.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
                bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
                IQueryable<ZoneInfoDto> paginatedData = !isExport ? allZones.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allZones;

                return new PagedResultDto<ZoneInfoDto>(
               allZones.Count(),
               await paginatedData.ToListAsync()
           );
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<ZoneInfoDto?> GetZoneInfo(Guid id)
        {
            ServiceZone? zoneDetails = await _zoneService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (zoneDetails != null)
            {
                return _mapper.Map<ZoneInfoDto>(zoneDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditZone(CreateOrEditZoneDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateZone(info);
            }
            else
            {
                return await UpdateZone(info);
            }
        }

        private async Task<BaseResponse> CreateZone(CreateOrEditZoneDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceZone existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Zone.ToLower().Trim() == info.Zone.ToLower().Trim() && !x.IsDeleted);
                if (existingZone != null)
                    return new BaseResponse(false, ResponseMessages.ZoneAlreadyTaken, HttpStatusCode.Conflict);

                ServiceZone zoneInfo = _mapper.Map<ServiceZone>(info);
                zoneInfo.IsActive = true;
                var response = await _zoneService.AddAsync(zoneInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateZone(CreateOrEditZoneDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceZone zoneDetails = await _zoneService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (zoneDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                ServiceZone existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.Zone.ToLower().Trim() == info.Zone.ToLower().Trim() && !x.IsDeleted);
                if (existingZone != null)
                    return new BaseResponse(false, ResponseMessages.ZoneAlreadyTaken, HttpStatusCode.Conflict);

                ServiceZone zoneInfo = _mapper.Map<ServiceZone>(info);
                zoneInfo.CreatedBy = zoneDetails.CreatedBy;
                zoneInfo.CreatedDate = zoneDetails.CreatedDate;
                zoneInfo.IsActive = zoneDetails.IsActive;
                var response = _zoneService.Update(zoneInfo, zoneDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteZone(Guid id)
        {
            ServiceZone zoneDetails = await _zoneService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (zoneDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.ServiceZoneId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, zoneDetails);

                zoneDetails.IsDeleted = true;
                var response = _zoneService.Update(zoneDetails, zoneDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, zoneDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, zoneDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkZones(List<Guid> ids)
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
                    var deleteResponse = await DeleteZone(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as ServiceZone;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Zone,
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
                        Message = $"Failed to delete zones.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted zones. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of zones have not been successfully deleted. \n" +
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
        public async Task<ImportFileResultDto<ZoneInfoDto>> ImportZone([FromForm] FileUploadModel info)
        {
            List<ZoneInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.Zone || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.ZoneHeadings;

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

                    CreateOrEditZoneDto createDto = new()
                    {
                        Zone = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        Area = string.IsNullOrEmpty(dictionary[requiredKeys[2]]) ? null : Convert.ToInt32(dictionary[requiredKeys[2]]),
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    var importLog = new ImportLogDto
                    {
                        Name = createDto.Zone,
                        Operation = OperationType.Insert,
                    };

                    CommonHelper helper = new();
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
                            ServiceZone? existingZone;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingZone == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.Zone.ToLower().Trim() == createDto.Zone.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Zone is already taken.");
                                        importLog.Items = GetChanges(existingZone, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Zone.ToLower().Trim() == createDto.Zone.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                ServiceZone model = _mapper.Map<ServiceZone>(createDto);
                                model.ProjectId = info.ProjectId;

                                if (existingZone != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingZone.Id;
                                    model.CreatedBy = existingZone.CreatedBy;
                                    model.CreatedDate = existingZone.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingZone, createDto);

                                    var response = _zoneService.Update(model, existingZone, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(model, createDto);
                                    var response = await _zoneService.AddAsync(model, User.GetUserId());

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

                    ZoneInfoDto record = _mapper.Map<ZoneInfoDto>(createDto);
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportZone([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.Zone || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.ZoneHeadings;

            var transaction = await _zoneService.BeginTransaction();

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

                    CreateOrEditZoneDto createDto = new()
                    {
                        Zone = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        Area = string.IsNullOrEmpty(dictionary[requiredKeys[2]]) ? null : Convert.ToInt32(dictionary[requiredKeys[2]]),
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    ValidationDataDto validationData = new()
                    {
                        Name = createDto.Zone,
                        Operation = OperationType.Insert
                    };

                    CommonHelper helper = new();
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
                            ServiceZone? existingZone;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingZone == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.Zone.ToLower().Trim() == createDto.Zone.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Zone is already taken.");
                                        validationData.Changes = GetChanges(existingZone, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Zone.ToLower().Trim() == createDto.Zone.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                ServiceZone model = _mapper.Map<ServiceZone>(createDto);
                                model.ProjectId = info.ProjectId;

                                if (existingZone != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingZone.Id;
                                    model.CreatedBy = existingZone.CreatedBy;
                                    model.CreatedDate = existingZone.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingZone, createDto);

                                    var response = _zoneService.Update(model, existingZone, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(model, createDto);
                                    var response = await _zoneService.AddAsync(model, User.GetUserId());

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

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(ServiceZone entity, CreateOrEditZoneDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Zone),
                    NewValue = createDto.Zone,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Zone : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Area),
                    NewValue = createDto.Area?.ToString() ?? string.Empty,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Area?.ToString() ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }
    }
}
