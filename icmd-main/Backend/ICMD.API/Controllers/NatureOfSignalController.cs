using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.NatureOfSignal;
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
    public class NatureOfSignalController : BaseController
    {
        private readonly INatureOfSignalService _natureOfSignalService;
        private readonly IAttributeDefinitionService _attributeDefinitionService;
        private readonly IAttributeValueService _attributeValueService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Nature of signal";

        private readonly ChangeLogHelper _changeLogHelper;

        public NatureOfSignalController(IMapper mapper, INatureOfSignalService natureOfSignalService, IAttributeDefinitionService attributeDefinitionService, IAttributeValueService attributeValueService,
            IDeviceService deviceService, CSVImport csvImport, ChangeLogHelper changeLogHelper)
        {
            _natureOfSignalService = natureOfSignalService;
            _mapper = mapper;
            _attributeDefinitionService = attributeDefinitionService;
            _attributeValueService = attributeValueService;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region NatureOfSignal
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<NatureOfSignalListDto>> GetAllNatureOfSignals(PagedAndSortedResultRequestDto input)
        {
            IQueryable<NatureOfSignalListDto> allNatureOfSignals = _natureOfSignalService.GetAll(s => !s.IsDeleted).Select(s => new NatureOfSignalListDto
            {
                Id = s.Id,
                NatureOfSignalName = s.NatureOfSignalName,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allNatureOfSignals = allNatureOfSignals.Where(s => (!string.IsNullOrEmpty(s.NatureOfSignalName) && s.NatureOfSignalName.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allNatureOfSignals = allNatureOfSignals.Where(input.SearchColumnFilterQuery);

            allNatureOfSignals = allNatureOfSignals.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<NatureOfSignalListDto> paginatedData = !isExport ? allNatureOfSignals.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allNatureOfSignals;


            return new PagedResultDto<NatureOfSignalListDto>(
               allNatureOfSignals.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<CreateOrEditNatureOfSignalDto?> GetNatureOfSignalInfo(Guid id)
        {
            NatureOfSignal? signalDetails = await _natureOfSignalService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (signalDetails != null)
            {
                CreateOrEditNatureOfSignalDto signalInfo = _mapper.Map<CreateOrEditNatureOfSignalDto>(signalDetails);
                List<AttributeValue> allValues = await _attributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.NatureOfSignalId == id).ToListAsync();
                signalInfo.Attributes = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.NatureOfSignalId == id).Select(s => new AttributesDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description ?? "",
                    ValueType = s.ValueType ?? "",
                    Private = s.Private,
                    Inherit = s.Inherit,
                    Required = s.Required,
                }).ToListAsync();

                foreach (var item in signalInfo.Attributes)
                {
                    item.Value = allValues.Count() != 0 && allValues.FirstOrDefault(a => a.AttributeDefinitionId == item.Id) != null ?
                    allValues.FirstOrDefault(a => a.AttributeDefinitionId == item.Id)?.Value ?? "" : "";
                }
                return signalInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditNatureOfSignal(CreateOrEditNatureOfSignalDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateNatureOfSignal(info);
            }
            else
            {
                return await UpdateNatureOfSignal(info);
            }
        }

        private async Task<BaseResponse> CreateNatureOfSignal(CreateOrEditNatureOfSignalDto info)
        {
            if (ModelState.IsValid)
            {
                NatureOfSignal existingName = await _natureOfSignalService.GetSingleAsync(x => x.NatureOfSignalName.ToLower().Trim() == info.NatureOfSignalName.ToLower().Trim() && !x.IsDeleted);
                if (existingName != null)
                    return new BaseResponse(false, ResponseMessages.NatureSignalNameAlreadyTaken, HttpStatusCode.Conflict);

                NatureOfSignal signalInfo = _mapper.Map<NatureOfSignal>(info);
                signalInfo.IsActive = true;
                var response = await _natureOfSignalService.AddAsync(signalInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (info.Attributes != null && info.Attributes.Any())
                {
                    foreach (var item in info.Attributes)
                    {
                        AttributeDefinition definitionInfo = _mapper.Map<AttributeDefinition>(item);
                        definitionInfo.IsActive = true;
                        definitionInfo.NatureOfSignalId = signalInfo.Id;
                        var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                        if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                        {
                            AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                            valueInfo.IsActive = true;
                            valueInfo.NatureOfSignalId = signalInfo.Id;
                            valueInfo.AttributeDefinitionId = definitionRespose.Id;
                            await _attributeValueService.AddAsync(valueInfo, User.GetUserId());
                        }
                    }
                }

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateNatureOfSignal(CreateOrEditNatureOfSignalDto info)
        {
            if (ModelState.IsValid)
            {
                NatureOfSignal signalDetails = await _natureOfSignalService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (signalDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                NatureOfSignal existingSignal = await _natureOfSignalService.GetSingleAsync(x => x.Id != info.Id && x.NatureOfSignalName.ToLower().Trim() == info.NatureOfSignalName.ToLower().Trim() && !x.IsDeleted);
                if (existingSignal != null)
                    return new BaseResponse(false, ResponseMessages.NatureSignalNameAlreadyTaken, HttpStatusCode.Conflict);

                NatureOfSignal signalInfo = _mapper.Map<NatureOfSignal>(info);
                signalInfo.CreatedBy = signalDetails.CreatedBy;
                signalInfo.CreatedDate = signalDetails.CreatedDate;
                signalInfo.IsActive = signalDetails.IsActive;
                var response = _natureOfSignalService.Update(signalInfo, signalDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                List<AttributeDefinition> removeDefinitionInfo = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.NatureOfSignalId == info.Id &&
              info.Attributes != null && !info.Attributes.Select(a => a.Id).Contains(s.Id)).ToListAsync();
                if (removeDefinitionInfo.Count() != 0)
                {
                    foreach (var item in removeDefinitionInfo)
                    {
                        //check in DeviceAttributeValue pending
                        AttributeValue chkValueInfo = await _attributeValueService.GetSingleAsync(s => s.NatureOfSignalId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted);
                        if (chkValueInfo != null)
                        {
                            chkValueInfo.IsDeleted = true;
                            _attributeValueService.Update(chkValueInfo, chkValueInfo, User.GetUserId(), true, true);
                        }

                        item.IsDeleted = true;
                        _attributeDefinitionService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                if (info.Attributes != null && info.Attributes.Any())
                {
                    foreach (var item in info.Attributes)
                    {
                        AttributeDefinition chkExistDefinitionInfo = await _attributeDefinitionService.GetSingleAsync(s => s.Id == item.Id && s.IsActive && !s.IsDeleted);
                        if (chkExistDefinitionInfo == null)
                        {
                            AttributeDefinition definitionInfo = _mapper.Map<AttributeDefinition>(item);
                            definitionInfo.IsActive = true;
                            definitionInfo.NatureOfSignalId = signalInfo.Id;
                            var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                            if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                valueInfo.IsActive = true;
                                valueInfo.NatureOfSignalId = signalInfo.Id;
                                valueInfo.AttributeDefinitionId = definitionRespose.Id;
                                await _attributeValueService.AddAsync(valueInfo, User.GetUserId());
                            }
                        }
                        else
                        {
                            AttributeDefinition defitionInfo = _mapper.Map<AttributeDefinition>(item);
                            defitionInfo.CreatedBy = chkExistDefinitionInfo.CreatedBy;
                            defitionInfo.CreatedDate = chkExistDefinitionInfo.CreatedDate;
                            defitionInfo.IsActive = chkExistDefinitionInfo.IsActive;
                            defitionInfo.NatureOfSignalId = info.Id;
                            _attributeDefinitionService.Update(defitionInfo, _mapper.Map<AttributeDefinition>(item), User.GetUserId());

                            AttributeValue? chkValueInfo = await _attributeValueService.GetAll(s => s.NatureOfSignalId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted).FirstOrDefaultAsync();
                            if (!string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                if (chkValueInfo != null)
                                {
                                    AttributeValue attributeValueInfo = _mapper.Map<AttributeValue>(chkValueInfo);
                                    attributeValueInfo.CreatedBy = chkValueInfo.CreatedBy;
                                    attributeValueInfo.CreatedDate = chkValueInfo.CreatedDate;
                                    attributeValueInfo.IsActive = chkValueInfo.IsActive;
                                    attributeValueInfo.Value = item.Value;
                                    attributeValueInfo.NatureOfSignalId = info.Id;
                                    attributeValueInfo.AttributeDefinitionId = item.Id;
                                    _attributeValueService.Update(attributeValueInfo, _mapper.Map<AttributeValue>(chkValueInfo), User.GetUserId());
                                }
                                else
                                {
                                    AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                    valueInfo.IsActive = true;
                                    valueInfo.NatureOfSignalId = signalInfo.Id;
                                    valueInfo.AttributeDefinitionId = item.Id;
                                    await _attributeValueService.AddAsync(valueInfo, User.GetUserId());
                                }
                            }
                            else if (chkValueInfo != null)
                            {
                                //check in DeviceAttributeValue pending
                                chkValueInfo.IsDeleted = true;
                                _attributeValueService.Update(chkValueInfo, chkValueInfo, User.GetUserId(), true, true);
                            }
                        }

                    }
                }

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteNatureOfSignal(Guid id)
        {
            NatureOfSignal signalDetails = await _natureOfSignalService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (signalDetails != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.NatureOfSignalId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, signalDetails);

                signalDetails.IsDeleted = true;
                var response = _natureOfSignalService.Update(signalDetails, signalDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, signalDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, signalDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkNatureOfSignals(List<Guid> ids)
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
                    var deleteResponse = await DeleteNatureOfSignal(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as NatureOfSignal;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.NatureOfSignalName,
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
                        Message = $"Failed to delete nature of signals.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted nature of signals. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of nature of signals have not been successfully deleted. \n" +
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
        public async Task<ImportFileResultDto<NatureOfSignalExportDto>> ImportNatureOfSignal([FromForm] FileUploadModel info)
        {
            List<NatureOfSignalExportDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.NatureOfSignals && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.NatureOfSignalTypeHeadings;

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

                            if (string.IsNullOrEmpty(dictionary[requiredKeys[0]]))
                                continue;

                            CreateOrEditNatureOfSignalDto createDto = new()
                            {
                                NatureOfSignalName = dictionary[requiredKeys[0]],
                                Id = Guid.Empty
                            };
                            var importLog = new ImportLogDto
                            {
                                Name = createDto.NatureOfSignalName,
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
                                    NatureOfSignal existingName;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingName = await _natureOfSignalService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingName == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _natureOfSignalService.GetSingleAsync(x => x.Id != editId &&
                                                x.NatureOfSignalName.ToLower().Trim() == createDto.NatureOfSignalName.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                importLog.Items = GetChanges(existingName, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingName = await _natureOfSignalService.GetSingleAsync(x => x.NatureOfSignalName.ToLower().Trim() == createDto.NatureOfSignalName.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        if (existingName != null)
                                        {
                                            isUpdate = true;

                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingName, createDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingName.NatureOfSignalName = createDto.NatureOfSignalName;

                                            var response = _natureOfSignalService.Update(existingName, existingName, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            NatureOfSignal model = _mapper.Map<NatureOfSignal>(createDto);
                                            importLog.Items = GetChanges(model, createDto);
                                            var response = await _natureOfSignalService.AddAsync(model, User.GetUserId());

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


                            NatureOfSignalExportDto record = _mapper.Map<NatureOfSignalExportDto>(createDto);
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportNatureOfSignal([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.NatureOfSignals && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.NatureOfSignalTypeHeadings;
                    var transaction = await _natureOfSignalService.BeginTransaction();

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

                            if (string.IsNullOrEmpty(dictionary[requiredKeys[0]]))
                                continue;

                            CreateOrEditNatureOfSignalDto createDto = new()
                            {
                                NatureOfSignalName = dictionary[requiredKeys[0]],
                                Id = Guid.Empty
                            };
                            ValidationDataDto validationData = new()
                            {
                                Name = createDto.NatureOfSignalName,
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
                                    NatureOfSignal existingName;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingName = await _natureOfSignalService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingName == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _natureOfSignalService.GetSingleAsync(x => x.Id != editId &&
                                                x.NatureOfSignalName.ToLower().Trim() == createDto.NatureOfSignalName.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                validationData.Changes = GetChanges(existingName, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingName = await _natureOfSignalService.GetSingleAsync(x => x.NatureOfSignalName.ToLower().Trim() == createDto.NatureOfSignalName.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        if (existingName != null)
                                        {
                                            isUpdate = true;

                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingName, createDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingName.NatureOfSignalName = createDto.NatureOfSignalName;

                                            var response = _natureOfSignalService.Update(existingName, existingName, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            NatureOfSignal model = _mapper.Map<NatureOfSignal>(createDto);
                                            validationData.Changes = GetChanges(model, createDto);

                                            var response = await _natureOfSignalService.AddAsync(model, User.GetUserId());
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
                    await _natureOfSignalService.RollbackTransaction(transaction);
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

        private List<ChangesDto> GetChanges(NatureOfSignal entity, CreateOrEditNatureOfSignalDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.NatureOfSignalName),
                    NewValue = createDto.NatureOfSignalName,
                    PreviousValue = entity.Id != Guid.Empty ? entity.NatureOfSignalName : string.Empty,
                },
            };
            return changes;
        }
    }
}
