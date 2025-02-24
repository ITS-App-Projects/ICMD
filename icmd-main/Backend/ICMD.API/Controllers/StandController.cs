using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.JunctionBox;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Dtos.Stand;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class StandController : BaseController
    {
        private readonly IStandService _standService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CommonMethods _commonMethods;
        private static string ModuleName = "Stand";
        private readonly ChangeLogHelper _changeLogHelper;
        public StandController(IMapper mapper, IStandService standService, IDeviceService deviceService, CommonMethods commonMethods, ChangeLogHelper changeLogHelper)
        {
            _standService = standService;
            _mapper = mapper;
            _deviceService = deviceService;
            _commonMethods = commonMethods;
            _changeLogHelper = changeLogHelper;
        }

        #region Stand
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<StandListDto>> GetAllStands(PagedAndSortedResultRequestDto input)
        {
            IQueryable<StandListDto> allStands = _standService.GetAll(s => !s.IsDeleted).Select(s => new StandListDto
            {
                Id = s.Id,
                Tag = s.Tag != null ? s.Tag.TagName : "",
                Process = (s.Tag != null && s.Tag.Process != null) ? s.Tag.Process.ProcessName : "",
                SubProcess = (s.Tag != null && s.Tag.SubProcess != null) ? s.Tag.SubProcess.SubProcessName : "",
                Stream = (s.Tag != null && s.Tag.Stream != null) ? s.Tag.Stream.StreamName : "",
                EquipmentCode = (s.Tag != null && s.Tag.EquipmentCode != null) ? s.Tag.EquipmentCode.Code : "",
                SequenceNumber = (s.Tag != null && s.Tag.SequenceNumber != null) ? s.Tag.SequenceNumber : "",
                EquipmentIdentifier = (s.Tag != null && s.Tag.EquipmentIdentifier != null) ? s.Tag.EquipmentIdentifier : "",
                Type = s.Type,
                Description = s.Description,
                Number = (s.ReferenceDocument != null) ? s.ReferenceDocument.DocumentNumber : "",
                ReferenceDocumentType = (s.ReferenceDocument != null) ? s.ReferenceDocument.ReferenceDocumentType.Type : string.Empty,
                DocumentNumber = (s.ReferenceDocument != null) ? _commonMethods.GenerateFullReportName(s.ReferenceDocument) : "",
                Revision = (s.ReferenceDocument != null) ? s.ReferenceDocument.Revision : "",
                Sheet = (s.ReferenceDocument != null) ? s.ReferenceDocument.Sheet : "",
                Version = (s.ReferenceDocument != null) ? s.ReferenceDocument.Version : "",
                IsVDPDocumentNumber = (s.ReferenceDocument != null) ? s.ReferenceDocument.IsVDPDocumentNumber : false,
                ProjectId = s.Tag != null ? s.Tag.ProjectId : null,
                IsActive = s.IsActive,
                Area = s.Area ?? ""
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allStands = allStands.Where(s =>
                (!string.IsNullOrEmpty(s.Tag) && s.Tag.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Process) && s.Process.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.SubProcess) && s.SubProcess.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Stream) && s.Stream.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.EquipmentCode) && s.EquipmentCode.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.SequenceNumber) && s.SequenceNumber.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.EquipmentIdentifier) && s.EquipmentIdentifier.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Type) && s.Type.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Revision) && s.Revision.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Version) && s.Version.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Area) && s.Area.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Sheet) && s.Sheet.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Number) && s.Number.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allStands = allStands.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }


                    if (item.FieldName.ToLower() == "type".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        int value = Convert.ToInt16(item.FieldValue);
                        if (value == (int)RecordType.Active)
                        {
                            allStands = allStands.Where(x => x.IsActive);
                        }
                        else if (value == (int)RecordType.Inactive)
                        {
                            allStands = allStands.Where(x => !x.IsActive);
                        }
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allStands = allStands.Where(input.SearchColumnFilterQuery);

            allStands = allStands.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<StandListDto> paginatedData = !isExport ? allStands.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allStands;


            return new PagedResultDto<StandListDto>(
               allStands.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<CreateOrEditStandDto?> GetStandInfo(Guid id)
        {
            Stand? standDetails = await _standService.GetAll(s => !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (standDetails != null)
            {
                CreateOrEditStandDto standInfo = _mapper.Map<CreateOrEditStandDto>(standDetails);
                standInfo.ReferenceDocumentTypeId = standDetails.ReferenceDocument != null ? standDetails.ReferenceDocument.ReferenceDocumentTypeId : null;
                return standInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditStand(CreateOrEditStandDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateStand(info);
            }
            else
            {
                return await UpdateStand(info);
            }
        }

        private async Task<BaseResponse> CreateStand(CreateOrEditStandDto info)
        {
            if (ModelState.IsValid)
            {
                Stand standInfo = _mapper.Map<Stand>(info);
                standInfo.IsActive = true;
                var response = await _standService.AddAsync(standInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                await _changeLogHelper.CreateStandChangeLog(new Stand(), info);
                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateStand(CreateOrEditStandDto info)
        {
            if (ModelState.IsValid)
            {
                Guid userId = User.GetUserId();
                Stand standDetails = await _standService.GetSingleAsync(s => s.Id == info.Id && !s.IsDeleted);
                if (standDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Stand standInfo = _mapper.Map<Stand>(info);
                standInfo.CreatedBy = standDetails.CreatedBy;
                standInfo.CreatedDate = standDetails.CreatedDate;
                standInfo.IsActive = standDetails.IsActive;
                var response = _standService.Update(standInfo, standDetails, userId);

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                await _changeLogHelper.CreateStandChangeLog(standDetails, info);
                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteStand(Guid id)
        {
            Stand standDetails = await _standService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (standDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.TagId == standDetails.TagId).Any();

                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleTagNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, standDetails);

                standDetails.IsDeleted = true;
                var response = _standService.Update(standDetails, standDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, standDetails);

                await _changeLogHelper.CreateActivationChangeLog(true, standDetails?.Tag?.TagName ?? "", "Stand", ChangeLogOptions.Deleted);
                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, standDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkStands(List<Guid> ids)
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
                    var deleteResponse = await DeleteStand(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as Stand;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Tag.TagName,
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
                        Message = $"Failed to delete stands.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted stands. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of stands have not been successfully deleted. \n" +
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

        [HttpPost]
        [AuthorizePermission(Operations.ActiveInActive)]
        public async Task<BaseResponse> ActiveInActiveStand(ActiveInActiveDto info)
        {
            Guid userId = User.GetUserId();
            Stand standDetails = await _standService.GetSingleAsync(s => s.Id == info.Id);
            if (standDetails != null)
            {
                if (!info.IsActive)
                {
                    bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.TagId == standDetails.TagId).Any();

                    if (isChkExist)
                        return new BaseResponse(false, ResponseMessages.ModuleTagNotDeactivatedAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);
                }
                standDetails.IsActive = info.IsActive;
                var response = _standService.Update(standDetails, standDetails, userId);

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                await _changeLogHelper.CreateActivationChangeLog(info.IsActive, standDetails?.Tag?.TagName ?? "", "Stand", ChangeLogOptions.ActiveDeactive);
                return new BaseResponse(true, info.IsActive ? ResponseMessages.ModuleActivate.ToString().Replace("{module}", ModuleName) : ResponseMessages.ModuleDeactivate.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<JunctionBoxListDto>> ImportStand([FromForm] FileUploadModel info)
        {
            return await _commonMethods.CommonBulkImport(info, FileType.Stand, User.GetUserId(), ModuleName);
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportStand([FromForm] FileUploadModel info)
        {
            return await _commonMethods.ValidateCommonBulkImport(info, FileType.Stand, User.GetUserId(), ModuleName);
        }
    }
}
