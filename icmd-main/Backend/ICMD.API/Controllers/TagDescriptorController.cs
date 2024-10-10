using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.TagDescriptor;
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
    public class TagDescriptorController : BaseController
    {
        private readonly ITagDescriptorService _tagDescriptorService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Tag descriptor";
        public TagDescriptorController(IMapper mapper, ITagDescriptorService tagDescriptorService, CSVImport csvImport)
        {
            _tagDescriptorService = tagDescriptorService;
            _mapper = mapper;
            _csvImport = csvImport;
        }

        #region TagDescriptor
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<TagTypeInfoDto>> GetAllTagDescriptors(PagedAndSortedResultRequestDto input)
        {
            IQueryable<TagTypeInfoDto> allTagDescriptors = _tagDescriptorService.GetAll(s => !s.IsDeleted).Select(s => new TagTypeInfoDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTagDescriptors = allTagDescriptors.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTagDescriptors = allTagDescriptors.Where(input.SearchColumnFilterQuery);

            allTagDescriptors = allTagDescriptors.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<TagTypeInfoDto> paginatedData = !isExport ? allTagDescriptors.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTagDescriptors;


            return new PagedResultDto<TagTypeInfoDto>(
               allTagDescriptors.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<TagTypeInfoDto?> GetTagDescriptorInfo(Guid id)
        {
            TagDescriptor? tagDescriptordetails = await _tagDescriptorService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (tagDescriptordetails != null)
            {
                return _mapper.Map<TagTypeInfoDto>(tagDescriptordetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditTagDescriptor(CreateOrEditTagDescriptorDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateTagDescriptor(info);
            }
            else
            {
                return await UpdateTagDescriptor(info);
            }
        }

        private async Task<BaseResponse> CreateTagDescriptor(CreateOrEditTagDescriptorDto info)
        {
            if (ModelState.IsValid)
            {
                TagDescriptor existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingTagDescriptor != null)
                    return new BaseResponse(false, ResponseMessages.NameAlreadyTaken, HttpStatusCode.Conflict);

                TagDescriptor tagDescriptorInfo = _mapper.Map<TagDescriptor>(info);
                tagDescriptorInfo.IsActive = true;
                var response = await _tagDescriptorService.AddAsync(tagDescriptorInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateTagDescriptor(CreateOrEditTagDescriptorDto info)
        {
            if (ModelState.IsValid)
            {
                TagDescriptor tagDescriptorDetails = await _tagDescriptorService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (tagDescriptorDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                TagDescriptor existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Id != info.Id && x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingTagDescriptor != null)
                    return new BaseResponse(false, ResponseMessages.NameAlreadyTaken, HttpStatusCode.Conflict);

                TagDescriptor descriptorInfo = _mapper.Map<TagDescriptor>(info);
                descriptorInfo.CreatedBy = tagDescriptorDetails.CreatedBy;
                descriptorInfo.CreatedDate = tagDescriptorDetails.CreatedDate;
                descriptorInfo.IsActive = tagDescriptorDetails.IsActive;
                var response = _tagDescriptorService.Update(descriptorInfo, tagDescriptorDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteTagDescriptor(Guid id)
        {
            TagDescriptor tagDescriptorDetails = await _tagDescriptorService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (tagDescriptorDetails != null)
            {
                tagDescriptorDetails.IsDeleted = true;
                var response = _tagDescriptorService.Update(tagDescriptorDetails, tagDescriptorDetails, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<TagTypeInfoDto>> ImportTagDescriptor([FromForm] FileUploadModel info)
        {
            List<TagTypeInfoDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if ((fileType == FileType.TagDescriptor || fileType == FileType.TagType)
                    && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.TagDescriptorHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditTagDescriptorDto createDto = new()
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
                                    TagDescriptor existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted);

                                    if (message.Count == 0)
                                    {
                                        TagDescriptor model = _mapper.Map<TagDescriptor>(createDto);
                                        if (existingTagDescriptor != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingTagDescriptor.Id;
                                            model.CreatedBy = existingTagDescriptor.CreatedBy;
                                            model.CreatedDate = existingTagDescriptor.CreatedDate;
                                            var response = _tagDescriptorService.Update(model, existingTagDescriptor, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            var response = await _tagDescriptorService.AddAsync(model, User.GetUserId());

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
                            record.TagDescriptorName = createDto.Name;
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
