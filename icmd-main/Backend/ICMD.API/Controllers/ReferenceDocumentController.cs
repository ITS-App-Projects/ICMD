using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Reference_Document;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using ICMD.Core.Dtos;
using ICMD.API.Helpers;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReferenceDocumentController : BaseController
    {
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IPanelService _panelService;
        private readonly ISkidService _skidService;
        private readonly IStandService _standService;
        private readonly IJunctionBoxService _junctionBoxService;
        private readonly IReferenceDocumentDeviceService _referenceDocumentDeviceService;
        private readonly IMapper _mapper;
        private readonly CommonMethods _commonMethods;
        private static string ModuleName = "Reference document";
        private readonly CSVImport _csvImport;
        private readonly IReferenceDocumentTypeService _referenceDocumentTypeService;
        public ReferenceDocumentController(IMapper mapper, IReferenceDocumentService referenceDocumentService, IPanelService panelService, ISkidService skidService, IStandService standService, IJunctionBoxService junctionBoxService,
            IReferenceDocumentDeviceService referenceDocumentDeviceService, CommonMethods commonMethods, CSVImport csvImport, IReferenceDocumentTypeService referenceDocumentTypeService)
        {
            _referenceDocumentService = referenceDocumentService;
            _mapper = mapper;
            _panelService = panelService;
            _skidService = skidService;
            _standService = standService;
            _junctionBoxService = junctionBoxService;
            _referenceDocumentDeviceService = referenceDocumentDeviceService;
            _commonMethods = commonMethods;
            _csvImport = csvImport;
            _referenceDocumentTypeService = referenceDocumentTypeService;
        }

        #region ReferenceDocument
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ReferenceDocumentInfoDto>> GetAllReferenceDocument(PagedAndSortedResultRequestDto input)
        {
            IQueryable<ReferenceDocumentInfoDto> allDocuments = _referenceDocumentService.GetAll(s => !s.IsDeleted).Select(s => new ReferenceDocumentInfoDto
            {
                Id = s.Id,
                DocumentNumber = s.DocumentNumber,
                Description = s.Description ?? "",
                URL = s.URL ?? "",
                Revision = s.Revision ?? "",
                Version = s.Version ?? "",
                Sheet = s.Sheet ?? "",
                Date = s.Date.HasValue ? s.Date.Value.ToString("MM/dd/yyyy") : string.Empty,
                ProjectId = s.ProjectId,
                ReferenceDocumentTypeId = s.ReferenceDocumentTypeId,
                ReferenceDocumentType = s.ReferenceDocumentType != null ? s.ReferenceDocumentType.Type : ""
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allDocuments = allDocuments.Where(s => (!string.IsNullOrEmpty(s.DocumentNumber) && s.DocumentNumber.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.ReferenceDocumentType) && s.ReferenceDocumentType.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.URL) && s.URL.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Version) && s.Version.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Revision) && s.Revision.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Sheet) && s.Sheet.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "referenceDocumentTypeId".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allDocuments = allDocuments.Where(x => ids != null && ids.Contains(x.ReferenceDocumentTypeId.ToString()));
                    }

                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allDocuments = allDocuments.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allDocuments = allDocuments.Where(input.SearchColumnFilterQuery);

            allDocuments = allDocuments.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<ReferenceDocumentInfoDto> paginatedData = !isExport ? allDocuments.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allDocuments;


            return new PagedResultDto<ReferenceDocumentInfoDto>(
               allDocuments.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<ReferenceDocumentInfoDto?> GetReferenceDocumentInfo(Guid id)
        {
            ReferenceDocument? documentDetails = await _referenceDocumentService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (documentDetails != null)
            {
                ReferenceDocumentInfoDto documentInfo = _mapper.Map<ReferenceDocumentInfoDto>(documentDetails);
                documentInfo.ReferenceDocumentType = documentDetails.ReferenceDocumentType != null ? documentDetails.ReferenceDocumentType.Type : "";
                return documentInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditReferenceDocument(CreateOrEditReferenceDocumentDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateReferenceDocument(info);
            }
            else
            {
                return await UpdateReferenceDocument(info);
            }
        }

        private async Task<BaseResponse> CreateReferenceDocument(CreateOrEditReferenceDocumentDto info)
        {
            if (ModelState.IsValid)
            {
                ReferenceDocument existingDocument = await _referenceDocumentService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.ReferenceDocumentTypeId == info.ReferenceDocumentTypeId && x.DocumentNumber.ToLower().Trim() == info.DocumentNumber.ToLower().Trim() && !x.IsDeleted);
                if (existingDocument != null)
                    return new BaseResponse(false, ResponseMessages.DocumentNumberAlreadyTaken, HttpStatusCode.Conflict);

                ReferenceDocument documentInfo = _mapper.Map<ReferenceDocument>(info);
                documentInfo.IsActive = true;
                var response = await _referenceDocumentService.AddAsync(documentInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateReferenceDocument(CreateOrEditReferenceDocumentDto info)
        {
            if (ModelState.IsValid)
            {
                ReferenceDocument documentDetails = await _referenceDocumentService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (documentDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                ReferenceDocument existingDocument = await _referenceDocumentService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.ReferenceDocumentTypeId == info.ReferenceDocumentTypeId && x.Id != info.Id && x.DocumentNumber.ToLower().Trim() == info.DocumentNumber.ToLower().Trim() && !x.IsDeleted);
                if (existingDocument != null)
                    return new BaseResponse(false, ResponseMessages.DocumentNumberAlreadyTaken, HttpStatusCode.Conflict);

                ReferenceDocument documentInfo = _mapper.Map<ReferenceDocument>(info);
                documentInfo.CreatedBy = documentDetails.CreatedBy;
                documentInfo.CreatedDate = documentDetails.CreatedDate;
                documentInfo.IsActive = documentDetails.IsActive;
                var response = _referenceDocumentService.Update(documentInfo, documentDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteReferenceDocument(Guid id)
        {
            ReferenceDocument documentDetail = await _referenceDocumentService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (documentDetail != null)
            {
                bool isPanelExist = _panelService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentId == id).Any();
                bool isSkidExist = _skidService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentId == id).Any();
                bool isStandExist = _standService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentId == id).Any();
                bool isJunctionExist = _junctionBoxService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentId == id).Any();
                bool isDeviceExist = _referenceDocumentDeviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.ReferenceDocumentId == id).Any();
                if (isPanelExist || isSkidExist || isStandExist || isJunctionExist || isDeviceExist)
                    return new BaseResponse(false, ResponseMessages.AlreadyUsedNotDelete.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                documentDetail.IsDeleted = true;
                var response = _referenceDocumentService.Update(documentDetail, documentDetail, User.GetUserId(), true, true);
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
        public async Task<List<DropdownInfoDto>> GetAllDocumentInfo(Guid projectId, Guid referenceDocumentTypeId)
        {
            List<DropdownInfoDto> allTypeInfo = await _referenceDocumentService.GetAll(s => s.ProjectId == projectId && s.ReferenceDocumentTypeId == referenceDocumentTypeId && s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.DocumentNumber)
                .ThenBy(s => s.Version)
                .ThenBy(s => s.Revision)
                .ThenBy(s => s.Sheet)
                .Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = _commonMethods.GenerateFullReportName(s),
                }).ToListAsync();

            return allTypeInfo;
        }
        #endregion

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ReferenceDocumentInfoDto>> ImportReferenceDocument([FromForm] FileUploadModel info)
        {
            List<ReferenceDocumentInfoDto> responseList = [];
            if (!(info.File != null && info.File.Length > 0))
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
            if (fileType != FileType.ReferenceDocument || typeHeaders == null)
                return new() { Message = ResponseMessages.GlobalModelValidationMessage };

            List<string> requiredKeys = FileHeadingConstants.ReferenceDocumentHeadings;
            List<string> requiredExportFormatKeys = FileHeadingConstants.ReferenceDocumentExportHeadings;

            foreach (var dictionary in typeHeaders)
            {
                try
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredExportFormatKeys.All(keys.Contains))
                    {
                        requiredKeys = requiredExportFormatKeys;
                    }

                    if (requiredKeys.All(keys.Contains) || requiredExportFormatKeys.All(keys.Contains))
                    {
                        bool isSuccess = false;
                        List<string> message = [];

                        string? referenceDocumentTypeName = dictionary[requiredKeys[1]];
                        ReferenceDocumentType? referenceDocumentType = !string.IsNullOrEmpty(referenceDocumentTypeName) ? await _referenceDocumentTypeService.GetSingleAsync(x => x.Type == referenceDocumentTypeName && !x.IsDeleted && x.IsActive) : null;

                        CreateOrEditReferenceDocumentDto createDto = new()
                        {
                            ReferenceDocumentTypeId = referenceDocumentType?.Id ?? Guid.Empty,
                            DocumentNumber = dictionary[requiredKeys[0]],
                            URL = dictionary[requiredKeys[2]],
                            Description = dictionary[requiredKeys[3]],
                            Version = dictionary[requiredKeys[4]],
                            Revision = dictionary[requiredKeys[5]],
                            Date = dictionary[requiredKeys[6]],
                            Sheet = dictionary[requiredKeys[7]],
                            ProjectId = info.ProjectId,
                            Id = Guid.Empty
                        };

                        CommonHelper helper = new();
                        Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                        isSuccess = validationResponse.Item1;
                        if (!isSuccess) message.AddRange(validationResponse.Item2);

                        if (referenceDocumentType == null)
                        {
                            message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "reference document type"));
                            if (isSuccess) isSuccess = false;
                        }

                        DateTime? documentDate = null;
                        if (!string.IsNullOrEmpty(createDto.Date))
                        {
                            string[] formats = ["MM/dd/yyyy", "M/dd/yyyy", "M/dd/yy", "MM/dd/yy", "MM/dd/yyyy hh:mm:ss tt"];

                            if (DateTime.TryParseExact(createDto.Date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime myDate))
                            {
                                documentDate = myDate;
                                createDto.Date = documentDate.ToString();
                            }
                            else if (DateTime.TryParse(createDto.Date, out myDate))
                            {
                                documentDate = myDate;
                                createDto.Date = documentDate.ToString();
                            }
                            if (!isSuccess)
                            {
                                message.AddRange(validationResponse.Item2);
                            }
                            else if (documentDate == null)
                            {
                                message.Add(ResponseMessages.DateIsNotValid.Replace("{module}", createDto.Date));
                                if (isSuccess) isSuccess = false;
                            }

                        }

                        if (isSuccess)
                        {
                            bool isUpdate = false;
                            try
                            {
                                ReferenceDocument existingDocument = await _referenceDocumentService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.ReferenceDocumentTypeId == createDto.ReferenceDocumentTypeId && x.DocumentNumber.ToLower().Trim() == createDto.DocumentNumber.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                if (message.Count == 0)
                                {
                                    ReferenceDocument model = _mapper.Map<ReferenceDocument>(createDto);
                                    model.Date = documentDate;
                                    model.ProjectId = info.ProjectId;
                                    model.ReferenceDocumentTypeId = referenceDocumentType?.Id ?? Guid.Empty;
                                    if (existingDocument != null)
                                    {
                                        model.Id = existingDocument.Id;
                                        model.CreatedBy = existingDocument.CreatedBy;
                                        model.CreatedDate = existingDocument.CreatedDate;
                                        var response = _referenceDocumentService.Update(model, existingDocument, User.GetUserId());

                                        if (response == null)
                                            message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                    }
                                    else
                                    {
                                        var response = await _referenceDocumentService.AddAsync(model, User.GetUserId());

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

                        ReferenceDocumentInfoDto record = _mapper.Map<ReferenceDocumentInfoDto>(createDto);
                        record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                        record.Message = string.Join(", ", message);
                        record.ReferenceDocumentType = referenceDocumentTypeName;
                        responseList.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    throw;
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
