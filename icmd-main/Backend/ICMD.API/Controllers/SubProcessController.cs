using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.SubProcess;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.API.Helpers;

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
        public SubProcessController(IMapper mapper, ISubProcessService subProcessService, ITagService tagService, CSVImport csvImport)
        {
            _subProcessService = subProcessService;
            _mapper = mapper;
            _tagService = tagService;
            _csvImport = csvImport;
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
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                subProcessDetail.IsDeleted = true;
                var response = _subProcessService.Update(subProcessDetail, subProcessDetail, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }
        #endregion


        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<SubProcessInfoDto>> ImportSubProcess([FromForm] FileUploadModel info)
        {
            List<SubProcessInfoDto> responseList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.TagField2 || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TagField2Headings;

            foreach (var dictionary in typeHeaders)
            {
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

                    var helper = new CommonHelper();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                    isSuccess = validationResponse.Item1;

                    if (isSuccess)
                    {
                        bool isUpdate = false;
                        try
                        {
                            SubProcess existingSubProcess = await _subProcessService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.SubProcessName.ToLower().Trim() == createDto.SubProcessName.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                            if (message.Count == 0)
                            {
                                SubProcess processInfo = _mapper.Map<SubProcess>(createDto);
                                processInfo.ProjectId = info.ProjectId;

                                if (existingSubProcess != null)
                                {
                                    processInfo.Id = existingSubProcess.Id;
                                    processInfo.CreatedBy = existingSubProcess.CreatedBy;
                                    processInfo.CreatedDate = existingSubProcess.CreatedDate;
                                    var response = _subProcessService.Update(processInfo, existingSubProcess, User.GetUserId());

                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
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
                        message.AddRange(validationResponse.Item2);

                    SubProcessInfoDto record = _mapper.Map<SubProcessInfoDto>(createDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
                    responseList.Add(record);
                }
            }

            return new()
            {
                IsSucceeded = true,
                Message = ResponseMessages.ImportFile,
                Records = responseList
            };
        }
    }
}
