using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.DeviceType;
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
    public class DeviceTypeController : BaseController
    {
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IAttributeDefinitionService _attributeDefinitionService;
        private readonly IAttributeValueService _attributeValueService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Device type";
        public DeviceTypeController(IMapper mapper, IDeviceTypeService deviceTypeService, IAttributeDefinitionService attributeDefinitionService, IAttributeValueService attributeValueService,
            IDeviceService deviceService, CSVImport csvImport)
        {
            _deviceTypeService = deviceTypeService;
            _mapper = mapper;
            _attributeDefinitionService = attributeDefinitionService;
            _attributeValueService = attributeValueService;
            _deviceService = deviceService;
            _csvImport = csvImport;
        }

        #region DeviceType
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<DeviceTypeListDto>> GetAllDeviceTypes(PagedAndSortedResultRequestDto input)
        {
            IQueryable<DeviceTypeListDto> allTypes = _deviceTypeService.GetAll(s => !s.IsDeleted).Select(s => new DeviceTypeListDto
            {
                Id = s.Id,
                Type = s.Type,
                Description = s.Description,
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allTypes = allTypes.Where(s => (!string.IsNullOrEmpty(s.Type) && s.Type.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())));
            }
            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTypes = allTypes.Where(input.SearchColumnFilterQuery);

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allTypes = allTypes.Where(input.SearchColumnFilterQuery);

            allTypes = allTypes.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<DeviceTypeListDto> paginatedData = !isExport ? allTypes.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allTypes;


            return new PagedResultDto<DeviceTypeListDto>(
               allTypes.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<DeviceTypeInfoDto?> GetDeviceTypeInfo(Guid id)
        {
            DeviceType? typeDetails = await _deviceTypeService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (typeDetails != null)
            {
                DeviceTypeInfoDto deviceInfo = _mapper.Map<DeviceTypeInfoDto>(typeDetails);
                List<AttributeValue> allValues = await _attributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceTypeId == id).ToListAsync();
                deviceInfo.Attributes = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceTypeId == id).Select(s => new AttributesDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description ?? "",
                    ValueType = s.ValueType ?? "",
                    Private = s.Private,
                    Inherit = s.Inherit,
                    Required = s.Required,
                }).ToListAsync();

                foreach (var item in deviceInfo.Attributes)
                {
                    item.Value = allValues.Count() != 0 && allValues.FirstOrDefault(a => a.AttributeDefinitionId == item.Id) != null ?
                    allValues.FirstOrDefault(a => a.AttributeDefinitionId == item.Id)?.Value ?? "" : "";
                }
                return deviceInfo;
            }
            return null;
        }

        [HttpPost]
        [AuthorizePermission(Operations.Add, Operations.Edit)]
        public async Task<BaseResponse> CreateOrEditDeviceType(CreateOrEditDeviceTypeDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateDeviceType(info);
            }
            else
            {
                return await UpdateDeviceType(info);
            }
        }

        private async Task<BaseResponse> CreateDeviceType(CreateOrEditDeviceTypeDto info)
        {
            if (ModelState.IsValid)
            {
                DeviceType existingType = await _deviceTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == info.Type.ToLower().Trim() && !x.IsDeleted);
                if (existingType != null)
                    return new BaseResponse(false, ResponseMessages.TypeAlreadyTaken, HttpStatusCode.Conflict);

                DeviceType typeInfo = _mapper.Map<DeviceType>(info);
                typeInfo.IsActive = true;
                var response = await _deviceTypeService.AddAsync(typeInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (info.Attributes != null && info.Attributes.Any())
                {
                    foreach (var item in info.Attributes)
                    {
                        AttributeDefinition definitionInfo = _mapper.Map<AttributeDefinition>(item);
                        definitionInfo.IsActive = true;
                        definitionInfo.DeviceTypeId = typeInfo.Id;
                        var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                        if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                        {
                            AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                            valueInfo.IsActive = true;
                            valueInfo.DeviceTypeId = typeInfo.Id;
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

        private async Task<BaseResponse> UpdateDeviceType(CreateOrEditDeviceTypeDto info)
        {
            if (ModelState.IsValid)
            {
                DeviceType typeDetails = await _deviceTypeService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (typeDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                DeviceType existingType = await _deviceTypeService.GetSingleAsync(x => x.Id != info.Id && x.Type.ToLower().Trim() == info.Type.ToLower().Trim() && !x.IsDeleted);
                if (existingType != null)
                    return new BaseResponse(false, ResponseMessages.TypeAlreadyTaken, HttpStatusCode.Conflict);

                DeviceType typeInfo = _mapper.Map<DeviceType>(info);
                typeInfo.CreatedBy = typeDetails.CreatedBy;
                typeInfo.CreatedDate = typeDetails.CreatedDate;
                typeInfo.IsActive = typeDetails.IsActive;
                var response = _deviceTypeService.Update(typeInfo, typeDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                List<AttributeDefinition> removeDefinitionInfo = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceTypeId == info.Id &&
              info.Attributes != null && !info.Attributes.Select(a => a.Id).Contains(s.Id)).ToListAsync();
                if (removeDefinitionInfo.Count() != 0)
                {
                    foreach (var item in removeDefinitionInfo)
                    {
                        //check in DeviceAttributeValue pending
                        AttributeValue chkValueInfo = await _attributeValueService.GetSingleAsync(s => s.DeviceTypeId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted);
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
                            definitionInfo.DeviceTypeId = typeInfo.Id;
                            var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                            if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                valueInfo.IsActive = true;
                                valueInfo.DeviceTypeId = typeInfo.Id;
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
                            defitionInfo.DeviceTypeId = info.Id;
                            _attributeDefinitionService.Update(defitionInfo, _mapper.Map<AttributeDefinition>(item), User.GetUserId());

                            AttributeValue? chkValueInfo = await _attributeValueService.GetAll(s => s.DeviceTypeId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted).FirstOrDefaultAsync();
                            if (!string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                if (chkValueInfo != null)
                                {
                                    AttributeValue attributeValueInfo = _mapper.Map<AttributeValue>(chkValueInfo);
                                    attributeValueInfo.CreatedBy = chkValueInfo.CreatedBy;
                                    attributeValueInfo.CreatedDate = chkValueInfo.CreatedDate;
                                    attributeValueInfo.IsActive = chkValueInfo.IsActive;
                                    attributeValueInfo.Value = item.Value;
                                    attributeValueInfo.DeviceTypeId = info.Id;
                                    attributeValueInfo.AttributeDefinitionId = item.Id;
                                    _attributeValueService.Update(attributeValueInfo, _mapper.Map<AttributeValue>(chkValueInfo), User.GetUserId());
                                }
                                else
                                {
                                    AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                    valueInfo.IsActive = true;
                                    valueInfo.DeviceTypeId = typeInfo.Id;
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
        public async Task<BaseResponse> DeleteDeviceType(Guid id)
        {
            DeviceType typeDetail = await _deviceTypeService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (typeDetail != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceTypeId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                typeDetail.IsDeleted = true;
                var response = _deviceTypeService.Update(typeDetail, typeDetail, User.GetUserId(), true, true);
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
        public async Task<ImportFileResultDto<DeviceTypeListDto>> ImportDeviceType([FromForm] FileUploadModel info)
        {
            List<DeviceTypeListDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.DeviceType && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.DeviceTypeHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            CreateOrEditDeviceTypeDto createDto = new()
                            {
                                Type = dictionary[requiredKeys[0]],
                                Description = dictionary[requiredKeys[1]],
                                Id = Guid.Empty
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                            isSuccess = validationResponse.Item1;
                            if (!isSuccess)
                                message.AddRange(validationResponse.Item2);

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    DeviceType existingType = await _deviceTypeService.GetSingleAsync(x => x.Type.ToLower().Trim() == createDto.Type.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        DeviceType model = _mapper.Map<DeviceType>(createDto);
                                        if (existingType != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingType.Id;
                                            model.CreatedBy = existingType.CreatedBy;
                                            model.CreatedDate = existingType.CreatedDate;
                                            var response = _deviceTypeService.Update(model, existingType, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            var response = await _deviceTypeService.AddAsync(model, User.GetUserId());

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


                            DeviceTypeListDto record = _mapper.Map<DeviceTypeListDto>(createDto);
                            record.DeviceType = createDto.Type;
                            record.Status = message.Count > 0 ? ImportFileRecordStatus.Fail : ImportFileRecordStatus.Success;
                            record.Message = string.Join(", ", message);
                            responseList.Add(record);
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
                    Records = responseList
                };
            }
            return new()
            {
                Message = ResponseMessages.GlobalModelValidationMessage
            };
        }

    }
}
