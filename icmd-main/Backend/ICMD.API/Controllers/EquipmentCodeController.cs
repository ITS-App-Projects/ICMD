using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.EquipmentCode;
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
    public class EquipmentCodeController : BaseController
    {
        private readonly IEquipmentCodeService _equipmentCodeService;
        private readonly ITagService _tagServie;
        private readonly IMapper _mapper;
        private static string ModuleName = "Equipment code";
        private readonly CSVImport _csvImport;
        public EquipmentCodeController(IMapper mapper, IEquipmentCodeService equipmentCodeService, ITagService tagServie, CSVImport csvImport)
        {
            _equipmentCodeService = equipmentCodeService;
            _mapper = mapper;
            _tagServie = tagServie;
            _csvImport = csvImport;
        }

        #region EquipmentCode
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<EquipmentCodeInfoDto>> GetAllEquipmentCodes(PagedAndSortedResultRequestDto input)
        {
            IQueryable<EquipmentCodeInfoDto> allCodes = _equipmentCodeService.GetAll(s => !s.IsDeleted).Select(s => new EquipmentCodeInfoDto
            {
                Id = s.Id,
                Code = s.Code,
                Descriptor = s.Descriptor,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allCodes = allCodes.Where(s => (!string.IsNullOrEmpty(s.Code) && s.Code.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Descriptor) && s.Descriptor.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allCodes = allCodes.Where(input.SearchColumnFilterQuery);

            allCodes = allCodes.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<EquipmentCodeInfoDto> paginatedData = !isExport ? allCodes.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allCodes;


            return new PagedResultDto<EquipmentCodeInfoDto>(
               allCodes.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<EquipmentCodeInfoDto?> GetEquipmentCodeInfo(Guid id)
        {
            EquipmentCode? codeDetails = await _equipmentCodeService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (codeDetails != null)
            {
                return _mapper.Map<EquipmentCodeInfoDto>(codeDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditEquipmentCode(CreateOrEditEquipmentCodeDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateEquipmentCode(info);
            }
            else
            {
                return await UpdateEquipmentCode(info);
            }
        }

        private async Task<BaseResponse> CreateEquipmentCode(CreateOrEditEquipmentCodeDto info)
        {
            if (ModelState.IsValid)
            {
                EquipmentCode existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Code.ToLower().Trim() == info.Code.ToLower().Trim() && !x.IsDeleted);
                if (existingCode != null)
                    return new BaseResponse(false, ResponseMessages.CodeAlreadyTaken, HttpStatusCode.Conflict);

                EquipmentCode codeInfo = _mapper.Map<EquipmentCode>(info);
                codeInfo.IsActive = true;
                var response = await _equipmentCodeService.AddAsync(codeInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateEquipmentCode(CreateOrEditEquipmentCodeDto info)
        {
            if (ModelState.IsValid)
            {
                EquipmentCode codeDetails = await _equipmentCodeService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (codeDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                EquipmentCode existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Id != info.Id && x.Code.ToLower().Trim() == info.Code.ToLower().Trim() && !x.IsDeleted);
                if (existingCode != null)
                    return new BaseResponse(false, ResponseMessages.CodeAlreadyTaken, HttpStatusCode.Conflict);

                EquipmentCode codeInfo = _mapper.Map<EquipmentCode>(info);
                codeInfo.CreatedBy = codeDetails.CreatedBy;
                codeInfo.CreatedDate = codeDetails.CreatedDate;
                codeInfo.IsActive = codeDetails.IsActive;
                var response = _equipmentCodeService.Update(codeInfo, codeDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteEquipmentCode(Guid id)
        {
            EquipmentCode codeDetails = await _equipmentCodeService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (codeDetails != null)
            {
                bool isChkExist = _tagServie.GetAll(s => s.IsActive && !s.IsDeleted && s.EquipmentCodeId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                codeDetails.IsDeleted = true;
                var response = _equipmentCodeService.Update(codeDetails, codeDetails, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<EquipmentCodeInfoDto>> ImportEquipmentCode([FromForm] FileUploadModel info)
        {
            List<EquipmentCodeInfoDto> bankResponseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.EquipmentCode && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.EquipmentCodeHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditEquipmentCodeDto createDto = new()
                            {
                                Code = dictionary[requiredKeys[0]],
                                Descriptor = dictionary[requiredKeys[1]],
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
                                    EquipmentCode existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Code.ToLower().Trim() == createDto.Code.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        EquipmentCode model = _mapper.Map<EquipmentCode>(createDto);
                                        if (existingCode != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingCode.Id;
                                            model.CreatedBy = existingCode.CreatedBy;
                                            model.CreatedDate = existingCode.CreatedDate;
                                            var response = _equipmentCodeService.Update(model, existingCode, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            var response = await _equipmentCodeService.AddAsync(model, User.GetUserId());

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

                            EquipmentCodeInfoDto record = _mapper.Map<EquipmentCodeInfoDto>(createDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            bankResponseList.Add(record);
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
                    Records = bankResponseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }
    }
}
