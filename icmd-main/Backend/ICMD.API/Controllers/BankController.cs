using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Bank;
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
    public class BankController : BaseController
    {
        private readonly IBankService _bankService;
        private readonly IDeviceService _deviceService;
        private readonly CSVImport _csvImport;
        private readonly IMapper _mapper;
        private static string ModuleName = "Bank";
        public BankController(IMapper mapper, IBankService bankService, IDeviceService deviceService, CSVImport csvImport)
        {
            _bankService = bankService;
            _mapper = mapper;
            _deviceService = deviceService;
            _csvImport = csvImport;
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
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                bankDetail.IsDeleted = true;
                var response = _bankService.Update(bankDetail, bankDetail, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<BankInfoDto>> ImportBank([FromForm] FileUploadModel info)
        {
            List<BankInfoDto> bankResponseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.Bank && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.BankListHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
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

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(bankDto);
                            isSuccess = validationResponse.Item1;

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    ServiceBank existingBank = await _bankService.GetSingleAsync(x => x.ProjectId == info.ProjectId && x.Bank.ToLower().Trim() == dictionary[requiredKeys[0]].ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        if (existingBank != null)
                                        {
                                            isUpdate = true;
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
                            }
                            else
                                message.AddRange(validationResponse.Item2);

                            BankInfoDto record = _mapper.Map<BankInfoDto>(bankDto);
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            bankResponseList.Add(record);
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
                    Records = bankResponseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }
    }
}
