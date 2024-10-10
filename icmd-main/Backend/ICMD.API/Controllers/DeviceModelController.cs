﻿using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.DeviceModel;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Linq.Dynamic.Core;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos;
using ICMD.API.Helpers;


namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DeviceModelController : BaseController
    {
        private readonly IDeviceModelService _deviceModelService;
        private readonly IAttributeDefinitionService _attributeDefinitionService;
        private readonly IAttributeValueService _attributeValueService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IManufacturerService _manufacturerService;
        private readonly CSVImport _csvImport;
        private static string ModuleName = "Device model";
        public DeviceModelController(IMapper mapper, IDeviceModelService deviceModelService, IAttributeDefinitionService attributeDefinitionService, IAttributeValueService attributeValueService,
            IDeviceService deviceService, CSVImport csvImport, IManufacturerService manufacturerService)
        {
            _deviceModelService = deviceModelService;
            _mapper = mapper;
            _attributeDefinitionService = attributeDefinitionService;
            _attributeValueService = attributeValueService;
            _deviceService = deviceService;
            _csvImport = csvImport;
            _manufacturerService = manufacturerService;
        }

        #region DeviceModel
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<DeviceModelListDto>> GetAllDeviceModels(PagedAndSortedResultRequestDto input)
        {
            IQueryable<DeviceModelListDto> allModels = _deviceModelService.GetAll(s => !s.IsDeleted).Select(s => new DeviceModelListDto
            {
                Id = s.Id,
                Model = s.Model,
                Description = s.Description,
                Manufacturer = s.Manufacturer != null ? s.Manufacturer.Name : "",
                ManufacturerId = s.ManufacturerId
            });

            if (!string.IsNullOrEmpty(input.Search))
            {
                allModels = allModels.Where(s => (!string.IsNullOrEmpty(s.Model) && s.Model.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                (!string.IsNullOrEmpty(s.Manufacturer) && s.Manufacturer.ToLower().Contains(input.Search.ToLower())));
            }

            if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
            {
                foreach (var item in input.CustomSearchs)
                {
                    if (item.FieldName.ToLower() == "manufacturerId".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                    {
                        var ids = item.FieldValue?.Split(",");
                        allModels = allModels.Where(x => ids != null && ids.Contains(x.ManufacturerId.ToString()));
                    }
                }
            }

            if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                allModels = allModels.Where(input.SearchColumnFilterQuery);

            allModels = allModels.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "id" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");

            bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
            IQueryable<DeviceModelListDto> paginatedData = !isExport ? allModels.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allModels;


            return new PagedResultDto<DeviceModelListDto>(
               allModels.Count(),
               await paginatedData.ToListAsync()
           );
        }

        [HttpGet]
        public async Task<DeviceModelInfoDto?> GetDeviceModelInfo(Guid id)
        {
            DeviceModel? modelDetails = await _deviceModelService.GetAll(s => s.IsActive && !s.IsDeleted && s.Id == id).FirstOrDefaultAsync();
            if (modelDetails != null)
            {
                DeviceModelInfoDto deviceInfo = _mapper.Map<DeviceModelInfoDto>(modelDetails);
                List<AttributeValue> allValues = await _attributeValueService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceModelId == id).ToListAsync();
                deviceInfo.Attributes = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceModelId == id).Select(s => new AttributesDto
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
        public async Task<BaseResponse> CreateOrEditDeviceModel(CreateOrEditDeviceModelDto info)
        {
            if (info.Id == Guid.Empty)
            {
                return await CreateDeviceModel(info);
            }
            else
            {
                return await UpdateDeviceModel(info);
            }
        }

        private async Task<BaseResponse> CreateDeviceModel(CreateOrEditDeviceModelDto info)
        {
            if (ModelState.IsValid)
            {
                DeviceModel existingModel = await _deviceModelService.GetSingleAsync(x => x.ManufacturerId == info.ManufacturerId && x.Model.ToLower().Trim() == info.Model.ToLower().Trim() && !x.IsDeleted);
                if (existingModel != null)
                    return new BaseResponse(false, ResponseMessages.ModelAlreadyTaken, HttpStatusCode.Conflict);

                DeviceModel modelInfo = _mapper.Map<DeviceModel>(info);
                modelInfo.IsActive = true;
                var response = await _deviceModelService.AddAsync(modelInfo, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                if (info.Attributes != null && info.Attributes.Any())
                {
                    foreach (var item in info.Attributes)
                    {
                        AttributeDefinition definitionInfo = _mapper.Map<AttributeDefinition>(item);
                        definitionInfo.IsActive = true;
                        definitionInfo.DeviceModelId = modelInfo.Id;
                        var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                        if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                        {
                            AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                            valueInfo.IsActive = true;
                            valueInfo.DeviceModelId = modelInfo.Id;
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

        private async Task<BaseResponse> UpdateDeviceModel(CreateOrEditDeviceModelDto info)
        {
            if (ModelState.IsValid)
            {
                DeviceModel modelDetails = await _deviceModelService.GetSingleAsync(s => s.Id == info.Id && s.IsActive && !s.IsDeleted);
                if (modelDetails == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);

                DeviceModel existingModel = await _deviceModelService.GetSingleAsync(x => x.ManufacturerId == info.ManufacturerId && x.Id != info.Id && x.Model.ToLower().Trim() == info.Model.ToLower().Trim() && !x.IsDeleted);
                if (existingModel != null)
                    return new BaseResponse(false, ResponseMessages.ModelAlreadyTaken, HttpStatusCode.Conflict);

                DeviceModel modelInfo = _mapper.Map<DeviceModel>(info);
                modelInfo.CreatedBy = modelDetails.CreatedBy;
                modelInfo.CreatedDate = modelDetails.CreatedDate;
                modelInfo.IsActive = modelDetails.IsActive;
                var response = _deviceModelService.Update(modelInfo, modelDetails, User.GetUserId());

                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                List<AttributeDefinition> removeDefinitionInfo = await _attributeDefinitionService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceModelId == info.Id &&
              info.Attributes != null && !info.Attributes.Select(a => a.Id).Contains(s.Id)).ToListAsync();
                if (removeDefinitionInfo.Count() != 0)
                {
                    foreach (var item in removeDefinitionInfo)
                    {
                        //check in DeviceAttributeValue pending
                        AttributeValue chkValueInfo = await _attributeValueService.GetSingleAsync(s => s.DeviceModelId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted);
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
                            definitionInfo.DeviceModelId = modelInfo.Id;
                            var definitionRespose = await _attributeDefinitionService.AddAsync(definitionInfo, User.GetUserId());

                            if (definitionRespose != null && !string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                valueInfo.IsActive = true;
                                valueInfo.DeviceModelId = modelInfo.Id;
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
                            defitionInfo.DeviceModelId = info.Id;
                            _attributeDefinitionService.Update(defitionInfo, _mapper.Map<AttributeDefinition>(item), User.GetUserId());

                            AttributeValue? chkValueInfo = await _attributeValueService.GetAll(s => s.DeviceModelId == info.Id && s.AttributeDefinitionId == item.Id && s.IsActive && !s.IsDeleted).FirstOrDefaultAsync();
                            if (!string.IsNullOrWhiteSpace(item.Value?.Trim()))
                            {
                                if (chkValueInfo != null)
                                {
                                    AttributeValue attributeValueInfo = _mapper.Map<AttributeValue>(chkValueInfo);
                                    attributeValueInfo.CreatedBy = chkValueInfo.CreatedBy;
                                    attributeValueInfo.CreatedDate = chkValueInfo.CreatedDate;
                                    attributeValueInfo.IsActive = chkValueInfo.IsActive;
                                    attributeValueInfo.Value = item.Value;
                                    attributeValueInfo.DeviceModelId = info.Id;
                                    attributeValueInfo.AttributeDefinitionId = item.Id;
                                    _attributeValueService.Update(attributeValueInfo, _mapper.Map<AttributeValue>(chkValueInfo), User.GetUserId());
                                }
                                else
                                {
                                    AttributeValue valueInfo = _mapper.Map<AttributeValue>(item);
                                    valueInfo.IsActive = true;
                                    valueInfo.DeviceModelId = modelInfo.Id;
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
        public async Task<BaseResponse> DeleteDeviceModel(Guid id)
        {
            DeviceModel modelDetail = await _deviceModelService.GetSingleAsync(s => s.Id == id && !s.IsDeleted);
            if (modelDetail != null)
            {
                bool isChkExist = _deviceService.GetAll(s => s.IsActive && !s.IsDeleted && s.DeviceModelId == id).Any();
                if (isChkExist)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleteAlreadyAssigned.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                modelDetail.IsDeleted = true;
                var response = _deviceModelService.Update(modelDetail, modelDetail, User.GetUserId(), true, true);
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
        public async Task<List<DropdownInfoDto>> GetDeviceInfoFromManufacturerId(Guid manufacturerId)
        {
            List<DropdownInfoDto> deviceModelInfo = await _deviceModelService.GetAll(s => s.IsActive && !s.IsDeleted && s.ManufacturerId == manufacturerId)
                .OrderBy(s => s.Model)
                .Select(s => new DropdownInfoDto
                {
                    Id = s.Id,
                    Name = s.Model
                }).ToListAsync();

            return deviceModelInfo;
        }
        #endregion


        [HttpPost]
        [AuthorizePermission(Operations.Add)]
        public async Task<ImportFileResultDto<DeviceModelListDto>> ImportDeviceModel([FromForm] FileUploadModel info)
        {
            List<DeviceModelListDto> responseList = [];
            if (info.File != null && info.File.Length > 0)
            {
                var typeHeaders = _csvImport.ReadFile(info.File, out FileType fileType);
                if (fileType == FileType.DeviceModel && typeHeaders != null)
                {
                    List<string> requiredKeys = FileHeadingConstants.DeviceModelHeadings;

                    foreach (var dictionary in typeHeaders)
                    {
                        var keys = dictionary.Keys.ToList();
                        if (requiredKeys.All(keys.Contains))
                        {
                            bool isSuccess = false;
                            List<string> message = [];

                            string? manufacturerName = dictionary[requiredKeys[2]];
                            Manufacturer? manufacturer = !string.IsNullOrEmpty(manufacturerName) ? await _manufacturerService.GetSingleAsync(x => x.Name.ToLower().Trim() == manufacturerName.ToLower().Trim() && !x.IsDeleted && x.IsActive) : null;

                            CreateOrEditDeviceModelDto createDto = new()
                            {
                                Model = dictionary[requiredKeys[0]],
                                Description = dictionary[requiredKeys[1]],
                                ManufacturerId = manufacturer?.Id ?? Guid.Empty,
                                Id = Guid.Empty
                            };

                            var helper = new CommonHelper();
                            Tuple<bool, List<string>> validationResponse = helper.CheckImportFileRecordValidations(createDto);
                            isSuccess = validationResponse.Item1;
                            if (!isSuccess)
                                message.AddRange(validationResponse.Item2);

                            if (manufacturer == null)
                            {
                                message.Add(ResponseMessages.ModuleNotValid.Replace("{module}", "manufacturer"));
                                if (isSuccess) isSuccess = false;
                            }

                            if (isSuccess)
                            {
                                bool isUpdate = false;
                                try
                                {
                                    DeviceModel existingModel = await _deviceModelService.GetSingleAsync(x => x.ManufacturerId == createDto.ManufacturerId && x.Model.ToLower().Trim() == createDto.Model.ToLower().Trim() && !x.IsDeleted && x.IsActive);

                                    if (message.Count == 0)
                                    {
                                        DeviceModel model = _mapper.Map<DeviceModel>(createDto);
                                        if (existingModel != null)
                                        {
                                            isUpdate = true;
                                            model.Id = existingModel.Id;
                                            model.CreatedBy = existingModel.CreatedBy;
                                            model.CreatedDate = existingModel.CreatedDate;
                                            var response = _deviceModelService.Update(model, existingModel, User.GetUserId());

                                            if (response == null)
                                                message.Add(ResponseMessages.ModuleNotUpdated.ToString().Replace("{module}", ModuleName));
                                        }
                                        else
                                        {
                                            var response = await _deviceModelService.AddAsync(model, User.GetUserId());

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


                            DeviceModelListDto record = _mapper.Map<DeviceModelListDto>(createDto);
                            record.Manufacturer = manufacturerName;
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
