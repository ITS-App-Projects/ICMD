using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.System;
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
    public class SystemController : BaseController
    {
        private readonly ISystemService _systemService;
        private readonly ISubSystemService _subSystemService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly IMapper _mapper;
        private static string ModuleName = "System";
        private readonly CSVImport _csvImport;
        public SystemController(IMapper mapper, ISystemService systemService, ISubSystemService subSystemService, IWorkAreaPackService workAreaPackService, CSVImport csvImport)
        {
            _systemService = systemService;
            _mapper = mapper;
            _subSystemService = subSystemService;
            _workAreaPackService = workAreaPackService;
            _csvImport = csvImport;
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
                    return new BaseResponse(false, ResponseMessages.SystemNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError);

                systemDetails.IsDeleted = true;
                var response = _systemService.Update(systemDetails, systemDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
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
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.System || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.SystemHeadings;

            foreach (var dictionary in typeHeaders)
            {
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

                    CommonHelper helper = new();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                    isSuccess = validationResponse.Item1;
                    if (!isSuccess) message.AddRange(validationResponse.Item2);

                    if (workAreaPack == null)
                    {
                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "work area pack"));
                        if (isSuccess) isSuccess = false;
                    }

                    if (isSuccess)
                    {
                        bool isUpdate = false;
                        try
                        {
                            Core.DBModels.System? existingSystem = await _systemService.GetSingleAsync(x => x.WorkAreaPackId == createDto.WorkAreaPackId && x.Number.ToLower().Trim() == createDto.Number.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                            if (message.Count == 0)
                            {
                                Core.DBModels.System model = _mapper.Map<Core.DBModels.System>(createDto);
                                
                                if (existingSystem != null)
                                {
                                    isUpdate = true;
                                    model.Id = existingSystem.Id;
                                    model.CreatedBy = existingSystem.CreatedBy;
                                    model.CreatedDate = existingSystem.CreatedDate;
                                    var response = _systemService.Update(model, existingSystem, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
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

                    SystemInfoDto record = _mapper.Map<SystemInfoDto>(createDto);
                    record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    record.Message = string.Join(", ", message);
                    record.WorkAreaPack = workAreaPackNumber;
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
