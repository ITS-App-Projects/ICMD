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
using ICMD.Core.Dtos.Manufacturer;
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
    public class ManufacturerController : BaseController
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IDeviceModelService _deviceModelService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Manufacturer";

        private readonly ChangeLogHelper _changeLogHelper;

        public ManufacturerController(IManufacturerService manufacturerService, IMapper mapper, IDeviceModelService deviceModelService, CSVImport csvImport,
            ChangeLogHelper changeLogHelper)
        {
            _manufacturerService = manufacturerService;
            _mapper = mapper;
            _deviceModelService = deviceModelService;
            _csvImport = csvImport;
            _changeLogHelper = changeLogHelper;
        }

        #region Manufacturer
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ManufacturerInfoDto>> GetAllManufacturers(PagedAndSortedResultRequestDto input)
        {
            IQueryable<ManufacturerInfoDto> allManufacturers = _manufacturerService.GetAll(s => !s.IsDeleted).Select(s => new ManufacturerInfoDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description ?? "",
                Comment = s.Comment ?? ""
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allManufacturers = allManufacturers.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Comment) && s.Comment.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allManufacturers = allManufacturers.Where(input.SearchColumnFilterQuery);

            allManufacturers = allManufacturers.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<ManufacturerInfoDto> paginatedData = !isExport ? allManufacturers.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allManufacturers;


            return new PagedResultDto<ManufacturerInfoDto>(
               allManufacturers.Count(),
               await paginatedData.ToListAsync()
           );
        }


        [HttpGet]
        public async Task<ManufacturerInfoDto?> GetManufacturerInfo(Guid id)
        {
            Manufacturer? manufacturerDetails = await _manufacturerService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (manufacturerDetails != null)
            {
                return _mapper.Map<ManufacturerInfoDto>(manufacturerDetails);
            }
            return null;
        }


        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditManufacturer(CreateOrEditManufacturerDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateManufacturer(info);
            }
            else
            {
                return await UpdateManufacturer(info);
            }
        }

        private async Task<BaseResponse> CreateManufacturer(CreateOrEditManufacturerDto info)
        {
            if (ModelState.IsValid)
            {
                Manufacturer existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingManufacturer != null)
                    return new BaseResponse(false, ResponseMessages.ManufacturerNameAlreadyTaken, HttpStatusCode.Conflict);

                Manufacturer manufacturerInfo = _mapper.Map<Manufacturer>(info);
                manufacturerInfo.IsActive = true;
                var response = await _manufacturerService.AddAsync(manufacturerInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<BaseResponse> UpdateManufacturer(CreateOrEditManufacturerDto info)
        {
            if (ModelState.IsValid)
            {
                Manufacturer manufacturerDetails = await _manufacturerService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (manufacturerDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                Manufacturer existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Id != info.Id && x.Name.ToLower().Trim() == info.Name.ToLower().Trim() && !x.IsDeleted);
                if (existingManufacturer != null)
                    return new BaseResponse(false, ResponseMessages.ManufacturerNameAlreadyTaken, HttpStatusCode.Conflict);

                Manufacturer manufacturerInfo = _mapper.Map<Manufacturer>(info);
                manufacturerInfo.CreatedBy = manufacturerDetails.CreatedBy;
                manufacturerInfo.CreatedDate = manufacturerDetails.CreatedDate;
                manufacturerInfo.IsActive = manufacturerDetails.IsActive;
                var response = _manufacturerService.Update(manufacturerInfo, manufacturerDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                return new BaseResponse(true, ResponseMessages.ModuleUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteManufacturer(Guid id)
        {
            Manufacturer manufacturerDetail = await _manufacturerService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (manufacturerDetail != null)
            {
                bool isChkExist = _deviceModelService.GetAll(s => s.IsActive && !s.IsDeleted && s.ManufacturerId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ManufacturerNotDelete, HttpStatusCode.InternalServerError, manufacturerDetail);

                manufacturerDetail.IsDeleted = true;
                var response = _manufacturerService.Update(manufacturerDetail, manufacturerDetail, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError, manufacturerDetail);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, manufacturerDetail);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [AuthorizePermission(Operations.Delete)]
        public async Task<BaseResponse> DeleteBulkManufacturers(List<Guid> ids)
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
                    var deleteResponse = await DeleteManufacturer(id);
                    if (deleteResponse.Data != null)
                    {
                        var record = deleteResponse.Data as Manufacturer;
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
                        Message = $"Failed to delete manufacturers.",
                        Data = result,
                    };
                }

                if (result.Count != 0 && result.All(r => r.IsSucceeded))
                {
                    return new BaseResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSucceeded = true,
                        Message = $"Successfully deleted manufacturers. \n" +
                                  $"Success: {result.Where(r => r.IsSucceeded).Count()}",
                        Data = result,
                    };
                }

                return new BaseResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSucceeded = true,
                    IsWarning = result.Any(r => !r.IsSucceeded),
                    Message = $"Some records of manufacturers have not been successfully deleted. \n" +
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
        public async Task<List<DropdownInfoDto>> GetAllManufacturerInfo()
        {
            List<DropdownInfoDto> allManufacturers = await _manufacturerService.GetAll(s => !s.IsDeleted).Select(s => new DropdownInfoDto
            {
                Id = s.Id,
                Name = s.Name,
            }).ToListAsync();

            return allManufacturers;
        }

        #endregion


        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<ManufacturerInfoDto>> ImportManufacturer([FromForm] FileUploadModel info)
        {
            List<ManufacturerInfoDto> responseList = [];
            List<ImportLogDto> importLogs = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.Manufacturer && typeHeaders != null)
                {
                    var isEditImport = false;
                    if (typeHeaders.FirstOrDefault() != null && typeHeaders.FirstOrDefault()!.FirstOrDefault().Key == FileHeadingConstants.IdHeading)
                        isEditImport = true;

                    List<string> requiredKeys = FileHeadingConstants.ManufacturerHeadings;

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

                            CreateOrEditManufacturerDto createDto = new()
                            {
                                Name = dictionary[requiredKeys[0]],
                                Description = dictionary[requiredKeys[1]],
                                Comment = dictionary[requiredKeys[2]],
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
                                    Manufacturer existingManufacturer;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        importLog.Operation = OperationType.Edit;
                                        existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingManufacturer == null)
                                        {
                                            message.Add("Record is not found.");
                                            importLog.Items = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _manufacturerService.GetSingleAsync(x => x.Id != editId &&
                                                x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                importLog.Items = GetChanges(existingManufacturer, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        Manufacturer model = _mapper.Map<Manufacturer>(createDto);
                                        if (existingManufacturer != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingManufacturer.Id;
                                            model.CreatedBy = existingManufacturer.CreatedBy;
                                            model.CreatedDate = existingManufacturer.CreatedDate;

                                            importLog.Operation = OperationType.Edit;
                                            importLog.Items = GetChanges(existingManufacturer, createDto);

                                            var response = _manufacturerService.Update(model, existingManufacturer, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            importLog.Items = GetChanges(model, createDto);
                                            var response = await _manufacturerService.AddAsync(model, User.GetUserId());

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

                            ManufacturerInfoDto record = _mapper.Map<ManufacturerInfoDto>(createDto);
                            record.Manufacturer = record.Name;
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
        public async Task<ImportFileResultDto<ValidationDataDto>> ValidateImportManufacturer([FromForm] FileUploadModel info)
        {
            List<ValidationDataDto> validationDataList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.Manufacturer && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.ManufacturerHeadings;
                    var transaction = await _manufacturerService.BeginTransaction();

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

                            CreateOrEditManufacturerDto createDto = new()
                            {
                                Name = dictionary[requiredKeys[0]],
                                Description = dictionary[requiredKeys[1]],
                                Comment = dictionary[requiredKeys[2]],
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
                                    Manufacturer existingManufacturer;
                                    if (isEditImport && editId != Guid.Empty)
                                    {
                                        validationData.Operation = OperationType.Edit;
                                        existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Id == editId &&
                                            !x.IsDeleted && x.IsActive);
                                        if (existingManufacturer == null)
                                        {
                                            message.Add("Record is not found.");
                                            validationData.Changes = GetChanges(new(), createDto);
                                        }
                                        else
                                        {
                                            var existingRecordName = await _manufacturerService.GetSingleAsync(x => x.Id != editId &&
                                                x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() &&
                                                !x.IsDeleted && x.IsActive);
                                            if (existingRecordName != null)
                                            {
                                                message.Add("Name is already taken.");
                                                validationData.Changes = GetChanges(existingManufacturer, createDto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingManufacturer = await _manufacturerService.GetSingleAsync(x => x.Name.ToLower().Trim() == createDto.Name.ToLower().Trim() && !x.IsDeleted && x.IsActive);
                                    }

                                    if (message.Count == 0)
                                    {
                                        Manufacturer model = _mapper.Map<Manufacturer>(createDto);
                                        if (existingManufacturer != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingManufacturer.Id;
                                            model.CreatedBy = existingManufacturer.CreatedBy;
                                            model.CreatedDate = existingManufacturer.CreatedDate;

                                            validationData.Operation = OperationType.Edit;
                                            validationData.Changes = GetChanges(existingManufacturer, createDto);

                                            var response = _manufacturerService.Update(model, existingManufacturer, User.GetUserId());
                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            validationData.Changes = GetChanges(model, createDto);

                                            var response = await _manufacturerService.AddAsync(model, User.GetUserId());
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
                    await _manufacturerService.RollbackTransaction(transaction);
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

        private List<ChangesDto> GetChanges(Manufacturer entity, CreateOrEditManufacturerDto createDto)
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
                new() {
                    ItemColumnName = nameof(entity.Comment),
                    NewValue = createDto.Comment,
                    PreviousValue = entity.Id != Guid.Empty ? entity.Comment ?? string.Empty : string.Empty,
                },
            };
            return changes;
        }
    }
}
