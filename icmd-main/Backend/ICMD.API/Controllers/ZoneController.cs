using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Zone;
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
    public class ZoneController : BaseController
    {
        private readonly IZoneService _zoneService;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Zone";
        public ZoneController(IMapper mapper, IZoneService zoneService, IDeviceService deviceService, CSVImport csvImport)
        {
            _zoneService = zoneService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
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
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                zoneDetails.IsDeleted = true;
                var response = _zoneService.Update(zoneDetails, zoneDetails, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<ZoneInfoDto>> ImportZone([FromForm] FileUploadModel info)
        {
            List<ZoneInfoDto> responseList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.Zone || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.ZoneHeadings;

            foreach (var dictionary in typeHeaders)
            {
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

                    CommonHelper helper = new();
                    Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                    isSuccess = validationResponse.Item1;

                    if (isSuccess)
                    {
                        bool isUpdate = false;
                        try
                        {
                            ServiceZone? existingZone = await _zoneService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Zone.ToLower().Trim() == createDto.Zone.ToLower().Trim() && !x.IsDeleted && x.IsActive);

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
                                    var response = _zoneService.Update(model, existingZone, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
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
                        message.AddRange(validationResponse.Item2);

                    ZoneInfoDto record = _mapper.Map<ZoneInfoDto>(createDto);
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
