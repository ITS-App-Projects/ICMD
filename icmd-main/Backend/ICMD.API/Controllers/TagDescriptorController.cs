using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.TagDescriptor;
using ICMD.Core.Dtos.TagType;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using ICMD.Repository.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

        private readonly ChangeLogHelper _changeLogHelper;

        public TagDescriptorController(IMapper mapper, ITagDescriptorService tagDescriptorService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _tagDescriptorService = tagDescriptorService;
            _mapper = mapper;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
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
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, tagDescriptorDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, tagDescriptorDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkTagDescriptors(List<Guid> ids)
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
                    var deleteResponse = await DeleteTagDescriptor(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as TagDescriptor;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Name,
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
                        Message = $"Failed to delete tag descriptors.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted tag descriptors. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of tag descriptors have not been successfully deleted. \n" +
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
        #endregion


        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<TagTypeInfoDto>> ImportTagDescriptor([FromForm] FileUploadModel info)
        {
            List<TagTypeInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if ((fileType == FileType.TagDescriptor || fileType == FileType.TagType)
                    && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.TagDescriptorHeadings;

                    var isEditImport = false;
                    if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                        isEditImport = true;

                    foreach (var columns in typeHeaders)
                    {
                        var dictionary = new Dictionary<string, string>();
                        var editId = Guid.Empty;
                        foreach (var item in columns)
                        {
                            if (item.Key == FileHeadingConstants.IdHeading)
                            {
                                var isSuccess = Guid.TryParse(item.Value, out editId);
                                if (!isSuccess)
                                    editId = Guid.Empty;

                                continue;
                            }

                            dictionary.Add(item.Key, item.Value);
                        }

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
                            var importLog = new ImportLogDto
                            {
                                Name = createDto.Name,
                                Operation = OperationType.Insert,
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                            isSuccess = validationResponse.Item1;
                            if (isEditImport && editId == Guid.Empty)
                            {
                                isSuccess = false;
                                message.Add("Id is incorrect format.");
                            }
                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    TagDescriptor existingTagDescriptor;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted);
                                        if (existingTagDescriptor == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _tagDescriptorService.GetSingleAsync(x => x.Id != editId &&
                                                x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() &&
                                                !x.IsDeleted);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                importLog.Items = GetChanges(existingTagDescriptor, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted);
                                    }
                                    if (message.Count == 0)
                                    {
                                        TagDescriptor model = _mapper.Map<TagDescriptor>(createDto);
                                        if (existingTagDescriptor != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingTagDescriptor.Id;
                                            model.CreatedBy = existingTagDescriptor.CreatedBy;
                                            model.CreatedDate = existingTagDescriptor.CreatedDate;

                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingTagDescriptor, createDto);

                                            var response = _tagDescriptorService.Update(model, existingTagDescriptor, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            importLog.Items = GetChanges(model, createDto);

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
                            {
                                message.AddRange(validationResponse.Item2);
                                importLog.Items = GetChanges(new(), createDto);
                            }

                            TagTypeInfoDto record = _mapper.Map<TagTypeInfoDto>(createDto);
                            record.TagDescriptorName = createDto.Name;
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            responseList.Add(record);

                            importLog.Status = record.Status;
                            importLog.Message = record.Message;
                            importLogs.Add(importLog);
                        }
                    }
                }
                else
                {
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };
                }

                // Record logs
                await _changeLogHelper.CreateImportLogs(ModuleName, importLogs);

                if (responseList.All(x => x.Status == ImportFileRecordStatus.Success))
                {
                    return new()
                    {
                        IsSucceeded = true,
                        Message = ResponseMessages.ImportFile,
                        Records = responseList
                    };
                }
                else if (responseList.All(x => x.Status == ImportFileRecordStatus.Fail))
                {

                    return new()
                    {
                        IsSucceeded = false,
                        Message = ResponseMessages.FailedImportFile,
                        Records = responseList
                    };
                }

                return new()
                {
                    IsSucceeded = true,
                    IsWarning = true,
                    Message = ResponseMessages.SomeFailedImportFile,
                    Records = responseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportTagDescriptor([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if ((fileType == FileType.TagDescriptor || fileType == FileType.TagType)
                    && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.TagDescriptorHeadings;

                    var transaction = await _tagDescriptorService.BeginTransaction();

                    var isEditImport = false;
                    if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                        isEditImport = true;

                    foreach (var columns in typeHeaders)
                    {
                        var dictionary = new Dictionary<string, string>();
                        var editId = Guid.Empty;

                        foreach (var item in columns)
                        {
                            if (item.Key == FileHeadingConstants.IdHeading)
                            {
                                editId = Guid.Parse(item.Value);
                                continue;
                            }

                            dictionary.Add(item.Key, item.Value);
                        }

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
                            ValidationDataDto validationData = new()
                            {
                                Name = createDto.Name,
                                Operation = OperationType.Insert
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                            isSuccess = validationResponse.Item1;

                            if (isEditImport && editId == Guid.Empty)
                            {
                                isSuccess = false;
                                message.Add("Id is incorrect format.");
                            }

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    TagDescriptor existingTagDescriptor;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted);
                                        if (existingTagDescriptor == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _tagDescriptorService.GetSingleAsync(x => x.Id != editId &&
                                                x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() &&
                                                !x.IsDeleted);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                validationData.Changes = GetChanges(existingTagDescriptor, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingTagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted);
                                    }
                                    if (message.Count == 0)
                                    {
                                        TagDescriptor model = _mapper.Map<TagDescriptor>(createDto);
                                        if (existingTagDescriptor != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingTagDescriptor.Id;
                                            model.CreatedBy = existingTagDescriptor.CreatedBy;
                                            model.CreatedDate = existingTagDescriptor.CreatedDate;

                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingTagDescriptor, createDto);

                                            var response = _tagDescriptorService.Update(model, existingTagDescriptor, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            validationData.Changes = GetChanges(model, createDto);
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
                            {
                                message.AddRange(validationResponse.Item2);
                                validationData.Changes = GetChanges(new(), createDto);
                            }

                            validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            validationData.Message = string.Join(", ", message);
                            validationDataList.Add(validationData);

                        }
                    }
                    await _tagDescriptorService.RollbackTransaction(transaction);
                }
                else
                {
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };
                }

                return new()
                {
                    IsSucceeded = true,
                    Message = ResponseMessages.ImportFile,
                    Records = validationDataList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }

        private List<ChangesDto> GetChanges(TagDescriptor entity, CreateOrEditTagDescriptorDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Name),
                    NewValue = createDto.Name,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Name : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Description),
                    NewValue = createDto.Description,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Description ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }
    }
}
