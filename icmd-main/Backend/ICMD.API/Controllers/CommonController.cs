using AutoMapper;
using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Dtos.MemoryCache;
using ICMD.Core.Dtos.MetaData;
using ICMD.Core.Dtos.NonInstrument;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CommonController : BaseController
    {
        private readonly IEquipmentCodeService _equipmentCodeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProcessService _processService;
        private readonly IZoneService _zoneService;
        private readonly INatureOfSignalService _natureOfSignalService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IDeviceService _deviceService;
        private readonly IMemoryCache _memoryCache;
        private readonly IMetaDataService _metaDataService;
        private readonly IMapper _mapper;

        public CommonController(IEquipmentCodeService equipmentCodeService, IManufacturerService manufacturerService, IProcessService processService, IZoneService zoneService,
            INatureOfSignalService natureOfSignalService, IDeviceTypeService deviceTypeService, IDeviceService deviceService, IMemoryCache memoryCache, IMetaDataService metaDataService, IMapper mapper)
        {
            _equipmentCodeService = equipmentCodeService;
            _manufacturerService = manufacturerService;
            _processService = processService;
            _zoneService = zoneService;
            _natureOfSignalService = natureOfSignalService;
            _deviceTypeService = deviceTypeService;
            _deviceService = deviceService;
            _memoryCache = memoryCache;
            _metaDataService = metaDataService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<InstrumentDropdownInfoDto> GetInstrumentsDropdownInfo(Guid? projectId)
        {
            InstrumentDropdownInfoDto info = new InstrumentDropdownInfoDto();

            //EquipmentCode
            info.EquipmentCodeList = await _equipmentCodeService.GetAll(s => s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Code }).ToListAsync();

            //Manufacturer
            info.ManufacturerList = await _manufacturerService.GetAll(s => s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Name }).ToListAsync();

            //Process
            info.ProcessList = await _processService.GetAll(s => s.ProjectId == projectId && s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.ProcessName }).ToListAsync();

            //Zone
            info.ZoneList = await _zoneService.GetAll(s => s.ProjectId == projectId && s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Zone }).ToListAsync();

            //NatureOfSignal
            info.NatureOfSignalList = await _natureOfSignalService.GetAll(s => s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.NatureOfSignalName }).ToListAsync();

            //PLCNumber
            info.PLCNumberList = await (from dt in _deviceTypeService.GetAll(s => s.IsActive && !s.IsDeleted)
                                        join dv in _deviceService.GetAll() on dt.Id equals dv.DeviceTypeId
                                        where dt.Type.ToLower().Equals("plc") && dv.Tag.ProjectId == projectId
                                        select new DropdownInfoDto
                                        {
                                            Id = dt.Id,
                                            Name = dv.Tag.TagName
                                        }).ToListAsync();

            //Tag
            info.TagList = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted &&
             (s.IsInstrument.Equals("B") || s.IsInstrument.Equals("Y"))
            ).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Tag.TagName }).ToListAsync();


            return info;
        }

        [HttpGet]
        public async Task<NonInstrumentDropdownInfoDto> GetNonInstrumentsDropdownInfo(Guid? projectId)
        {
            NonInstrumentDropdownInfoDto info = new NonInstrumentDropdownInfoDto();

            //Tag
            info.TagList = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted &&
             (s.IsInstrument.Equals("B") || s.IsInstrument.Equals("N"))
            ).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Tag.TagName }).ToListAsync();

            //PLCNumber
            info.PLCNumberList = await (from dt in _deviceTypeService.GetAll(s => s.IsActive && !s.IsDeleted)
                                        join dv in _deviceService.GetAll() on dt.Id equals dv.DeviceTypeId
                                        where dt.Type.ToLower().Equals("plc") && dv.Tag.ProjectId == projectId
                                        select new DropdownInfoDto
                                        {
                                            Id = dt.Id,
                                            Name = dv.Tag.TagName
                                        }).ToListAsync();

            //EquipmentCode
            info.EquipmentCodeList = await _equipmentCodeService.GetAll(s => s.IsActive && !s.IsDeleted).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.Code }).ToListAsync();

            //Location
            info.LocationList = await _deviceService.GetAll(s => s.Tag.ProjectId == projectId && s.IsActive && !s.IsDeleted &&
            (s.IsInstrument.Equals("B") || s.IsInstrument.Equals("N"))
           ).Select(s => new DropdownInfoDto { Id = s.Id, Name = s.PanelTag != null ? s.PanelTag.TagName : "" }).ToListAsync();


            return info;
        }

        [HttpGet]
        public async Task<object?> GetMemoryCacheItem(string key)
        {
            string[] stringList = Array.Empty<string>();
            return await Task.Run(() =>
            {
                object? data = _memoryCache.Get($"{User.GetUserId()}_{key}");

                if (data != null && data?.ToString() != null)
                    stringList = JsonSerializer.Deserialize<string[]>(data?.ToString()) ?? Array.Empty<string>();

                return stringList;
            });
        }

        [HttpPost]
        public async Task SetMemoryCache(AddMemoryCacheDto cacheDto)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            await Task.Run(() =>
            {
                _memoryCache.Set($"{User.GetUserId()}_{cacheDto.Key}", cacheDto.Value != null ? JsonSerializer.Serialize(cacheDto.Value, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }) : "", cacheEntryOptions);
            });
        }

        #region Column Selector
        [HttpGet]
        public async Task<List<MetaDataDto>> GetColumnTemplate()
        {
            List<MetaData> metaDataList = await _metaDataService.GetAll(x => x.Property.StartsWith(ResponseMessages.PrefixMetaDataTableForColumnTemplate) && x.IsActive && !x.IsDeleted).ToListAsync();


            List<MetaDataDto> metaData = _mapper.Map<List<MetaDataDto>>(metaDataList);
            foreach (var template in metaData)
            {
                if (MetaDataColumnTemplate.DefaultTemplates.Contains(template.Property))
                    template.IsDefault = true;

                template.Property = template.Property.Replace(ResponseMessages.PrefixMetaDataTableForColumnTemplate, string.Empty);
            }
            return metaData;
        }

        [HttpGet]
        public async Task<BaseResponse> DeleteColumnTemplate(Guid templateId)
        {
            string ModuleName = "MetaData";
            MetaData? metaData = await _metaDataService.GetSingleAsync(x => x.Id == templateId && x.IsActive && !x.IsDeleted);
            if (metaData != null)
            {
                metaData.IsDeleted = true;
                var response = _metaDataService.Update(metaData, metaData, User.GetUserId(), true, true);
                if (response == null)
                    return new BaseResponse(false, ResponseMessages.ModuleNotDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.InternalServerError);

                return new BaseResponse(true, ResponseMessages.ModuleDeleted.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK);
            }
            else
            {
                return new BaseResponse(false, ResponseMessages.ModuleNotExist.ToString().Replace("{module}", ModuleName), HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public async Task<BaseResponse> CreateColumnTemplate(CreateMetaDataDto metaDataDto)
        {
            string ModuleName = "Template";
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(metaDataDto.Value))
                        return new BaseResponse(false, ResponseMessages.ColumnNotSelected.ToString(), HttpStatusCode.InternalServerError);


                    MetaData existing = await _metaDataService.GetSingleAsync(x => x.Property == $"{ResponseMessages.PrefixMetaDataTableForColumnTemplate}{metaDataDto.TemplateName}" && x.IsActive && !x.IsDeleted);
                    if (existing != null)
                        return new BaseResponse(false, ResponseMessages.ModuleAlreadyTaken.Replace("{module}", ModuleName), HttpStatusCode.Conflict);

                    MetaData metaData = _mapper.Map<MetaData>(metaDataDto);
                    metaData.Property = $"{ResponseMessages.PrefixMetaDataTableForColumnTemplate}{metaDataDto.TemplateName}";
                    metaData.IsActive = true;
                    var response = await _metaDataService.AddAsync(metaData, User.GetUserId());

                    if (response == null)
                        return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);

                    return new BaseResponse(true, ResponseMessages.ModuleCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.OK, response.Id);
                }
                catch (Exception ex)
                {
                    return new BaseResponse(false, ResponseMessages.ModuleNotCreated.ToString().Replace("{module}", ModuleName), HttpStatusCode.NoContent);
                }
            }
            else
                return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }
        #endregion
    }
}
