using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.Train;
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
    public class TrainController : BaseController
    {
        private readonly ITrainService _trainService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Train";

        private readonly ChangeLogHelper _changeLogHelper;

        public TrainController(ITrainService trainService, IMapper mapper, IDeviceService deviceService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _trainService = trainService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region Train
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<TrainInfoDto>> GetAllTrains(PagedAndSortedResultRequestDto input)
        {
            IQueryable<TrainInfoDto> allTrains = _trainService.GetAll(s => !s.IsDeleted).Select(s => new TrainInfoDto
            {
                Id = s.Id,
                Train = s.Train,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTrains = allTrains.Where(s => (!string.IsNullOrEmpty(s.Train) && s.Train.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allTrains = allTrains.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTrains = allTrains.Where(input.SearchColumnFilterQuery);

            allTrains = allTrains.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<TrainInfoDto> paginatedData = !isExport ? allTrains.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTrains;


            return new PagedResultDto<TrainInfoDto>(
               allTrains.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<TrainInfoDto?> GetTrainInfo(Guid id)
        {
            ServiceTrain? trainDetails = await _trainService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (trainDetails != null)
            {
                return _mapper.Map<TrainInfoDto>(trainDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditTrain(CreateOrEditTrainDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateTrain(info);
            }
            else
            {
                return await UpdateTrain(info);
            }
        }

        private async Task<BaseResponse> CreateTrain(CreateOrEditTrainDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceTrain existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Train.ToLower().Trim() == info.Train.ToLower().Trim() && !x.IsDeleted);
                if (existingTrain != null)
                    return new BaseResponse(false, ResponseMessages.TrainAlreadyTaken, HttpStatusCode.Conflict);

                ServiceTrain trainInfo = _mapper.Map<ServiceTrain>(info);
                trainInfo.IsActive = true;
                var response = await _trainService.AddAsync(trainInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateTrain(CreateOrEditTrainDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceTrain trainDetails = await _trainService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (trainDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                ServiceTrain existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.Train.ToLower().Trim() == info.Train.ToLower().Trim() && !x.IsDeleted);
                if (existingTrain != null)
                    return new BaseResponse(false, ResponseMessages.TrainAlreadyTaken, HttpStatusCode.Conflict);

                ServiceTrain trainInfo = _mapper.Map<ServiceTrain>(info);
                trainInfo.CreatedBy = trainDetails.CreatedBy;
                trainInfo.CreatedDate = trainDetails.CreatedDate;
                trainInfo.IsActive = trainDetails.IsActive;
                var response = _trainService.Update(trainInfo, trainDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteTrain(Guid id)
        {
            ServiceTrain trainDetails = await _trainService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (trainDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.ServiceTrainId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, trainDetails);

                trainDetails.IsDeleted = true;
                var response = _trainService.Update(trainDetails, trainDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, trainDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, trainDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkTrains(List<Guid> ids)
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
                    var deleteResponse = await DeleteTrain(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as ServiceTrain;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Train,
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
                        Message = $"Failed to delete trains.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted trains. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of trains have not been successfully deleted. \n" +
                    $"Success: {result.Where(r => r.IsSucceeded).Count()} \n" +
                    $"Failed: {result.Where(r => !r.IsSucceeded).Count()} \n" +
                    $"Please check logs for more details.",
                    Data = result
                };
            }
            catch (Exception)
            {
                return new BaseResponse(false, "Unexpected error occured. Please try again", HttpStatusCode.BadRequest);
            }
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<TrainInfoDto>> ImportTrain([FromForm] FileUploadModel info)
        {
            List<TrainInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.Train || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TrainHeadings;

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

                    CreateOrEditTrainDto createDto = new()
                    {
                        Train = dictionary[requiredKeys[0]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    var importLog = new ImportLogDto
                    {
                        Name = createDto.Train,
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
                            ServiceTrain existingTrain;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                importLog.Operation = OperationType.Edit;
                                existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingTrain == null)
                                {
                                    message.Add("Record is not found.");
                                    importLog.Items = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.Train.ToLower().Trim() == createDto.Train.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Train is already taken.");
                                        importLog.Items = GetChanges(existingTrain, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Train.ToLower().Trim() == createDto.Train.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                if (existingTrain != null)
                                {
                                    isUpdate = true;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingTrain, createDto);

                                    if (isEditImport && editId != Guid.Empty)
                                        existingTrain.Train = createDto.Train;

                                    var response = _trainService.Update(existingTrain, existingTrain, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    ServiceTrain model = new()
                                    {
                                        Train = createDto.Train,
                                        ProjectId = info.ProjectId,
                                    };
                                    importLog.Items = GetChanges(model, createDto);

                                    var response = await _trainService.AddAsync(model, User.GetUserId());

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

                    TrainInfoDto record = _mapper.Map<TrainInfoDto>(createDto);
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportTrain([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.Train || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TrainHeadings;

            var transaction = await _trainService.BeginTransaction();

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

                    CreateOrEditTrainDto createDto = new()
                    {
                        Train = dictionary[requiredKeys[0]],
                        ProjectId = info.ProjectId,
                        Id = Guid.Empty
                    };
                    ValidationDataDto validationData = new()
                    {
                        Name = createDto.Train,
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
                            ServiceTrain existingTrain;
                            if (isEditImport && editId != Guid.Empty)
                            {
                                validationData.Operation = OperationType.Edit;
                                existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                    x.Id == editId &&
                                    !x.IsDeleted && x.IsActive);
                                if (existingTrain == null)
                                {
                                    message.Add("Record is not found.");
                                    validationData.Changes = GetChanges(new(), createDto);
                                }
                                else
                                {
                                    var existingRecordName = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                        x.Id != editId &&
                                        x.Train.ToLower().Trim() == createDto.Train.ToLower().Trim() &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingRecordName != null)
                                    {
                                        message.Add("Train is already taken.");
                                        validationData.Changes = GetChanges(existingTrain, createDto);
                                    }
                                }
                            }
                            else
                            {
                                existingTrain = await _trainService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Train.ToLower().Trim() == createDto.Train.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                            }
                            if (message.Count == 0)
                            {
                                if (existingTrain != null)
                                {
                                    isUpdate = true;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingTrain, createDto);

                                    if (isEditImport && editId != Guid.Empty)
                                        existingTrain.Train = createDto.Train;

                                    var response = _trainService.Update(existingTrain, existingTrain, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    ServiceTrain model = new()
                                    {
                                        Train = createDto.Train,
                                        ProjectId = info.ProjectId,
                                    };
                                    validationData.Changes = GetChanges(model, createDto);

                                    var response = await _trainService.AddAsync(model, User.GetUserId());

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

            await _trainService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }
        private List<ChangesDto> GetChanges(ServiceTrain entity, CreateOrEditTrainDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Train),
                    NewValue = createDto.Train,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Train : string.Empty,
                },
            };
            return changes;
        }
    }
}
