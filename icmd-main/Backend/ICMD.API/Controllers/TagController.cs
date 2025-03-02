using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.Project;
using ICMD.Core.Dtos.Tag;
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
    public class TagController : BaseController
    {
        private readonly ITagService _tagService;
        private readonly IDeviceService _deviceService;
        private readonly IStandService _standService;
        private readonly ICableService _cableService;
        private readonly CommonMethods _commonMethods;
        private readonly IProcessService _processService;
        private readonly ISubProcessService _subProcessService;
        private readonly IStreamService _streamService;
        private readonly IEquipmentCodeService _equipmentCodeService;
        private readonly ITagTypeService _tagTypeService;
        private readonly ITagDescriptorService _tagDescriptorService;
        private readonly IMapper _mapper;
        private readonly ISkidService _skidService;
        private readonly IPanelService _panelService;
        private readonly IJunctionBoxService _junctionBoxService;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Tag";
        private static string TagNameKey = "Tag";

        private readonly ChangeLogHelper _changeLogHelper;

        public TagController(IMapper mapper, ITagService tagService, IDeviceService deviceService, IStandService standService, ICableService cableService, CommonMethods commonMethods,
            IProcessService processService, ISubProcessService subProcessService, IEquipmentCodeService equipmentCodeService, IStreamService streamService,
            ITagTypeService tagTypeService, ITagDescriptorService tagDescriptorService, ISkidService skidService, IPanelService panelService, IJunctionBoxService junctionBoxService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _tagService = tagService;
            _mapper = mapper;
            _deviceService = deviceService;
            _standService = standService;
            _cableService = cableService;
            _commonMethods = commonMethods;
            _tagTypeService = tagTypeService;
            _tagDescriptorService = tagDescriptorService;
            _processService = processService;
            _subProcessService = subProcessService;
            _streamService = streamService;
            _equipmentCodeService = equipmentCodeService;
            _skidService = skidService;
            _panelService = panelService;
            _junctionBoxService = junctionBoxService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region Tag
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<TagListDto>> GetAllTags(PagedAndSortedResultRequestDto input)
        {
            string? requestProjectId = input.CustomSearchs.Find(x => x.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(x.FieldValue))?.FieldValue;
            Guid projectId = Guid.Empty;
            List<ProjectTagFieldInfoDto> fieldSourceInfo = (!string.IsNullOrEmpty(requestProjectId) && Guid.TryParse(requestProjectId, out projectId)) ? await _commonMethods.GetProjectTagFieldDataInfo(projectId) : [];
            List<string> tableSources = fieldSourceInfo.Select(s => s.Source ?? "").ToList();

            List<Tag> tags = await _tagService.GetAll(s => !s.IsDeleted && s.ProjectId == projectId).ToListAsync();
            IQueryable<TagListDto> allTags = tags.Select(s => new TagListDto
            {
                Id = s.Id,
                Tag = s.TagName,
                Field1String = GetTagFieldValue(s, fieldSourceInfo[0], tableSources[0], 1),
                Field2String = GetTagFieldValue(s, fieldSourceInfo[1], tableSources[1], 2),
                Field3String = GetTagFieldValue(s, fieldSourceInfo[2], tableSources[2], 3),
                Field4String = GetTagFieldValue(s, fieldSourceInfo[3], tableSources[3], 4),
                Field5String = GetTagFieldValue(s, fieldSourceInfo[4], tableSources[4], 5),
                Field6String = GetTagFieldValue(s, fieldSourceInfo[5], tableSources[5], 6),
                ProjectId = s.ProjectId
            }).AsQueryable();

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTags = allTags.Where(s => (!string.IsNullOrEmpty(s.Tag) && s.Tag.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field1String) && s.Field1String.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field2String) && s.Field2String.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field3String) && s.Field3String.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field4String) && s.Field4String.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field5String) && s.Field5String.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Field6String) && s.Field6String.ToLower().Contains(input.Search.ToLower())));
            }

            //if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            //{
            //    foreach (var item in input.CustomSearchs)
            //    {
            //        if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
            //        {
            //            var ids = item.FieldValue?.Split(",");
            //            allTags = allTags.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
            //        }
            //    }
            //}

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTags = allTags.Where(input.SearchColumnFilterQuery);

            allTags = allTags.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<TagListDto> paginatedData = !isExport ? allTags.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTags;


            return new PagedResultDto<TagListDto>(
               allTags.Count(),
               paginatedData.ToList()
           );
        }

        [HttpGet]
        public async Task<CreateOrEditTagDto?> GetTagInfo(Guid id)
        {
            Tag? tagDetails = await _tagService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (tagDetails != null)
            {
                List<ProjectTagFieldInfoDto> fieldSourceInfo = await _commonMethods.GetProjectTagFieldDataInfo(tagDetails.ProjectId);
                List<string> tableSources = fieldSourceInfo.Select(s => s.Source ?? "").ToList();
                CreateOrEditTagDto tagInfo = _mapper.Map<CreateOrEditTagDto>(tagDetails);
                tagInfo.Field1Id = GetTagListValue(tagDetails, tableSources[0]);
                tagInfo.Field2Id = GetTagListValue(tagDetails, tableSources[1]);
                tagInfo.Field3Id = GetTagListValue(tagDetails, tableSources[2]);
                tagInfo.Field4Id = GetTagListValue(tagDetails, tableSources[3]);
                tagInfo.Field5Id = GetTagListValue(tagDetails, tableSources[4]);
                tagInfo.Field6Id = GetTagListValue(tagDetails, tableSources[5]);
                tagInfo.Field5String = tagDetails.SequenceNumber;
                tagInfo.Field6String = tagDetails.EquipmentIdentifier;
                return tagInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditTag(CreateOrEditTagDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateTag(info);
            }
            else
            {
                return await UpdateTag(info);
            }
        }

        private async Task<BaseResponse> CreateTag(CreateOrEditTagDto info)
        {
            if (ModelState.IsValid)
            {
                Tag existingTag = await _tagService.GetSingleAsync(x => x.TagName.ToLower().Trim() == info.TagName.ToLower().Trim() && x.IsActive && !x.IsDeleted);
                if (existingTag != null)
                    return new BaseResponse(false, ResponseMessages.TagNameAlreadyTaken, HttpStatusCode.Conflict);

                Tag tagInfo = _mapper.Map<Tag>(info);
                tagInfo.SequenceNumber = info.Field5String;
                tagInfo.EquipmentIdentifier = info.Field6String;
                tagInfo.IsActive = true;

                tagInfo = await SetTagFieldValue(tagInfo, info);
                var response = await _tagService.AddAsync(tagInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateTag(CreateOrEditTagDto info)
        {
            if (ModelState.IsValid)
            {
                Guid userId = User.GetUserId();
                Tag tagDetails = await _tagService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (tagDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Tag existingTag = await _tagService.GetSingleAsync(x => x.Id != info.Id && x.TagName.ToLower().Trim() == info.TagName.ToLower().Trim() && x.IsActive && !x.IsDeleted);
                if (existingTag != null)
                    return new BaseResponse(false, ResponseMessages.TagNameAlreadyTaken, HttpStatusCode.Conflict);

                Tag tagInfo = _mapper.Map<Tag>(info);
                tagInfo.CreatedBy = tagDetails.CreatedBy;
                tagInfo.CreatedDate = tagDetails.CreatedDate;
                tagInfo.IsActive = tagDetails.IsActive;
                tagInfo.SequenceNumber = info.Field5String;
                tagInfo.EquipmentIdentifier = info.Field6String;
                tagInfo.IsActive = true;
                tagInfo = await SetTagFieldValue(tagInfo, info);
                var response = _tagService.Update(tagInfo, tagDetails, userId);

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteTag(Guid id)
        {
            Tag tagDetails = await _tagService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (tagDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.TagId == id).Any();
                bool isStandChkExist = _standService.GetAll(s => s.IsActive && !s.IsDeleted && s.TagId == id).Any();
                bool isCableChkExist = _cableService.GetAll(s => s.IsActive && !s.IsDeleted && s.TagId == id).Any();
                if (isChkExist || isStandChkExist || isCableChkExist)
                    return new BaseResponse(false, ResponseMessages.TagNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError, tagDetails);

                tagDetails.IsDeleted = true;
                var response = _tagService.Update(tagDetails, tagDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, tagDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, tagDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkTags(List<Guid> ids)
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
                    var deleteResponse = await DeleteTag(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as Tag;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.TagName,
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
                        Message = $"Failed to delete tags.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted tags. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of tags have not been successfully deleted. \n" +
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
        public async Task<BaseResponse> GenerateTag(GenerateTagDto info)
        {
            List<string?> sixStrings = new List<string?> { info.Field1Id, info.Field2Id, info.Field3Id, info.Field4Id, info.Field5Id, info.Field6Id };
            List<ProjectTagFieldInfoDto> fieldSourceInfo = await _commonMethods.GetProjectTagFieldDataInfo(info.ProjectId);
            List<TagFieldSource> sourceInfo = new List<TagFieldSource>();
            string tag = string.Empty;
            foreach (var item in fieldSourceInfo.Select(s => s.Source).ToList())
            {
                TagFieldSource sourceEnum;
                if (Enum.TryParse(item, out sourceEnum))
                {
                    sourceInfo.Add(sourceEnum);
                }
            }
            for (int i = 0; i < sixStrings.Count(); ++i)
            {
                if (sixStrings[i] == null || "".Equals(sixStrings[i]))
                {
                    continue;
                }
                if (sourceInfo.ElementAt(i) == TagFieldSource.HandTyped)
                {
                    tag += sixStrings[i];
                }
                else
                {
                    Guid? id = !string.IsNullOrEmpty(sixStrings[i]) ? new Guid(sixStrings[i] ?? "") : Guid.Empty;
                    switch (sourceInfo.ElementAt(i))
                    {
                        case TagFieldSource.Process: tag += _processService.GetSingle(p => p.Id == id)?.ProcessName ?? ""; break;
                        case TagFieldSource.SubProcess: tag += _subProcessService.GetSingle(p => p.Id == id)?.SubProcessName ?? ""; break;
                        case TagFieldSource.Stream: tag += _streamService.GetSingle(p => p.Id == id)?.StreamName ?? ""; break;
                        case TagFieldSource.TagTypeId: tag += _tagTypeService.GetSingle(p => p.Id == id)?.Name ?? ""; break;
                        case TagFieldSource.Descriptor: tag += _tagDescriptorService.GetSingle(p => p.Id == id)?.Name ?? ""; break;
                        case TagFieldSource.EquipmentCode: tag += _equipmentCodeService.GetSingle(p => p.Id == id)?.Code ?? ""; break;
                    }
                }
                if (i < sixStrings.Count() - 1)
                {
                    tag += fieldSourceInfo.ElementAt(i)?.Separator ?? "";
                }
            }
            return new BaseResponse(true, ResponseMessages.GenerateTag, HttpStatusCode.NoContent, new
            {
                Tag = tag,
            });
        }

        [HttpGet]
        public async Task<List<DropdownInfoDto>> GetProjectWiseTagInfo(Guid projectId, string type, Guid? id)
        {
            return await _commonMethods.GetProjectWiseTagInfo(projectId, type, id);
        }

        private Guid? GetTagListValue(Tag tagInfo, string source)
        {
            TagFieldSource sourceEnum;
            if (Enum.TryParse(source, out sourceEnum))
            {
                switch (sourceEnum)
                {
                    case TagFieldSource.NotUsed: return null;
                    case TagFieldSource.Process: return tagInfo.ProcessId;
                    case TagFieldSource.SubProcess: return tagInfo.SubProcessId;
                    case TagFieldSource.Stream: return tagInfo.StreamId;
                    case TagFieldSource.HandTyped: return null;
                    case TagFieldSource.TagTypeId: return tagInfo.TagTypeId;
                    case TagFieldSource.Descriptor: return tagInfo.TagDescriptorId;
                    case TagFieldSource.EquipmentCode: return tagInfo.EquipmentCodeId;
                }
            }
            return null;
        }

        private static string? GetTagFieldValue(Tag tagInfo, ProjectTagFieldInfoDto fieldSourceInfo, string source, int fieldNumber)
        {
            if (fieldSourceInfo != null && fieldSourceInfo.FieldData != null && fieldSourceInfo.FieldData.Count > 0)
            {
                if (Enum.TryParse(source, out TagFieldSource sourceEnum))
                {
                    switch (sourceEnum)
                    {
                        case TagFieldSource.NotUsed: return string.Empty;
                        case TagFieldSource.Process: return (tagInfo.Process != null ? tagInfo.Process.ProcessName : string.Empty);
                        case TagFieldSource.SubProcess: return (tagInfo.SubProcess != null ? tagInfo.SubProcess.SubProcessName : string.Empty);
                        case TagFieldSource.Stream: return (tagInfo.Stream != null ? tagInfo.Stream.StreamName : string.Empty);
                        case TagFieldSource.HandTyped: return string.Empty;
                        case TagFieldSource.TagTypeId: return (tagInfo.TagType != null ? tagInfo.TagType.Name : string.Empty);
                        case TagFieldSource.Descriptor: return (tagInfo.TagDescriptor != null ? tagInfo.TagDescriptor.Name : string.Empty);
                        case TagFieldSource.EquipmentCode: return (tagInfo.EquipmentCode != null ? tagInfo.EquipmentCode.Code : string.Empty);
                    }
                }
            }
            else
            {
                return fieldNumber == 1 ? tagInfo.Field1String :
                       fieldNumber == 2 ? tagInfo.Field2String :
                       fieldNumber == 3 ? tagInfo.Field3String :
                       fieldNumber == 4 ? tagInfo.Field4String :
                       fieldNumber == 5 ? tagInfo.SequenceNumber :
                       fieldNumber == 6 ? tagInfo.EquipmentIdentifier :
                       string.Empty;
            }
            return null;
        }

        private async Task<Tag> SetTagFieldValue(Tag tagInfo, CreateOrEditTagDto info)
        {
            List<ProjectTagFieldInfoDto> fieldSourceInfo = await _commonMethods.GetProjectTagFieldDataInfo(info.ProjectId);
            return SetTagFields(tagInfo, info, fieldSourceInfo);
        }
        #endregion

        private Tag SetTagFields(Tag tagInfo, CreateOrEditTagDto info, List<ProjectTagFieldInfoDto> fieldSourceInfo)
        {
            List<string> tableSources = fieldSourceInfo.Select(s => s.Source ?? "").ToList();
            List<Guid?> idSources = new List<Guid?>
                {
                     info.Field1Id, info.Field2Id, info.Field3Id, info.Field4Id, info.Field5Id, info.Field6Id
                };


            for (int i = 0; i < tableSources.Count(); ++i)
            {
                TagFieldSource sourceEnum;
                if (Enum.TryParse(tableSources.ElementAt(i), out sourceEnum))
                {
                    switch (sourceEnum)
                    {
                        case TagFieldSource.Process:
                            tagInfo.ProcessId = idSources[i];
                            break;
                        case TagFieldSource.SubProcess:
                            tagInfo.SubProcessId = idSources[i];
                            break;
                        case TagFieldSource.Stream:
                            tagInfo.StreamId = idSources[i];
                            break;
                        case TagFieldSource.TagTypeId:
                            tagInfo.TagTypeId = idSources[i];
                            break;
                        case TagFieldSource.Descriptor:
                            tagInfo.TagDescriptorId = idSources[i];
                            break;
                        case TagFieldSource.EquipmentCode:
                            tagInfo.EquipmentCodeId = idSources[i];
                            break;
                    }
                }
            }
            return tagInfo;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<Dictionary<string, string>>> ImportTag([FromForm] FileUploadModel info)
        {
            List<Dictionary<string, string>> responseList = new();
            List<ImportLogDto> importLogs = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<ProjectTagFieldInfoDto> tagFieldInfoDtos = await _commonMethods.GetProjectTagFieldDataInfo(info.ProjectId);

            List<string> requiredKeys = [TagNameKey, .. tagFieldInfoDtos.Where(x => x.IsUsed).Select(x => x.Name!).ToList()];
            List<string> tempHeadingList = [];
            Dictionary<string, int> itemCounts = [];
            foreach (string item in requiredKeys)
            {
                if (!itemCounts.ContainsKey(item))
                {
                    itemCounts[item] = 1;
                    tempHeadingList.Add(item);
                }
                else
                {
                    int count = ++itemCounts[item];
                    tempHeadingList.Add($"{item}{count}");
                }
            }
            requiredKeys = tempHeadingList;

            var typeHeaders = _csvImport.ReadTagFile(TagNameKey, info.File, tagFieldInfoDtos.Where(x => x.IsUsed).ToList(), requiredKeys, out FileType fileType);

            if (fileType != FileType.Tags || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var isEditImport = false;
            if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault()!.Item1 == FileHeadingConstants.IdHeading)
                isEditImport = true;

            foreach (var columns in typeHeaders)
            {
                var dictionary = new List<Tuple<string, string, Guid?>>();
                var editId = Guid.Empty;

                foreach (var item in columns)
                {
                    if (item.Item1 == FileHeadingConstants.IdHeading)
                    {
                        var isSuccess = Guid.TryParse(item.Item2, out editId);
                        if (!isSuccess)
                            editId = Guid.Empty;

                        continue;
                    }

                    dictionary.Add(Tuple.Create(item.Item1, item.Item2, item.Item3));
                }

                var keys = dictionary.Select(x => x.Item1).ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    ImportLogDto importLog = new()
                    {
                        Operation = OperationType.Insert
                    };

                    Tag createDto = new();
                    bool isSuccess = false;
                    List<string> message = [];

                    Dictionary<string, string> Records = [];
                    bool isUpdate = false;
                    try
                    {
                        int index = 1;
                        Dictionary<string, int> sameColumnCount = [];
                        List<Tuple<string, string, TagFieldSource>> dataRecords = new List<Tuple<string, string, TagFieldSource>>();
                        foreach (var item in dictionary.Where(x => x.Item3 != null && x.Item3 != Guid.Empty))
                        {
                            ProjectTagFieldInfoDto? fieldInfoDto = tagFieldInfoDtos.Find(x => x.Id == item.Item3);
                            if (fieldInfoDto != null)
                            {
                                if (fieldInfoDto.FieldData != null && Enum.TryParse(fieldInfoDto.Source, out TagFieldSource sourceEnum))
                                {
                                    bool notExist = false;
                                    switch (sourceEnum)
                                    {
                                        case TagFieldSource.Process:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Process));
                                            Process? processType = await _processService.GetSingleAsync(x => x.ProcessName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.ProcessId = processType?.Id;
                                            if (processType == null) notExist = true;
                                            break;

                                        case TagFieldSource.SubProcess:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.SubProcess));
                                            SubProcess? subProcessType = await _subProcessService.GetSingleAsync(x => x.SubProcessName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.SubProcessId = subProcessType?.Id;
                                            if (subProcessType == null) notExist = true;
                                            break;

                                        case TagFieldSource.Stream:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Stream));
                                            ICMD.Core.DBModels.Stream? streamType = await _streamService.GetSingleAsync(x => x.StreamName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.StreamId = streamType?.Id;
                                            if (streamType == null) notExist = true;
                                            break;

                                        case TagFieldSource.TagTypeId:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.TagTypeId));
                                            TagType? tagType = await _tagTypeService.GetSingleAsync(x => x.Name == item.Item2 && !x.IsDeleted);
                                            createDto.TagTypeId = tagType?.Id;
                                            if (tagType == null) notExist = true;
                                            break;

                                        case TagFieldSource.EquipmentCode:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.EquipmentCode));
                                            EquipmentCode? equipmentCode = await _equipmentCodeService.GetSingleAsync(x => x.Code == item.Item2 && !x.IsDeleted);
                                            createDto.EquipmentCodeId = equipmentCode?.Id;
                                            if (equipmentCode == null) notExist = true;
                                            break;

                                        case TagFieldSource.Descriptor:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Descriptor));
                                            TagDescriptor? tagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name == item.Item2 && !x.IsDeleted);
                                            createDto.TagDescriptorId = tagDescriptor?.Id;
                                            if (tagDescriptor == null) notExist = true;
                                            break;
                                    }

                                    if (notExist)
                                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", item.Item1));
                                }
                                else
                                {
                                    if (index == 1)
                                        createDto.Field1String = item.Item2;
                                    else if (index == 2)
                                        createDto.Field2String = item.Item2;
                                    else if (index == 3)
                                        createDto.Field3String = item.Item2;
                                    else if (index == 4)
                                        createDto.Field4String = item.Item2;
                                    else if (index == 5)
                                        createDto.SequenceNumber = item.Item2;
                                    else if (index == 6)
                                        createDto.EquipmentIdentifier = item.Item2;
                                }

                                if (!Records.Any(item => item.Key == fieldInfoDto.Name!))
                                {
                                    sameColumnCount[fieldInfoDto.Name!] = 1;
                                    Records.Add(fieldInfoDto.Name!, item.Item2);
                                }
                                else
                                {
                                    int count = ++sameColumnCount[fieldInfoDto.Name!];
                                    Records.Add($"{fieldInfoDto.Name!}{count}", item.Item2);
                                }
                            }

                            index++;
                        }

                        CommonHelper helper = new();
                        Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                        isSuccess = validationResponse.Item1;
                        if (isEditImport && editId == Guid.Empty)
                        {
                            isSuccess = false;
                            message.Add("Id is incorrect format.");
                        }

                        string tagName = dictionary.FirstOrDefault(x => x.Item1 == TagNameKey)?.Item2 ?? string.Empty;

                        createDto.TagName = tagName;
                        importLog.Name = createDto.TagName;
                        Records.Add(TagNameKey, tagName);

                        if (isSuccess)
                        {
                            createDto.ProjectId = info.ProjectId;
                            createDto.Id = Guid.Empty;

                            if (message.Count == 0)
                            {
                                Tag existingTag;
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    importLog.Operation = OperationType.Edit;
                                    existingTag = await _tagService.GetSingleAsync(x => x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingTag == null)
                                    {
                                        message.Add("Record is not found.");
                                        importLog.Items = GetChanges(new(), createDto, dataRecords);
                                    }
                                    else
                                    {
                                        var existingRecordName = await _tagService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.TagName.ToLower().Trim() == tagName.ToLower().Trim() &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Tag Name is already taken.");
                                            importLog.Items = GetChanges(existingTag, createDto, dataRecords);
                                        }
                                    }
                                }
                                else
                                {
                                    existingTag = await _tagService.GetSingleAsync(x => x.TagName.ToLower().Trim() == tagName.ToLower().Trim() && x.IsActive && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                }

                                if (existingTag != null)
                                {
                                    isUpdate = true;
                                    createDto.Id = existingTag.Id;
                                    createDto.CreatedBy = existingTag.CreatedBy;
                                    createDto.CreatedDate = existingTag.CreatedDate;

                                    importLog.Operation = OperationType.Edit;
                                    importLog.Items = GetChanges(existingTag, createDto, dataRecords);

                                    var response = _tagService.Update(createDto, existingTag, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    importLog.Items = GetChanges(new(), createDto, dataRecords);

                                    var response = await _tagService.AddAsync(createDto, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName));
                                }
                            }
                        }
                        else
                        {
                            message.AddRange(validationResponse.Item2);
                            importLog.Items = GetChanges(new(), createDto, dataRecords);
                        }
                    }
                    catch (Exception ex)
                    {
                        message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", ModuleName));
                    }


                    Records.Add("Status", message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success);
                    Records.Add("Message", string.Join(", ", message));
                    responseList.Add(Records);

                    importLog.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    importLog.Message = string.Join(", ", message);
                    importLogs.Add(importLog);
                }
            }

            // Record logs
            await _changeLogHelper.CreateImportLogs(ModuleName, importLogs);

            if (responseList.All(x => x.Where(p => p.Key == "Status")
                .All(p => p.Key == ImportFileRecordStatus.Success)))
            {
                return new()
                {
                    IsSucceeded = true,
                    Headers = requiredKeys,
                    Message = ResponseMessages.ImportFile,
                    Records = responseList
                };
            }
            else if (responseList.All(x => x.Where(p => p.Key == "Status")
                .All(p => p.Key == ImportFileRecordStatus.Fail)))
            {

                return new()
                {
                    IsSucceeded = false,
                    Headers = requiredKeys,
                    Message = ResponseMessages.FailedImportFile,
                    Records = responseList
                };
            }

            return new()
            {
                IsSucceeded = true,
                IsWarning = true,
                Headers = requiredKeys,
                Message = ResponseMessages.SomeFailedImportFile,
                Records = responseList
            };
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportTag([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<ProjectTagFieldInfoDto> tagFieldInfoDtos = await _commonMethods.GetProjectTagFieldDataInfo(info.ProjectId);

            List<string> requiredKeys = [TagNameKey, .. tagFieldInfoDtos.Where(x => x.IsUsed).Select(x => x.Name!).ToList()];
            List<string> tempHeadingList = [];
            Dictionary<string, int> itemCounts = [];
            foreach (string item in requiredKeys)
            {
                if (!itemCounts.ContainsKey(item))
                {
                    itemCounts[item] = 1;
                    tempHeadingList.Add(item);
                }
                else
                {
                    int count = ++itemCounts[item];
                    tempHeadingList.Add($"{item}{count}");
                }
            }
            requiredKeys = tempHeadingList;

            var typeHeaders = _csvImport.ReadTagFile(TagNameKey, info.File, tagFieldInfoDtos.Where(x => x.IsUsed).ToList(), requiredKeys, out FileType fileType);

            if (fileType != FileType.Tags || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var transaction = await _tagService.BeginTransaction();

            var isEditImport = false;
            if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault()!.Item1 == FileHeadingConstants.IdHeading)
                isEditImport = true;

            foreach (var columns in typeHeaders)
            {
                var dictionary = new List<Tuple<string, string, Guid?>>();
                var editId = Guid.Empty;

                foreach (var item in columns)
                {
                    if (item.Item1 == FileHeadingConstants.IdHeading)
                    {
                        var isSuccess = Guid.TryParse(item.Item2, out editId);
                        if (!isSuccess)
                            editId = Guid.Empty;

                        continue;
                    }

                    dictionary.Add(Tuple.Create(item.Item1, item.Item2, item.Item3));
                }

                var keys = dictionary.Select(x => x.Item1).ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    ValidationDataDto validationData = new()
                    {
                        Operation = OperationType.Insert
                    };

                    Tag createDto = new();
                    bool isSuccess = false;
                    List<string> message = [];

                    Dictionary<string, string> Records = [];
                    bool isUpdate = false;
                    try
                    {
                        int index = 1;
                        Dictionary<string, int> sameColumnCount = [];
                        List<Tuple<string, string, TagFieldSource>> dataRecords = new List<Tuple<string, string, TagFieldSource>>();
                        
                        foreach (var item in dictionary.Where(x => x.Item3 != null && x.Item3 != Guid.Empty))
                        {
                            ProjectTagFieldInfoDto? fieldInfoDto = tagFieldInfoDtos.Find(x => x.Id == item.Item3);
                            if (fieldInfoDto != null)
                            {
                                if (fieldInfoDto.FieldData != null && Enum.TryParse(fieldInfoDto.Source, out TagFieldSource sourceEnum))
                                {
                                    bool notExist = false;
                                    switch (sourceEnum)
                                    {
                                        case TagFieldSource.Process:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Process));
                                            Process? processType = await _processService.GetSingleAsync(x => x.ProcessName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.ProcessId = processType?.Id;
                                            if (processType == null) notExist = true;
                                            break;

                                        case TagFieldSource.SubProcess:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.SubProcess));
                                            SubProcess? subProcessType = await _subProcessService.GetSingleAsync(x => x.SubProcessName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.SubProcessId = subProcessType?.Id;
                                            if (subProcessType == null) notExist = true;
                                            break;

                                        case TagFieldSource.Stream:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Stream));
                                            ICMD.Core.DBModels.Stream? streamType = await _streamService.GetSingleAsync(x => x.StreamName == item.Item2 && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                            createDto.StreamId = streamType?.Id;
                                            if (streamType == null) notExist = true;
                                            break;

                                        case TagFieldSource.TagTypeId:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.TagTypeId));
                                            TagType? tagType = await _tagTypeService.GetSingleAsync(x => x.Name == item.Item2 && !x.IsDeleted);
                                            createDto.TagTypeId = tagType?.Id;
                                            if (tagType == null) notExist = true;
                                            break;

                                        case TagFieldSource.EquipmentCode:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.EquipmentCode));
                                            EquipmentCode? equipmentCode = await _equipmentCodeService.GetSingleAsync(x => x.Code == item.Item2 && !x.IsDeleted);
                                            createDto.EquipmentCodeId = equipmentCode?.Id;
                                            if (equipmentCode == null) notExist = true;
                                            break;

                                        case TagFieldSource.Descriptor:
                                            dataRecords.Add(Tuple.Create(fieldInfoDto.Name!, item.Item2, TagFieldSource.Descriptor));
                                            TagDescriptor? tagDescriptor = await _tagDescriptorService.GetSingleAsync(x => x.Name == item.Item2 && !x.IsDeleted);
                                            createDto.TagDescriptorId = tagDescriptor?.Id;
                                            if (tagDescriptor == null) notExist = true;
                                            break;
                                    }

                                    if (notExist)
                                        message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", item.Item1));
                                }
                                else
                                {
                                    if (index == 1)
                                        createDto.Field1String = item.Item2;
                                    else if (index == 2)
                                        createDto.Field2String = item.Item2;
                                    else if (index == 3)
                                        createDto.Field3String = item.Item2;
                                    else if (index == 4)
                                        createDto.Field4String = item.Item2;
                                    else if (index == 5)
                                        createDto.SequenceNumber = item.Item2;
                                    else if (index == 6)
                                        createDto.EquipmentIdentifier = item.Item2;
                                }

                                if (!Records.Any(item => item.Key == fieldInfoDto.Name!))
                                {
                                    sameColumnCount[fieldInfoDto.Name!] = 1;
                                    Records.Add(fieldInfoDto.Name!, item.Item2);
                                }
                                else
                                {
                                    int count = ++sameColumnCount[fieldInfoDto.Name!];
                                    Records.Add($"{fieldInfoDto.Name!}{count}", item.Item2);
                                }
                            }

                            index++;
                        }

                        CommonHelper helper = new();
                        Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                        isSuccess = validationResponse.Item1;

                        if (isEditImport && editId == Guid.Empty)
                        {
                            isSuccess = false;
                            message.Add("Id is incorrect format.");
                        }

                        string tagName = dictionary.FirstOrDefault(x => x.Item1 == TagNameKey)?.Item2 ?? string.Empty;

                        createDto.TagName = tagName;
                        validationData.Name = createDto.TagName;

                        Records.Add(TagNameKey, tagName);
                        if (isSuccess)
                        {
                            createDto.ProjectId = info.ProjectId;
                            createDto.Id = Guid.Empty;

                            if (message.Count == 0)
                            {
                                Tag existingTag;
                                if (isEditImport && editId != Guid.Empty)
                                {
                                    validationData.Operation = OperationType.Edit;
                                    existingTag = await _tagService.GetSingleAsync(x => x.Id == editId &&
                                        !x.IsDeleted && x.IsActive);
                                    if (existingTag == null)
                                    {
                                        message.Add("Record is not found.");
                                        validationData.Changes = GetChanges(new(), createDto, dataRecords);
                                    }
                                    else
                                    {
                                        var existingRecordName = await _tagService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.TagName.ToLower().Trim() == tagName.ToLower().Trim() &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingRecordName != null)
                                        {
                                            message.Add("Tag Name is already taken.");
                                            validationData.Changes = GetChanges(existingTag, createDto, dataRecords);
                                        }
                                    }
                                }
                                else
                                {
                                    existingTag = await _tagService.GetSingleAsync(x => x.TagName.ToLower().Trim() == tagName.ToLower().Trim() && x.IsActive && !x.IsDeleted && x.ProjectId == info.ProjectId);
                                }
                                if (existingTag != null)
                                {
                                    isUpdate = true;
                                    createDto.Id = existingTag.Id;
                                    createDto.CreatedBy = existingTag.CreatedBy;
                                    createDto.CreatedDate = existingTag.CreatedDate;

                                    validationData.Operation = OperationType.Edit;
                                    validationData.Changes = GetChanges(existingTag, createDto, dataRecords);

                                    var response = _tagService.Update(createDto, existingTag, User.GetUserId());

                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    validationData.Changes = GetChanges(new (), createDto, dataRecords);

                                    var response = await _tagService.AddAsync(createDto, User.GetUserId());
                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName));
                                }
                            }
                        }
                        else
                        {
                            message.AddRange(validationResponse.Item2);
                            validationData.Changes = GetChanges(new(), createDto, dataRecords);
                        }
                    }
                    catch (Exception ex)
                    {
                        message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", ModuleName));
                    }

                    validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                    validationData.Message = string.Join(", ", message);
                    validationDataList.Add(validationData);
                }
            }
            await _tagService.RollbackTransaction(transaction);

            return new()
            {
                IsSucceeded = true,
                Headers = requiredKeys,
                Message = ResponseMessages.ImportFile,
                Records = validationDataList
            };
        }

        private List<ChangesDto> GetChanges(Tag entity, Tag createDto, List<Tuple<string, string, TagFieldSource>> newDataRecords)
        {
            var changes = new List<ChangesDto>
            {
                new()
                {
                    ItemColumnName = nameof(entity.TagName),
                    NewValue = createDto.TagName,
                    PreviousValue = entity.Id != Guid.Empty ? entity.TagName : string.Empty,
                }
            };

            // Items coming from dynamic columns
            foreach (var records in newDataRecords)
            {
                var previousData = string.Empty;
                if (records.Item3 == TagFieldSource.Process)
                    previousData = entity.Process?.ProcessName;
                else if (records.Item3 == TagFieldSource.SubProcess)
                    previousData = entity.SubProcess?.SubProcessName;
                else if (records.Item3 == TagFieldSource.Stream)
                    previousData = entity.Stream?.StreamName;
                else if (records.Item3 == TagFieldSource.TagTypeId)
                    previousData = entity.TagType?.Name;
                else if (records.Item3 == TagFieldSource.Descriptor)
                    previousData = entity.TagDescriptor?.Name;

                changes.Add(new()
                {
                    ItemColumnName = records.Item1,
                    PreviousValue = previousData ?? string.Empty,
                    NewValue = records.Item2
                });
            }

            if (createDto.Field1String != null)
            {
                changes.Add(new()
                {
                    ItemColumnName = nameof(entity.Field1String),
                    NewValue = createDto.Field1String,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Field1String ?? string.Empty : string.Empty,
                });
            };

            if (createDto.Field2String != null)
            {
                changes.Add(new()
                {
                    ItemColumnName = nameof(entity.Field2String),
                    NewValue = createDto.Field2String,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Field2String ?? string.Empty : string.Empty,
                });
            };

            if (createDto.Field3String != null)
            {
                changes.Add(new()
                {
                    ItemColumnName = nameof(entity.Field3String),
                    NewValue = createDto.Field3String,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Field3String ?? string.Empty : string.Empty,
                });
            };
            if (createDto.Field4String != null)
            {
                changes.Add(new()
                {
                    ItemColumnName = nameof(entity.Field4String),
                    NewValue = createDto.Field4String,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Field4String ?? string.Empty : string.Empty,
                });
            };
            return changes;
        }
    }
}
