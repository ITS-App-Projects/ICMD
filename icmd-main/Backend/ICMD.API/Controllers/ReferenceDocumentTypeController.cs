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
using ICMD.Core.Dtos.ReferenceDocumentType;
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
    public class ReferenceDocumentTypeController : BaseController
    {
        private readonly IReferenceDocumentTypeService _referenceDocumentTypeService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Document type";

        private readonly ChangeLogHelper _changeLogHelper;

        public ReferenceDocumentTypeController(IMapper mapper, IReferenceDocumentTypeService referenceDocumentTypeService, IReferenceDocumentService referenceDocumentService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _referenceDocumentTypeService = referenceDocumentTypeService;
            _mapper = mapper;
            _referenceDocumentService = referenceDocumentService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
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
                    return new BaseResponse(false, ResponseMessages.TypeNotDeleteAlreadyAssigned, HttpStatusCode.InternalServerError, typeDetails);

                typeDetails.IsDeleted = true;
                var response = _referenceDocumentTypeService.Update(typeDetails, typeDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, typeDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, typeDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkDocumentTypes(List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return new BaseResponse(false, "Empty record was provided", HttpStatusCode.BadRequest);
                }

                List<BaseResponse> result = new List<BaseResponse>();
                List<BulkDeleteLogDto> bulkLog = [];
                foreach (var id in ids)
                {
                    var deleteResponse = await DeleteDocumentType(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as ReferenceDocumentType;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Type,
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
                        Message = $"Failed to delete reference document types.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted reference document types. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records are of reference document types have not been successfully deleted. \n" +
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
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.ReferenceDocumentType && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.ReferenceDocumentTypeHeadings;

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

                            CreateOrEditReferenceDocumentType createDto = new()
                            {
                                Type = dictionary[requiredKeys[0]],
                                Id = Guid.Empty
                            };
                            var importLog = new ImportLogDto
                            {
                                Name = createDto.Type,
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
                                    ReferenceDocumentType existingType;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingType == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _referenceDocumentTypeService.GetSingleAsync(x => x.Id != editId &&
                                                x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Type is already taken.");
                                                importLog.Items = GetChanges(existingType, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }
                                    if (message.Count == 0)
                                    {
                                        if (existingType != null)
                                        {
                                            isUpdate = true;
                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingType, createDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingType.Type = createDto.Type;

                                            var response = _referenceDocumentTypeService.Update(existingType, existingType, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            ReferenceDocumentType model = _mapper.Map<ReferenceDocumentType>(createDto);
                                            importLog.Items = GetChanges(model, createDto);
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
                            {
                                message.AddRange(validationResponse.Item2);
                                importLog.Items = GetChanges(new(), createDto);
                            }

                            TypeInfoDto record = _mapper.Map<TypeInfoDto>(createDto);
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
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportReferenceDocumentType([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.ReferenceDocumentType && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.ReferenceDocumentTypeHeadings;

                    var transaction = await _referenceDocumentTypeService.BeginTransaction();

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

                            CreateOrEditReferenceDocumentType createDto = new()
                            {
                                Type = dictionary[requiredKeys[0]],
                                Id = Guid.Empty
                            };
                            ValidationDataDto validationData = new()
                            {
                                Name = createDto.Type,
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
                                    ReferenceDocumentType existingType;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingType == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _referenceDocumentTypeService.GetSingleAsync(x => x.Id != editId &&
                                                x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Type is already taken.");
                                                validationData.Changes = GetChanges(existingType, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingType = await _referenceDocumentTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        if (existingType != null)
                                        {
                                            isUpdate = true;
                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingType, createDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingType.Type = createDto.Type;

                                            var response = _referenceDocumentTypeService.Update(existingType, existingType, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            ReferenceDocumentType model = _mapper.Map<ReferenceDocumentType>(createDto);
                                            validationData.Changes = GetChanges(model, createDto);
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
                            {
                                message.AddRange(validationResponse.Item2);
                                validationData.Changes = GetChanges(new(), createDto);
                            }

                            validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            validationData.Message = string.Join(", ", message);
                            validationDataList.Add(validationData);
                        }
                    }
                    await _referenceDocumentTypeService.RollbackTransaction(transaction);
                }
                else
                    return new() { Message = ResponseMessages.GlobalModelValidationMessage };

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

        private List<ChangesDto> GetChanges(ReferenceDocumentType entity, CreateOrEditReferenceDocumentType createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Type),
                    NewValue = createDto.Type,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Type : string.Empty,
                },
            };
            return changes;
        }
    }
}
