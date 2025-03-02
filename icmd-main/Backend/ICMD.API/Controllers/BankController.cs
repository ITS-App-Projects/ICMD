using System.Linq.Dynamic.Core;
using System.Net;

using AutoMapper;

using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Bank;
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
    public class BankController : BaseController
    {
        private readonly IBankService _bankService;
        private readonly IDeviceService _deviceService;
        private readonly CSVImport _csvImport;
        private readonly IMapper _mapper;
        private static string ModuleName = "Bank";

        private readonly ChangeLogHelper _changeLogHelper;

        public BankController(IMapper mapper, IBankService bankService, IDeviceService deviceService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _bankService = bankService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region ServiceBank

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<BankInfoDto>> GetAllBanks(PagedAndSortedResultRequestDto input)
        {
            IQueryable<BankInfoDto> allBanks = _bankService.GetAll(s => !s.IsDeleted).Select(s => new BankInfoDto
            {
                Id = s.Id,
                Bank = s.Bank,
                ProjectId = s.ProjectId,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allBanks = allBanks.Where(s => (!string.IsNullOrEmpty(s.Bank) && s.Bank.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "projectIds".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allBanks = allBanks.Where(x => ids != null && ids.Contains(x.ProjectId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allBanks = allBanks.Where(input.SearchColumnFilterQuery);

            allBanks = allBanks.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<BankInfoDto> paginatedData = !isExport ? allBanks.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allBanks;


            return new PagedResultDto<BankInfoDto>(
               allBanks.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<BankInfoDto?> GetBankInfo(Guid id)
        {
            ServiceBank? bankDetails = await _bankService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (bankDetails != null)
            {
                return _mapper.Map<BankInfoDto>(bankDetails);
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditBank(CreateOrEditBankDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateBank(info);
            }
            else
            {
                return await UpdateBank(info);
            }
        }

        private async Task<BaseResponse> CreateBank(CreateOrEditBankDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceBank existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Bank.ToLower().Trim() == info.Bank.ToLower().Trim() && !x.IsDeleted);
                if (existingBank != null)
                    return new BaseResponse(false, ResponseMessages.BankAlreadyTaken, HttpStatusCode.Conflict);

                ServiceBank bankInfo = _mapper.Map<ServiceBank>(info);
                bankInfo.IsActive = true;
                var response = await _bankService.AddAsync(bankInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateBank(CreateOrEditBankDto info)
        {
            if (ModelState.IsValid)
            {
                ServiceBank bankDetails = await _bankService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (bankDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                ServiceBank existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Id != info.Id && x.Bank.ToLower().Trim() == info.Bank.ToLower().Trim() && !x.IsDeleted);
                if (existingBank != null)
                    return new BaseResponse(false, ResponseMessages.BankAlreadyTaken, HttpStatusCode.Conflict);

                ServiceBank bankInfo = _mapper.Map<ServiceBank>(info);
                bankInfo.CreatedBy = bankDetails.CreatedBy;
                bankInfo.CreatedDate = bankDetails.CreatedDate;
                bankInfo.IsActive = bankDetails.IsActive;
                var response = _bankService.Update(bankInfo, bankDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBank(Guid id)
        {
            ServiceBank bankDetail = await _bankService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (bankDetail != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.ServiceBankId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, bankDetail);

                bankDetail.IsDeleted = true;
                var response = _bankService.Update(bankDetail, bankDetail, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, bankDetail);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, bankDetail);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        public async Task<BaseResponse> DeleteBulkBanks(List<Guid> ids)
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
                    var deleteResponse = await DeleteBank(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as ServiceBank;
                        bulkLog.Add(new BulkDeleteLogDto()
                        {
                            Name = record?.Bank,
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
                        Message = $"Failed to delete banks.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted banks. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of banks have not been successfully deleted. \n" +
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
        public async Task<ImportFileResultDto<BankInfoDto>> ImportBank([FromForm] FileUploadModel info)
        {
            List<BankInfoDto> bankResponseList = [];
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.Bank && typeHeaders != null)
                {
                    var isEditImport = false;
                    if (typeHeaders.FirstOrDefault() != null &&
                        typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                    {
                        isEditImport = true;
                    }

                    List<string> requiredKeys = FileHeadingConstants.BankListHeadings;

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

                            CreateOrEditBankDto bankDto = new()
                            {
                                Bank = dictionary[requiredKeys[0]],
                                ProjectId = info.ProjectId,
                                Id = Guid.Empty
                            };
                            var importLog = new ImportLogDto
                            {
                                Name = bankDto.Bank,
                                Operation = OperationType.Insert,
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(bankDto);
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
                                    ServiceBank existingBank;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                            x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingBank == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), bankDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                            x.Id != editId &&
                                            x.Bank.ToLower().Trim() == dictionary[requiredKeys[0]].ToLower().Trim() &&
                                            !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Bank Name is already taken.");
                                                importLog.Items = GetChanges(existingBank, bankDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Bank.ToLower().Trim() == dictionary[requiredKeys[0]].ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        if (existingBank != null)
                                        {
                                            isUpdate = true;
                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingBank, bankDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingBank.Bank = bankDto.Bank;

                                            var response = _bankService.Update(existingBank, existingBank, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            ServiceBank bankInfo = new()
                                            {
                                                Bank = dictionary[requiredKeys[0]],
                                                ProjectId = info.ProjectId
                                            };
                                            importLog.Items = GetChanges(bankInfo, bankDto);

                                            var response = await _bankService.AddAsync(bankInfo, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName));
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    message.Add((isUpdate ? ResponseMessages.ModuleNotUpdated : ResponseMessages.ModuleNotCreated).ToString().Replace("{module}", ModuleName));
                                }

                                importLog.Operation = isUpdate ? OperationType.Edit : OperationType.Insert;
                            }
                            else
                            {
                                message.AddRange(validationResponse.Item2);
                                importLog.Operation = OperationType.Insert;
                                importLog.Items = GetChanges(new(), bankDto);
                            }

                            BankInfoDto record = _mapper.Map<BankInfoDto>(bankDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            bankResponseList.Add(record);

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

                if (bankResponseList.All(x => x.Status == ImportFileRecordStatus.Success))
                {
                    return new()
                    {
                        IsSucceeded = true,
                        Message = ResponseMessages.ImportFile,
                        Records = bankResponseList
                    };
                }
                else if (bankResponseList.All(x => x.Status == ImportFileRecordStatus.Fail))
                {

                    return new()
                    {
                        IsSucceeded = false,
                        Message = ResponseMessages.FailedImportFile,
                        Records = bankResponseList
                    };
                }

                return new()
                {
                    IsSucceeded = true,
                    IsWarning = true,
                    Message = ResponseMessages.SomeFailedImportFile,
                    Records = bankResponseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportBank([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = new();

            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.Bank && typeHeaders != null)
                {
                    var isEditImport = false;
                    if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                        isEditImport = true;

                    List<string> requiredKeys = FileHeadingConstants.BankListHeadings;

                    var transaction = await _bankService.BeginTransaction();

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

                            CreateOrEditBankDto createDto = new()
                            {
                                Bank = dictionary[requiredKeys[0]],
                                ProjectId = info.ProjectId,
                                Id = Guid.Empty
                            };
                            ValidationDataDto validationData = new()
                            {
                                Name = createDto.Bank,
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
                                    ServiceBank existingBank;

                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                            x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingBank == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId &&
                                                x.Id != editId &&
                                                x.Bank.ToLower().Trim() == dictionary[requiredKeys[0]].ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Bank Name is already taken.");
                                                validationData.Changes = GetChanges(existingBank, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Bank.ToLower().Trim() == dictionary[requiredKeys[0]].ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        if (existingBank != null)
                                        {
                                            isUpdate = true;
                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingBank, createDto);

                                            if (isEditImport && editId != Guid.Empty)
                                                existingBank.Bank = createDto.Bank;

                                            var response = _bankService.Update(existingBank, existingBank, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            ServiceBank model = new()
                                            {
                                                Bank = dictionary[requiredKeys[0]],
                                                ProjectId = info.ProjectId
                                            };
                                            validationData.Changes = GetChanges(model, createDto);

                                            var response = await _bankService.AddAsync(model, User.GetUserId());
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
                    await _bankService.RollbackTransaction(transaction);
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

        private List<ChangesDto> GetChanges(ServiceBank entity, CreateOrEditBankDto createDto)
        {
            var changes = new List<ChangesDto>
            {
                new ChangesDto
                {
                    ItemColumnName = nameof(entity.Bank),
                    NewValue = createDto.Bank,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Bank : string.Empty,
                }
            };
            return changes;
        }
    }
}
