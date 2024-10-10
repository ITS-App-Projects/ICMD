using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.Dtos.Stream;
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
    public class StreamController : BaseController
    {
        private readonly IStreamService _streamService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private static string ModuleName = "Stream";
        private readonly CSVImport _csvImport;
        public StreamController(IMapper mapper, IStreamService streamService, ITagService tagService, CSVImport csvImport)
        {
            _streamService = streamService;
            _mapper = mapper;
            _tagService = tagService;
            _csvImport = csvImport;
        }

        #region Stream
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<StreamInfoDto>> GetAllStreams(PagedAndSortedResultRequestDto input)
        {
            IQueryable<StreamInfoDto> allStreams = _streamService.GetAll(s => !s.IsDeleted).Select(s => new StreamInfoDto
            {
                Id = s.Id,
                StreamName = s.StreamName,
                Description = s.Description,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allStreams = allStreams.Where(s => (!string.IsNullOrEmpty(s.StreamName) && s.StreamName.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allStreams = allStreams.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allStreams = allStreams.Where(input.SearchColumnFilterQuery);

            allStreams = allStreams.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<StreamInfoDto> paginatedData = !isExport ? allStreams.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allStreams;


            return new PagedResultDto<StreamInfoDto>(
               allStreams.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<StreamInfoDto?> GetStreamInfo(Guid id)
        {
            Core.DBModels.Stream? streamDetails = await _streamService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (streamDetails != null)
            {
                return _mapper.Map<StreamInfoDto>(streamDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditStream(CreateOrEditStreamDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateStream(info);
            }
            else
            {
                return await UpdateStream(info);
            }
        }

        private async Task<BaseResponse> CreateStream(CreateOrEditStreamDto info)
        {
            if (ModelState.IsValid)
            {
                Core.DBModels.Stream existingStream = await _streamService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.StreamName.ToLower().Trim() == info.StreamName.ToLower().Trim() && !x.IsDeleted);
                if (existingStream != null)
                    return new BaseResponse(false, ResponseMessages.StreamNameAlreadyTaken, HttpStatusCode.Conflict);

                Core.DBModels.Stream streamInfo = _mapper.Map<Core.DBModels.Stream>(info);
                streamInfo.IsActive = true;
                var response = await _streamService.AddAsync(streamInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateStream(CreateOrEditStreamDto info)
        {
            if (ModelState.IsValid)
            {
                Core.DBModels.Stream streamDetails = await _streamService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (streamDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Core.DBModels.Stream existingStream = await _streamService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.StreamName.ToLower().Trim() == info.StreamName.ToLower().Trim() && !x.IsDeleted);
                if (existingStream != null)
                    return new BaseResponse(false, ResponseMessages.StreamNameAlreadyTaken, HttpStatusCode.Conflict);

                Core.DBModels.Stream streamInfo = _mapper.Map<Core.DBModels.Stream>(info);
                streamInfo.CreatedBy = streamDetails.CreatedBy;
                streamInfo.CreatedDate = streamDetails.CreatedDate;
                streamInfo.IsActive = streamDetails.IsActive;
                var response = _streamService.Update(streamInfo, streamDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteStream(Guid id)
        {
            Core.DBModels.Stream streamDetail = await _streamService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (streamDetail != null)
            {
                bool isChkExist = _tagService.GetAll(s => s.IsActive && !s.IsDeleted && s.StreamId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                streamDetail.IsDeleted = true;
                var response = _streamService.Update(streamDetail, streamDetail, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<StreamInfoDto>> ImportStream([FromForm] FileUploadModel info)
        {
            List<StreamInfoDto> responseList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.TagField3 || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.TagField3Headings;

            foreach (var dictionary in typeHeaders)
            {
                var keys = dictionary.Keys.ToList();
                if (requiredKeys.All(keys.Contains))
                {
                    bool isSuccess = false;
                    List<string> message = [];

                    CreateOrEditStreamDto createDto = new()
                    {
                        StreamName = dictionary[requiredKeys[0]],
                        Description = dictionary[requiredKeys[1]],
                        ProjectId = info.ProjectId,
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
                            Core.DBModels.Stream existingStream = await _streamService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.StreamName.ToLower().Trim() == createDto.StreamName.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                            if (message.Count == 0)
                            {
                                Core.DBModels.Stream streamInfo = _mapper.Map<Core.DBModels.Stream>(createDto);
                                streamInfo.ProjectId = info.ProjectId;
                                
                                if (existingStream != null)
                                {
                                    streamInfo.Id = existingStream.Id;
                                    streamInfo.CreatedBy = existingStream.CreatedBy;
                                    streamInfo.CreatedDate = existingStream.CreatedDate;
                                    var response = _streamService.Update(streamInfo, existingStream, User.GetUserId());

                                    if (response == null)
                                        message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                }
                                else
                                {
                                    var response = await _streamService.AddAsync(streamInfo, User.GetUserId());

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

                    StreamInfoDto record = _mapper.Map<StreamInfoDto>(createDto);
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
