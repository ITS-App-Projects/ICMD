using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ReferenceDocumentType;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.Core.Dtos;
using ICMD.API.Helpers;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReferenceDocumentTypeController : BaseController
    {
        private readonly IReferenceDocumentTypeService _referenceDocumentTypeService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Document type";
        public ReferenceDocumentTypeController(IMapper mapper, IReferenceDocumentTypeService referenceDocumentTypeService, IReferenceDocumentService referenceDocumentService, CSVImport csvImport)
        {
            _referenceDocumentTypeService = referenceDocumentTypeService;
            _mapper = mapper;
            _referenceDocumentService = referenceDocumentService;
            _csvImport = csvImport;
        }

        #region ReferenceDocumentType

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<TypeInfoDto>> GetAllDocumentTypes(PagedAndSortedResultRequestDto input)
        {
            IQueryable<TypeInfoDto> allTypes = _referenceDocumentTypeService.GetAll(s => !s.IsDeleted).Select(s => new TypeInfoDto
            {
                Id = s.Id,
                Type = s.Type,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTypes = allTypes.Where(s => (!string.IsNullOrEmpty(s.Type) && s.Type.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTypes = allTypes.Where(input.SearchColumnFilterQuery);

            allTypes = allTypes.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<TypeInfoDto> paginatedData = !isExport ? allTypes.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTypes;


            return new PagedResultDto<TypeInfoDto>(
               allTypes.Count(),
               await paginatedData.ToListAsync()
           );
        }



        [HttpGet]
        public async Task<TypeInfoDto?> GetDocumentTypeInfo(Guid id)
        {
            ReferenceDocumentType? typeDetails = await _referenceDocumentTypeService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (typeDetails != null)
            {
                return _mapper.Map<TypeInfoDto>(typeDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditReferenceDocumentType(CreateOrEditReferenceDocumentType info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateDocumentType(info);
            }
            else
            {
                return await UpdateDocumentType(info);
            }
        }

        private async Task<BaseResponse> CreateDocumentType(CreateOrEditReferenceDocumentType info)
        {
            if (ModelState.IsValid)
            {
                ReferenceDocumentType existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == info.Type.ToLower().Trim() && !x.IsDeleted);
                if (existingType != null)
                    return new BaseResponse(false, ResponseMessages.TypeAlreadyTaken, HttpStatusCode.Conflict);

                ReferenceDocumentType typeInfo = _mapper.Map<ReferenceDocumentType>(info);
                typeInfo.IsActive = true;
                var response = await _referenceDocumentTypeService.AddAsync(typeInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateDocumentType(CreateOrEditReferenceDocumentType info)
        {
            if (ModelState.IsValid)
            {
                ReferenceDocumentType typeDetails = await _referenceDocumentTypeService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (typeDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                ReferenceDocumentType existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Id != info.Id && x.Type.ToLower().Trim() == info.Type.ToLower().Trim() && !x.IsDeleted);
                if (existingType != null)
                    return new BaseResponse(false, ResponseMessages.TypeAlreadyTaken, HttpStatusCode.Conflict);

                ReferenceDocumentType typeInfo = _mapper.Map<ReferenceDocumentType>(info);
                typeInfo.CreatedBy = typeDetails.CreatedBy;
                typeInfo.CreatedDate = typeDetails.CreatedDate;
                typeInfo.IsActive = typeDetails.IsActive;
                var response = _referenceDocumentTypeService.Update(typeInfo, typeDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteDocumentType(Guid id)
        {
            ReferenceDocumentType typeDetails = await _referenceDocumentTypeService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (typeDetails != null)
            {
                bool isChkExist = _referenceDocumentService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentTypeId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.TypeNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError);

                typeDetails.IsDeleted = true;
                var response = _referenceDocumentTypeService.Update(typeDetails, typeDetails, User.GetUserId(), true, true);
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
        public async Task<List<DropdownInfoDto>> GetAllDocumentTypeInfo()
        {
            List<DropdownInfoDto> allTypeInfo = await _referenceDocumentTypeService.GetAll(s => s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto
            {
                Id = s.Id,
                Name = s.Type,
            }).ToListAsync();

            return allTypeInfo;
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<TypeInfoDto>> ImportReferenceDocumentType([FromForm] FileUploadModel info)
        {
            List<TypeInfoDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.ReferenceDocumentType && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.ReferenceDocumentTypeHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditReferenceDocumentType createDto = new()
                            {
                                Type = dictionary[requiredKeys[0]],
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
                                    ReferenceDocumentType existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        if (existingType != null)
                                        {
                                            isUpdate = true;
                                            var response = _referenceDocumentTypeService.Update(existingType, existingType, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            ReferenceDocumentType model = _mapper.Map<ReferenceDocumentType>(createDto);
                                            var response = await _referenceDocumentTypeService.AddAsync(model, User.GetUserId());

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

                            TypeInfoDto record = _mapper.Map<TypeInfoDto>(createDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            responseList.Add(record);
                        }
                    }
                }
                else
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

                return new()
                {
                    IsSucceeded = true,
                    Message = ResponseMessages.ImportFile,
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
