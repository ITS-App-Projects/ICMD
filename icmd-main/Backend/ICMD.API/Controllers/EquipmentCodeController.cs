using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.EquipmentCode;
using ICMD.Core.Dtos.ImportValidation;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

        private readonly ChangeLogHelper _changeLogHelper;

        public EquipmentCodeController(IMapper mapper, IEquipmentCodeService equipmentCodeService, ITagService tagServie, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _equipmentCodeService = equipmentCodeService;
            _mapper = mapper;
            _tagServie = tagServie;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
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
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssignedTag.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, codeDetails);

                codeDetails.IsDeleted = true;
                var response = _equipmentCodeService.Update(codeDetails, codeDetails, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, codeDetails);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, codeDetails);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkEquipmentCodes(List<Guid> ids)
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
                    var deleteResponse = await DeleteEquipmentCode(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as EquipmentCode;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Code,
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
                        Message = $"Failed to delete equipment codes.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted equipment codes. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of equipment codes have not been successfully deleted. \n" +
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
        public async Task<ImportFileResultDto<EquipmentCodeInfoDto>> ImportEquipmentCode([FromForm] FileUploadModel info)
        {
            List<EquipmentCodeInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.EquipmentCode && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.EquipmentCodeHeadings;

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

                            CreateOrEditEquipmentCodeDto createDto = new()
                            {
                                Code = dictionary[requiredKeys[0]],
                                Descriptor = dictionary[requiredKeys[1]],
                                Id = Guid.Empty
                            };
                            var importLog = new ImportLogDto
                            {
                                Name = createDto.Code,
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
                                    EquipmentCode existingCode;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingCode == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _equipmentCodeService.GetSingleAsync(x => x.Id != editId &&
                                                x.Code.ToLower().Trim() == createDto.Code.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Code is already taken.");
                                                importLog.Items = GetChanges(existingCode, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Code.ToLower().Trim() == createDto.Code.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        EquipmentCode model = _mapper.Map<EquipmentCode>(createDto);
                                        if (existingCode != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingCode.Id;
                                            model.CreatedBy = existingCode.CreatedBy;
                                            model.CreatedDate = existingCode.CreatedDate;

                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingCode, createDto);

                                            var response = _equipmentCodeService.Update(model, existingCode, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            importLog.Items = GetChanges(model, createDto);
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
                            {
                                message.AddRange(validationResponse.Item2);
                                importLog.Items = GetChanges(new(), createDto);
                            }

                            EquipmentCodeInfoDto record = _mapper.Map<EquipmentCodeInfoDto>(createDto);
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportEquipmentCode([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.EquipmentCode && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.EquipmentCodeHeadings;

                    var transaction = await _equipmentCodeService.BeginTransaction();

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

                            CreateOrEditEquipmentCodeDto createDto = new()
                            {
                                Code = dictionary[requiredKeys[0]],
                                Descriptor = dictionary[requiredKeys[1]],
                                Id = Guid.Empty
                            };
                            ValidationDataDto validationData = new()
                            {
                                Name = createDto.Code,
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
                                    EquipmentCode existingCode;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingCode == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _equipmentCodeService.GetSingleAsync(x => x.Id != editId &&
                                                x.Code.ToLower().Trim() == createDto.Code.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Code is already taken.");
                                                validationData.Changes = GetChanges(existingCode, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingCode = await _equipmentCodeService.GetSingleAsync(x => x.Code.ToLower().Trim() == createDto.Code.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }
                                    if (message.Count == 0)
                                    {
                                        EquipmentCode model = _mapper.Map<EquipmentCode>(createDto);
                                        if (existingCode != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingCode.Id;
                                            model.CreatedBy = existingCode.CreatedBy;
                                            model.CreatedDate = existingCode.CreatedDate;

                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingCode, createDto);

                                            var response = _equipmentCodeService.Update(model, existingCode, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            validationData.Changes = GetChanges(model, createDto);

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
                            {
                                message.AddRange(validationResponse.Item2);
                                validationData.Changes = GetChanges(new(), createDto);
                            }

                            validationData.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            validationData.Message = string.Join(", ", message);
                            validationDataList.Add(validationData);
                        }
                    }
                    await _equipmentCodeService.RollbackTransaction(transaction);
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

        private List<ChangesDto> GetChanges(EquipmentCode entity, CreateOrEditEquipmentCodeDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new() {
                    ItemColumnName = nameof(entity.Code),
                    NewValue = createDto.Code,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Code : string.Empty,
                },
                new() {
                    ItemColumnName = nameof(entity.Descriptor),
                    NewValue = createDto.Descriptor,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Descriptor ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }
    }
}
