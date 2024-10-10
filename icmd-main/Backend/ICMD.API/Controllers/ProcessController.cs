using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Process;
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
    public class ProcessController : BaseController
    {
        private readonly IProcessService _processService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private static string ModuleName = "Process";
        private readonly CSVImport _csvImport;
        public ProcessController(IMapper mapper, IProcessService processService, ITagService tagService, CSVImport csvImport)
        {
            _processService = processService;
            _mapper = mapper;
            _tagService = tagService;
            _csvImport = csvImport;
        }

        #region Process
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ProcessInfoDto>> GetAllProcess(PagedAndSortedResultRequestDto input)
        {
            IQueryable<ProcessInfoDto> allProcess = _processService.GetAll(s => !s.IsDeleted).Select(s => new ProcessInfoDto
            {
                Id = s.Id,
                ProcessName = s.ProcessName,
                Description = s.Description,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allProcess = allProcess.Where(s => (!string.IsNullOrEmpty(s.ProcessName) && s.ProcessName.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allProcess = allProcess.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allProcess = allProcess.Where(input.SearchColumnFilterQuery);

            allProcess = allProcess.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<ProcessInfoDto> paginatedData = !isExport ? allProcess.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allProcess;


            return new PagedResultDto<ProcessInfoDto>(
               allProcess.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<ProcessInfoDto?> GetProcessInfo(Guid id)
        {
            Process? processDetails = await _processService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (processDetails != null)
            {
                return _mapper.Map<ProcessInfoDto>(processDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditProcess(CreateOrEditProcessDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateProcess(info);
            }
            else
            {
                return await UpdateProcess(info);
            }
        }

        private async Task<BaseResponse> CreateProcess(CreateOrEditProcessDto info)
        {
            if (ModelState.IsValid)
            {
                Process existingProcess = await _processService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.ProcessName.ToLower().Trim() == info.ProcessName.ToLower().Trim() && !x.IsDeleted);
                if (existingProcess != null)
                    return new BaseResponse(false, ResponseMessages.ProcessNameAlreadyTaken, HttpStatusCode.Conflict);

                Process processInfo = _mapper.Map<Process>(info);
                processInfo.IsActive = true;
                var response = await _processService.AddAsync(processInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateProcess(CreateOrEditProcessDto info)
        {
            if (ModelState.IsValid)
            {
                Process processDetails = await _processService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (processDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Process existingProcess = await _processService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.ProcessName.ToLower().Trim() == info.ProcessName.ToLower().Trim() && !x.IsDeleted);
                if (existingProcess != null)
                    return new BaseResponse(false, ResponseMessages.ProcessNameAlreadyTaken, HttpStatusCode.Conflict);

                Process processInfo = _mapper.Map<Process>(info);
                processInfo.CreatedBy = processDetails.CreatedBy;
                processInfo.CreatedDate = processDetails.CreatedDate;
                processInfo.IsActive = processDetails.IsActive;
                var response = _processService.Update(processInfo, processDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteProcess(Guid id)
        {
            Process processDetail = await _processService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (processDetail != null)
            {
                bool isChkExist = _tagService.GetAll(s => s.IsActive && !s.IsDeleted && s.ProcessId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                processDetail.IsDeleted = true;
                var response = _processService.Update(processDetail, processDetail, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<ProcessInfoDto>> ImportProcess([FromForm] FileUploadModel info)
        {
            List<ProcessInfoDto> responseList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.TagField1 || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TagField1Headings;

            foreach (var dictionary in typeHeaders)
            {
                var keys = dictionary.Keys.ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    bool isSuccess = false;
                    List<string> message = [];

                    CreateOrEditProcessDto createDto = new()
                    {
                        ProcessName = dictionary[requiredKeys[0]],
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
                            Process existingProcess = await _processService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.ProcessName.ToLower().Trim() == createDto.ProcessName.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                            if (message.Count == 0)
                            {
                                Process processInfo = _mapper.Map<Process>(createDto);
                                processInfo.ProjectId = info.ProjectId;
                                if (existingProcess != null)
                                {
                                    processInfo.Id = existingProcess.Id;
                                    processInfo.CreatedBy = existingProcess.CreatedBy;
                                    processInfo.CreatedDate = existingProcess.CreatedDate;
                                    var response = _processService.Update(processInfo, existingProcess, User.GetUserId());

                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    var response = await _processService.AddAsync(processInfo, User.GetUserId());

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

                    ProcessInfoDto record = _mapper.Map<ProcessInfoDto>(createDto);
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
