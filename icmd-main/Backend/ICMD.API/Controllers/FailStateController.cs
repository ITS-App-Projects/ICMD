using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.FailState;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.API.Helpers;
using ICMD.Core.Dtos.UIChangeLog;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class FailStateController : BaseController
    {
        private readonly IFailStateService _failStateService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Fail state";

        private readonly ChangeLogHelper _changeLogHelper;
        public FailStateController(IFailStateService failStateService, IMapper mapper, IDeviceService deviceService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _failStateService = failStateService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region FailState
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<FailStateInfoDto>> GetAllFailStates(PagedAndSortedResultRequestDto input)
        {
            IQueryable<FailStateInfoDto> allFailStates = _failStateService.GetAll(s => !s.IsDeleted).Select(s => new FailStateInfoDto
            {
                Id = s.Id,
                FailStateName = s.FailStateName,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allFailStates = allFailStates.Where(s => (!string.IsNullOrEmpty(s.FailStateName) && s.FailStateName.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allFailStates = allFailStates.Where(input.SearchColumnFilterQuery);

            allFailStates = allFailStates.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<FailStateInfoDto> paginatedData = !isExport ? allFailStates.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allFailStates;


            return new PagedResultDto<FailStateInfoDto>(
               allFailStates.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<FailStateInfoDto?> GetFailStateInfo(Guid id)
        {
            FailState? failStateDetails = await _failStateService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (failStateDetails != null)
            {
                return _mapper.Map<FailStateInfoDto>(failStateDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditFailState(CreateOrEditFailStateDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateFailState(info);
            }
            else
            {
                return await UpdateFailState(info);
            }
        }

        private async Task<BaseResponse> CreateFailState(CreateOrEditFailStateDto info)
        {
            if (ModelState.IsValid)
            {
                FailState existingFailState = await _failStateService.GetSingleAsync(x => x.FailStateName.ToLower().Trim() == info.FailStateName.ToLower().Trim() && !x.IsDeleted);
                if (existingFailState != null)
                    return new BaseResponse(false, ResponseMessages.FailStateNameAlreadyTaken, HttpStatusCode.Conflict);

                FailState failStateInfo = _mapper.Map<FailState>(info);
                failStateInfo.IsActive = true;
                var response = await _failStateService.AddAsync(failStateInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateFailState(CreateOrEditFailStateDto info)
        {
            if (ModelState.IsValid)
            {
                FailState failStateDetails = await _failStateService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (failStateDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                FailState existingFailState = await _failStateService.GetSingleAsync(x => x.Id != info.Id && x.FailStateName.ToLower().Trim() == info.FailStateName.ToLower().Trim() && !x.IsDeleted);
                if (existingFailState != null)
                    return new BaseResponse(false, ResponseMessages.FailStateNameAlreadyTaken, HttpStatusCode.Conflict);

                FailState failStateInfo = _mapper.Map<FailState>(info);
                failStateInfo.CreatedBy = failStateDetails.CreatedBy;
                failStateInfo.CreatedDate = failStateDetails.CreatedDate;
                failStateInfo.IsActive = failStateDetails.IsActive;
                var response = _failStateService.Update(failStateInfo, failStateDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteFailState(Guid id)
        {
            FailState failStateDetails = await _failStateService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (failStateDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.FailStateId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, failStateDetails);

                failStateDetails.IsDeleted = true;
                var response = _failStateService.Update(failStateDetails, failStateDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, failStateDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, failStateDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkFailStates(List<Guid> ids)
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
                    var deleteResponse = await DeleteFailState(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as FailState;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.FailStateName,
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
                        Message = $"Failed to delete fail states.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted fail states. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of fail states have not been successfully deleted. \n" +
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
        public async Task<ImportFileResultDto<FailStateInfoDto>> ImportFailState([FromForm] FileUploadModel info)
        {
            List<FailStateInfoDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.FailState && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.FailStateHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            if (string.IsNullOrEmpty(dictionary[requiredKeys[0]])) continue;

                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditFailStateDto createDto = new()
                            {
                                FailStateName = dictionary[requiredKeys[0]],
                                Id = Guid.Empty
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                            isSuccess = validationResponse.Item1;

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    FailState existingFailState = await _failStateService.GetSingleAsync(x => x.FailStateName.ToLower().Trim() == createDto.FailStateName.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        if (existingFailState != null)
                                        {
                                            isUpdate = true;
                                            var response = _failStateService.Update(existingFailState, existingFailState, User.GetUserId());
                                        }
                                        else
                                        {
                                            FailState model = _mapper.Map<FailState>(createDto);
                                            var response = await _failStateService.AddAsync(model, User.GetUserId());

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
                                message.AddRange(validationResponse.Item2);

                            FailStateInfoDto record = _mapper.Map<FailStateInfoDto>(createDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            responseList.Add(record);
                        }
                    }
                }
                else
                {
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };
                }

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
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }
    }
}
