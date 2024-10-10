using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.TagType;
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
    public class TagTypeController : BaseController
    {
        private readonly ITagTypeService _tagTypeService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Tag type";
        public TagTypeController(IMapper mapper, ITagTypeService tagTypeService, CSVImport csvImport)
        {
            _tagTypeService = tagTypeService;
            _mapper = mapper;
            _csvImport = csvImport;
        }

        #region TagType
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<TagTypeInfoDto>> GetAllTagTypes(PagedAndSortedResultRequestDto input)
        {
            IQueryable<TagTypeInfoDto> allTagTypes = _tagTypeService.GetAll(s => !s.IsDeleted).Select(s => new TagTypeInfoDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTagTypes = allTagTypes.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTagTypes = allTagTypes.Where(input.SearchColumnFilterQuery);

            allTagTypes = allTagTypes.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<TagTypeInfoDto> paginatedData = !isExport ? allTagTypes.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTagTypes;


            return new PagedResultDto<TagTypeInfoDto>(
               allTagTypes.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<TagTypeInfoDto?> GetTagTypeInfo(Guid id)
        {
            TagType? tagTypeDetails = await _tagTypeService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (tagTypeDetails != null)
            {
                return _mapper.Map<TagTypeInfoDto>(tagTypeDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditTagType(CreateOrEditTagTypeDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateTagType(info);
            }
            else
            {
                return await UpdateTagType(info);
            }
        }

        private async Task<BaseResponse> CreateTagType(CreateOrEditTagTypeDto info)
        {
            if (ModelState.IsValid)
            {
                TagType existingTagType = await _tagTypeService.GetSingleAsync(x => x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingTagType != null)
                    return new BaseResponse(false, ResponseMessages.NameAlreadyTaken, HttpStatusCode.Conflict);

                TagType tagTypeInfo = _mapper.Map<TagType>(info);
                tagTypeInfo.IsActive = true;
                var response = await _tagTypeService.AddAsync(tagTypeInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateTagType(CreateOrEditTagTypeDto info)
        {
            if (ModelState.IsValid)
            {
                TagType tagTypeDetails = await _tagTypeService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (tagTypeDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                TagType existingTagType = await _tagTypeService.GetSingleAsync(x => x.Id != info.Id && x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingTagType != null)
                    return new BaseResponse(false, ResponseMessages.NameAlreadyTaken, HttpStatusCode.Conflict);

                TagType tagTypeInfo = _mapper.Map<TagType>(info);
                tagTypeInfo.CreatedBy = tagTypeDetails.CreatedBy;
                tagTypeInfo.CreatedDate = tagTypeDetails.CreatedDate;
                tagTypeInfo.IsActive = tagTypeDetails.IsActive;
                var response = _tagTypeService.Update(tagTypeInfo, tagTypeDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteTagType(Guid id)
        {
            TagType tagTypeDetails = await _tagTypeService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (tagTypeDetails != null)
            {
                tagTypeDetails.IsDeleted = true;
                var response = _tagTypeService.Update(tagTypeDetails, tagTypeDetails, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<TagTypeInfoDto>> ImportTagType([FromForm] FileUploadModel info)
        {
            List<TagTypeInfoDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if ((fileType == FileType.TagType || fileType == FileType.TagDescriptor)
                    && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.TagTypeHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditTagTypeDto createDto = new()
                            {
                                Name = dictionary[requiredKeys[0]],
                                Description = dictionary[requiredKeys[1]],
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
                                    TagType existingTagType = await _tagTypeService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted);

                                    if (message.Count == 0)
                                    {
                                        TagType model = _mapper.Map<TagType>(createDto);
                                        if (existingTagType != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingTagType.Id;
                                            model.CreatedBy = existingTagType.CreatedBy;
                                            model.CreatedDate = existingTagType.CreatedDate;
                                            var response = _tagTypeService.Update(model, existingTagType, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            var response = await _tagTypeService.AddAsync(model, User.GetUserId());

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

                            TagTypeInfoDto record = _mapper.Map<TagTypeInfoDto>(createDto);
                            record.TagTypeName = createDto.Name;
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
