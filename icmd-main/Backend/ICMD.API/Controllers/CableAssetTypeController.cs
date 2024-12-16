using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.CableAsset;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CableAssetTypeController : BaseController
    {
        private readonly ICableAssetTypeService _cableAssetTypeService;
        private readonly CSVImport _csvImport;
        private readonly IMapper _mapper;
        private static string ModuleName = "Cable Asset Type";
        public CableAssetTypeController(IMapper mapper, ICableAssetTypeService cableAssetService, CSVImport csvImport)
        {
            _cableAssetTypeService = cableAssetService;
            _mapper = mapper;
            _csvImport = csvImport;
        }

        #region ServiceCableAsset

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<CableAssetTypeInfoDto>> GetAllCableAssetTypes(PagedAndSortedResultRequestDto input)
        {
            IQueryable<CableAssetTypeInfoDto> allCableAssetTypes = _cableAssetTypeService.GetAll(c => !c.IsDeleted).Select(c => new CableAssetTypeInfoDto
            {
                Id = c.Id,
                CableAssetType = c.Type,
                CableAssetDescription = c.Description,
                CableCategory = c.Category,
                CableCategoryDescription = c.CategoryDescription,
                CableType = c.CableType,
                CableTypeDescription = c.CableTypeDescription,
                ProjectId = c.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allCableAssetTypes = allCableAssetTypes.Where(c => !string.IsNullOrEmpty(c.CableType) && c.CableType.ToLower().Contains(input.Search.ToLower()));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allCableAssetTypes = allCableAssetTypes.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allCableAssetTypes = allCableAssetTypes.Where(input.SearchColumnFilterQuery);

            allCableAssetTypes = allCableAssetTypes.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<CableAssetTypeInfoDto> paginatedData = !isExport ? allCableAssetTypes.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allCableAssetTypes;

            return new PagedResultDto<CableAssetTypeInfoDto>(
               allCableAssetTypes.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<CableAssetTypeInfoDto?> GetCableAssetTypeInfo(Guid id)
        {
            var cableAssetTypeDetails = await _cableAssetTypeService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (cableAssetTypeDetails != null)
            {
                return _mapper.Map<CableAssetTypeInfoDto>(cableAssetTypeDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditCableAsset(CreateOrEditCableAssetTypeDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateCableAssetType(info);
            }
            else
            {
                return await UpdateCableAssetType(info);
            }
        }

        private async Task<BaseResponse> CreateCableAssetType(CreateOrEditCableAssetTypeDto info)
        {
            if (ModelState.IsValid)
            {
                CableAssetType existingCableAsset = await _cableAssetTypeService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Type.ToLower().Trim() == info.CableType.ToLower().Trim() && !x.IsDeleted);
                if (existingCableAsset != null)
                    return new BaseResponse(false, ResponseMessages.CableAssetTypeExist, HttpStatusCode.Conflict);

                CableAssetType cableAssetTypeInfo = _mapper.Map<CableAssetType>(info);
                cableAssetTypeInfo.IsActive = true;
                var response = await _cableAssetTypeService.AddAsync(cableAssetTypeInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateCableAssetType(CreateOrEditCableAssetTypeDto info)
        {
            if (ModelState.IsValid)
            {
                CableAssetType cableAssetTypeDetails = await _cableAssetTypeService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (cableAssetTypeDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                CableAssetType existingCableAsset = await _cableAssetTypeService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.Type.ToLower().Trim() == info.CableAssetType!.ToLower().Trim() && !x.IsDeleted);
                if (existingCableAsset != null)
                    return new BaseResponse(false, ResponseMessages.CableAssetTypeExist, HttpStatusCode.Conflict);

                CableAssetType cableAssetTypeInfo = _mapper.Map<CableAssetType>(info);
                cableAssetTypeInfo.CreatedBy = cableAssetTypeDetails.CreatedBy;
                cableAssetTypeInfo.CreatedDate = cableAssetTypeDetails.CreatedDate;
                cableAssetTypeInfo.IsActive = cableAssetTypeDetails.IsActive;
                var response = _cableAssetTypeService.Update(cableAssetTypeInfo, cableAssetTypeDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteCableAssetType(Guid id)
        {
            CableAssetType cableAssetDetail = await _cableAssetTypeService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (cableAssetDetail != null)
            {
                // TODO: Check on cable list if it is used.
                //bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.ServiceBankId == id).Any();
                //if (isChkExist)
                //    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                cableAssetDetail.IsDeleted = true;
                var response = _cableAssetTypeService.Update(cableAssetDetail, cableAssetDetail, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<CableAssetTypeInfoDto>> ImportCableAssetType([FromForm] FileUploadModel info)
        {
            List<CableAssetTypeInfoDto> cableAssetResponseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.CableAssetType && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.CableAssetTypeListHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditCableAssetTypeDto cableAssetDto = new()
                            {
                                CableAssetType = dictionary[requiredKeys[0]],
                                CableAssetDescription = dictionary[requiredKeys[1]],
                                CableCategory = dictionary[requiredKeys[2]],
                                CableCategoryDescription = dictionary[requiredKeys[3]],
                                CableType = dictionary[requiredKeys[4]],
                                CableTypeDescription = dictionary[requiredKeys[5]],
                                ProjectId = info.ProjectId,
                                Id = Guid.Empty
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(cableAssetDto);
                            isSuccess = validationResponse.Item1;

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    var existingCableAsset = await _cableAssetTypeService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Type.ToLower().Trim() == cableAssetDto.CableAssetType.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        if (existingCableAsset != null)
                                        {
                                            isUpdate = true;
                                            var response = _cableAssetTypeService.Update(existingCableAsset, existingCableAsset, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));

                                        }
                                        else
                                        {
                                            var cableAssetTypeInfo = _mapper.Map<CableAssetType>(cableAssetDto);
                                            cableAssetDto.ProjectId = info.ProjectId;

                                            var response = await _cableAssetTypeService.AddAsync(cableAssetTypeInfo, User.GetUserId());

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

                            CableAssetTypeInfoDto record = _mapper.Map<CableAssetTypeInfoDto>(cableAssetDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            cableAssetResponseList.Add(record);
                        }
                    }
                }
                else
                {
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };
                }

                return new()
                {
                    IsSucceeded = true,
                    Message = ResponseMessages.ImportFile,
                    Records = cableAssetResponseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }
    }
}
