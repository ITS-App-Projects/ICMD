using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Authorization;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Change_Log;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Dtos.Node_Address;
using ICMD.Core.Dtos.PnIdTags;
using ICMD.Core.Dtos.Reports;
using ICMD.Core.Dtos.Spares;
using ICMD.Core.Dtos.Tag;
using ICMD.Core.Shared.Interface;
using ICMD.Core.ViewDto;
using ICMD.Repository.ViewService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private readonly IChangeLogService _changeLogService;
        private readonly UserManager<ICMDUser> _userManager;
        private readonly ViewCountDPPADeviceService _viewCountDPPADeviceService;
        private readonly StoredProcedureHelper _storedProcedureHelper;
        private readonly ViewPnIDTagExceptionService _viewPnIDTagExceptionService;
        private readonly ViewPnIDDeviceDocumentReferenceCompareService _viewPnIDDeviceDocumentReferenceCompareService;
        private readonly ViewInstrumentListLiveService _viewInstrumentListLiveService;
        private readonly ViewNonInstrumentListService _viewNonInstrumentListService;
        private readonly ViewOMItemInstrumentListService _viewOMItemInstrumentListService;
        private readonly ViewUnassociatedTagsService _viewUnassociatedTagsService;
        private readonly ViewUnassociatedSkidsService _viewUnassociatedSkidsService;
        private readonly ViewUnassociatedStandsService _viewUnassociatedStandsService;
        private readonly ViewUnassociatedJunctionBoxesService _viewUnassociatedJunctionBoxesService;
        private readonly ViewUnassociatedPanelsService _viewUnassociatedPanelsService;
        private readonly IProcessService _processService;
        private readonly ISubProcessService _subProcessService;
        private readonly IStreamService _streamService;
        private readonly IEquipmentCodeService _equipmentCodeService;
        private readonly ViewNatureOfSignalValidationFailuresService _viewNatureOfSignalValidationFailuresService;
        private readonly ViewPSSTagsService _viewPSSTagsService;

        public ReportController(IReportService reportService, IMapper mapper, IChangeLogService changeLogService, UserManager<ICMDUser> userManager,
            ViewCountDPPADeviceService viewCountDPPADeviceService, StoredProcedureHelper storedProcedureHelper,
            ViewPnIDTagExceptionService viewPnIDTagExceptionService,
            ViewPnIDDeviceDocumentReferenceCompareService viewPnIDDeviceDocumentReferenceCompareService,
            ViewInstrumentListLiveService viewInstrumentListLiveService, ViewNonInstrumentListService viewNonInstrumentListService,
            ViewOMItemInstrumentListService viewOMItemInstrumentListService, ViewUnassociatedTagsService viewUnassociatedTagsService,
            ViewUnassociatedSkidsService viewUnassociatedSkidsService, IProcessService processService, ISubProcessService subProcessService,
            IStreamService streamService, IEquipmentCodeService equipmentCodeService, ViewUnassociatedStandsService viewUnassociatedStandsService,
            ViewUnassociatedJunctionBoxesService viewUnassociatedJunctionBoxesService, ViewUnassociatedPanelsService viewUnassociatedPanelsService,
            ViewNatureOfSignalValidationFailuresService viewNatureOfSignalValidationFailuresService,
            ViewPSSTagsService viewPSSTagsService)
        {
            _reportService = reportService;
            _mapper = mapper;
            _changeLogService = changeLogService;
            _userManager = userManager;
            _viewCountDPPADeviceService = viewCountDPPADeviceService;
            _storedProcedureHelper = storedProcedureHelper;
            _viewPnIDTagExceptionService = viewPnIDTagExceptionService;
            _viewPnIDDeviceDocumentReferenceCompareService = viewPnIDDeviceDocumentReferenceCompareService;
            _viewInstrumentListLiveService = viewInstrumentListLiveService;
            _viewNonInstrumentListService = viewNonInstrumentListService;
            _viewOMItemInstrumentListService = viewOMItemInstrumentListService;
            _viewUnassociatedTagsService = viewUnassociatedTagsService;
            _viewUnassociatedSkidsService = viewUnassociatedSkidsService;
            _processService = processService;
            _subProcessService = subProcessService;
            _streamService = streamService;
            _equipmentCodeService = equipmentCodeService;
            _viewUnassociatedStandsService = viewUnassociatedStandsService;
            _viewUnassociatedJunctionBoxesService = viewUnassociatedJunctionBoxesService;
            _viewUnassociatedPanelsService = viewUnassociatedPanelsService;
            _viewNatureOfSignalValidationFailuresService = viewNatureOfSignalValidationFailuresService;
            _viewPSSTagsService = viewPSSTagsService;
        }

        #region Reports
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ReportListDto>> GetReportList()
        {
            List<Report> getallReports = await _reportService.GetAll(a => a.IsActive && !a.IsDeleted).ToListAsync();
            List<ReportInfoDto> reports = _mapper.Map<List<ReportInfoDto>>(getallReports);
            List<ReportListDto> typeLogsData = reports.ToLookup(a => a.Group).Select(a => new ReportListDto
            {
                Group = a.Key,
                Items = a.ToList()
            }).ToList();
            return typeLogsData.OrderBy(a => a.Group).ToList();
        }
        #endregion

        #region AggregatesReport
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<DPPADevicesDto>> GetDPPADevicesData(Guid projectId)
        {
            List<DPPADevicesDto> devices = new List<DPPADevicesDto>();
            List<ViewCountDPPADevicesDto> allDevices = await _viewCountDPPADeviceService.GetAll(x => x.ProjectId == projectId).ToListAsync();
            foreach (var item in allDevices.ToLookup(a => a.PLCNumber).ToList())
            {
                devices.Add(new DPPADevicesDto()
                {
                    PLCNumber = item.Key,
                    NoOfDPDevices = item.ToList().Sum(a => a.NoOfDPDevices),
                    NoOfPADevices = item.ToList().Sum(a => a.NoOfPADevices),
                });
                foreach (var slotitem in item.ToList().ToLookup(a => a.PLCSlotNumber))
                {
                    DPPADevicesDto currentDevice = devices[devices.Count - 1];
                    currentDevice.ChildInfo.Add(new DPPADevicesDto()
                    {
                        PLCSlotNumber = slotitem.Key,
                        NoOfDPDevices = slotitem.ToList().Sum(a => a.NoOfDPDevices),
                        NoOfPADevices = slotitem.ToList().Sum(a => a.NoOfPADevices),
                    });
                    foreach (var couplerItem in slotitem.ToList().ToLookup(a => a.DPPACoupler))
                    {
                        DPPADevicesDto childDevice = currentDevice.ChildInfo[currentDevice.ChildInfo.Count - 1];
                        childDevice.ChildInfo.Add(new DPPADevicesDto()
                        {
                            DPPACoupler = couplerItem.Key,
                            NoOfDPDevices = couplerItem.ToList().Sum(a => a.NoOfDPDevices),
                            NoOfPADevices = couplerItem.ToList().Sum(a => a.NoOfPADevices),
                        });
                        foreach (var hubItem in couplerItem.ToList().ToLookup(a => a.AFDHubNumber))
                        {
                            DPPADevicesDto hubDevice = childDevice.ChildInfo[childDevice.ChildInfo.Count - 1];
                            hubDevice.ChildInfo.Add(new DPPADevicesDto
                            {
                                AFDHubNumber = hubItem.Key,
                                NoOfDPDevices = hubItem.ToList().Sum(a => a.NoOfDPDevices),
                                NoOfPADevices = hubItem.ToList().Sum(a => a.NoOfPADevices),
                            });
                        }
                    }
                }
            }
            return devices;
        }
        #endregion

        #region AuditReport
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ChangeLogListDto>> GetAuditLogData(Guid projectId)
        {
            List<ChangeLogInfoDto> changeLogItems = await (from uc in _changeLogService.GetAll(c => c.ProjectId == projectId &&
                                                           c.IsActive && !c.IsDeleted)
                                                           join um in _userManager.Users on uc.CreatedBy equals um.Id
                                                           select new ChangeLogInfoDto
                                                           {
                                                               Context = uc.Context,
                                                               ContextId = uc.ContextId,
                                                               EntityName = uc.EntityName,
                                                               Status = uc.Status,
                                                               OriginalValues = uc.OriginalValues,
                                                               NewValues = uc.NewValues,
                                                               CreatedDate = uc.CreatedDate,
                                                               CreatedBy = um.FullName
                                                           }).OrderByDescending(a => a.CreatedDate).ToListAsync();

            List<ChangeLogListDto> typeLogsData = changeLogItems.ToLookup(a => a.Context).Select(a => new ChangeLogListDto
            {
                Key = a.Key,
                Items = a.ToList()
            }).ToList();

            return typeLogsData;
        }
        #endregion

        #region DuplicateReport
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<DuplicateDPNodeAddressDto>> GetDuplicateDPNodeAddress(Guid? projectId)
        {
            List<DuplicateDPNodeAddressDto> data = new List<DuplicateDPNodeAddressDto>();
            List<ViewInstrumentListLiveDto> nodeAddresses = await _storedProcedureHelper.GetDuplicateReportsData(@"public.""spDuplicateDPNodeAddress""", projectId);
            foreach (var item in nodeAddresses.Where(a => a.IsActive && !a.IsDeleted).ToLookup(a => a.DPDPCoupler).ToList())
            {
                data.Add(new DuplicateDPNodeAddressDto()
                {
                    DPDPCoupler = item.Key,
                    Items = item.ToList()
                });
            }
            return data;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<DuplicatePANodeAddressDto>> GetDuplicatePANodeAddress(Guid? projectId)
        {
            List<DuplicatePANodeAddressDto> data = new List<DuplicatePANodeAddressDto>();
            List<ViewInstrumentListLiveDto> nodeAddresses = await _storedProcedureHelper.GetDuplicateReportsData(@"public.""spDuplicatePANodeAddress""", projectId);
            foreach (var item in nodeAddresses.Where(a => a.IsActive && !a.IsDeleted).ToLookup(a => a.DPPACoupler).ToList())
            {
                data.Add(new DuplicatePANodeAddressDto()
                {
                    DPPACoupler = item.Key,
                    Items = item.ToList()
                });
            }
            return data;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<DuplicateRackSlotChannelDto>> GetDuplicateRackSlotChannelData(Guid? projectId)
        {
            List<DuplicateRackSlotChannelDto> data = new List<DuplicateRackSlotChannelDto>();
            List<ViewInstrumentListLiveDto> nodeAddresses = await _storedProcedureHelper.GetDuplicateReportsData(@"public.""spDuplicateRackSlotChannels""", projectId);
            foreach (var item in nodeAddresses.Where(a => !a.IsDeleted).ToLookup(a => a.RackNo).ToList())
            {
                data.Add(new DuplicateRackSlotChannelDto()
                {
                    RackNo = item.Key,
                    Items = item.ToList()
                });
            }
            return data;
        }
        #endregion

        #region ExceptionReport
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<PnIDTagExceptionInfoDto>> GetPnIdTagExceptionData(Guid? projectId)
        {
            List<PnIDTagExceptionInfoDto> data = new List<PnIDTagExceptionInfoDto>();
            List<ViewPnIDTagExceptionDto> pnIdTags = await _viewPnIDTagExceptionService.GetAll(a => a.ProjectId == projectId).ToListAsync();
            foreach (var item in pnIdTags.ToLookup(a => a.EquipmentCode))
            {
                data.Add(new PnIDTagExceptionInfoDto()
                {
                    Key = item.Key,
                    Items = item.ToList()
                });
            }
            return data.OrderBy(a => a.Key).ToList();
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<List<ViewPnIDDeviceDocumentReferenceCompareDto>> GetPnIdMisMatchedDocumentReferenceData(PnIdDeviceDocumentReferenceRequestDto info)
        {
            List<ViewPnIDDeviceDocumentReferenceCompareDto> documentReferenceData = await _viewPnIDDeviceDocumentReferenceCompareService.GetAll(a => a.ProjectId == info.ProjectId).OrderBy(a => a.Tag).ToListAsync();

            if (info.Type == PnIdDeviceMisMatchDocumentReference.PnID_Device_MismatchedDocumentNumber)
            {
                documentReferenceData = documentReferenceData.Where(a => a.DocumentNumber != a.PnIdDocumentNumber).ToList();
            }
            else if (info.Type == PnIdDeviceMisMatchDocumentReference.PnID_Device_MismatchedDocumentNumber_VersionRevision)
            {
                documentReferenceData = documentReferenceData.Where(a => (a.DocumentNumber != a.PnIdDocumentNumber) ||
                (a.Revision != a.PnIdRevision) || (a.Version != a.PnIdVersion)).ToList();
            }
            else if (info.Type == PnIdDeviceMisMatchDocumentReference.PnID_Device_MismatchedDocumentNumber_VersionRevisionInclNulls)
            {
                documentReferenceData = documentReferenceData.Where(a => (a.DocumentNumber != a.PnIdDocumentNumber) ||
                ((a.Revision ?? "") != (a.PnIdRevision ?? "")) || ((a.Version ?? "") != (a.PnIdVersion ?? ""))).ToList();
            }
            return documentReferenceData;
        }
        #endregion

        #region ListReport
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewInstrumentListLiveDto>> GetInstrumentListData(Guid projectId)
        {
            List<ViewInstrumentListLiveDto> allInstruments = await _viewInstrumentListLiveService.GetAll(x =>
            x.ProjectId == projectId && !x.IsDeleted).OrderBy(a => a.TagName).ToListAsync();
            return allInstruments;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewNonInstrumentListDto>> GetNonInstrumentListData(Guid projectId)
        {
            List<ViewNonInstrumentListDto> allNonInstruments = await _viewNonInstrumentListService.GetAll(x =>
            x.ProjectId == projectId && !x.IsDeleted).OrderBy(a => a.TagName).ToListAsync();
            return allNonInstruments;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewOMItemInstrumentListDto>> GetOMItemInstrumentListData(Guid projectId)
        {
            List<ViewOMItemInstrumentListDto> allInstruments = new List<ViewOMItemInstrumentListDto>();
            allInstruments = await _viewOMItemInstrumentListService.GetAll(x =>
       x.ProjectId == projectId && !x.IsDeleted).ToListAsync();
            allInstruments = allInstruments.OrderBy(a => a.ItemId).OrderBy(a => a.Tag).ToList();
            return allInstruments;
        }
        #endregion

        #region Spares Report
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<SparesReportResponceDto>> GetSparesData(Guid? projectId)
        {
            List<SparesReportDto> sparesData = await _storedProcedureHelper.GetSparesReportData(@"public.""spSparesReport""", projectId);
            List<SparesReportResponceDto> data = new List<SparesReportResponceDto>();
            int? totalChannels = 0;
            int totalUsedChannels = 0;
            decimal? totalSpareChannels = 0;
            decimal? totalpercentUsed = 0;
            decimal? totalpercentSpare = 0;
            foreach (var item in sparesData.ToLookup(a => a.PLCNumber).ToList())
            {
                totalChannels += item.ToList().Sum(a => a.TotalChanneles);
                totalUsedChannels += item.ToList().Sum(a => a.UsedChanneles);
                decimal? SpareChannels = 0;
                foreach (var test in item.ToList())
                {
                    if (test.TotalChanneles != null && test.TotalChanneles != 0)
                    {
                        int? channel = test.TotalChanneles > test.UsedChanneles ? test.TotalChanneles - test.UsedChanneles : 0;
                        SpareChannels += channel;
                    }
                }
                int? totalPLCChannels = item.ToList().Sum(a => a.TotalChanneles) ?? 0;
                int usedPLCChannels = item.ToList().Sum(a => a.UsedChanneles);
                totalSpareChannels += SpareChannels;
                totalpercentUsed += totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0) : 0;
                totalpercentSpare += totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : 0;
                data.Add(new SparesReportResponceDto()
                {
                    PLCNumber = item.Key,
                    TotalChanneles = item.ToList().Sum(a => a.TotalChanneles) ?? 0,
                    UsedChanneles = item.ToList().Sum(a => a.UsedChanneles),
                    SpareChannels = SpareChannels,
                    PercentUsed = totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0) : totalPLCChannels,
                    PercentSpare = totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : totalPLCChannels,
                });
                foreach (var rackItem in item.ToList().ToLookup(a => a.Rack).ToList())
                {
                    SpareChannels = 0;
                    foreach (var racktest in rackItem.ToList())
                    {
                        if (racktest.TotalChanneles != null && racktest.TotalChanneles != 0)
                        {
                            int? channel = racktest.TotalChanneles > racktest.UsedChanneles ? racktest.TotalChanneles - racktest.UsedChanneles : 0;
                            SpareChannels += channel;
                        }
                    }
                    int? totalRackChannels = rackItem.ToList().Sum(a => a.TotalChanneles) ?? 0;
                    int usedRackChannels = rackItem.ToList().Sum(a => a.UsedChanneles);
                    SparesReportResponceDto rack = data[data.Count - 1];
                    rack.ChildItems.Add(new SparesReportResponceDto()
                    {
                        Rack = rackItem.Key,
                        TotalChanneles = rackItem.ToList().Sum(a => a.TotalChanneles) ?? 0,
                        UsedChanneles = rackItem.ToList().Sum(a => a.UsedChanneles),
                        SpareChannels = SpareChannels,
                        PercentUsed = totalRackChannels != null && totalRackChannels != 0 ? Math.Round(((decimal)usedRackChannels / totalRackChannels * 100) ?? 0) : totalRackChannels,
                        PercentSpare = totalRackChannels != null && totalRackChannels != 0 ? Math.Round(((decimal)SpareChannels / totalRackChannels * 100) ?? 0) : totalRackChannels,
                        ChildItems = (rackItem.ToList()).Select(a => new SparesReportResponceDto
                        {
                            NatureOfSignal = a.NatureOfSignal,
                            TotalChanneles = a.TotalChanneles,
                            UsedChanneles = a.UsedChanneles,
                            SpareChannels = a.TotalChanneles != null && a.TotalChanneles != 0 ? (a.TotalChanneles > a.UsedChanneles ? a.TotalChanneles - a.UsedChanneles : 0) : a.TotalChanneles,
                            PercentUsed = a.TotalChanneles != null && a.TotalChanneles != 0 ?
                            Math.Round((a.TotalChanneles == null ? 0 : (decimal)a.UsedChanneles / a.TotalChanneles * 100) ?? 0) : a.TotalChanneles,
                            PercentSpare = a.TotalChanneles != null && a.TotalChanneles != 0 ?
                            Math.Round((a.TotalChanneles == null ? 0 : (decimal)((decimal)a.TotalChanneles > a.UsedChanneles ? a.TotalChanneles - a.UsedChanneles : 0) / a.TotalChanneles * 100) ?? 0) : a.TotalChanneles,
                        }).ToList()
                    });
                }
            }
            data.Add(new SparesReportResponceDto()
            {
                PLCNumber = "All PLCs",
                TotalChanneles = totalChannels,
                UsedChanneles = totalUsedChannels,
                SpareChannels = totalSpareChannels,
                PercentUsed = totalpercentUsed,
                PercentSpare = totalpercentSpare
            });
            return data;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<SparesReportDetailsResponceDto>> GetSparesDetailsData(Guid? projectId)
        {
            List<SparesReportDetailsDto> sparesData = await _storedProcedureHelper.GetSparesReportDetailsData(@"public.""spSparesReportNoSDetail""", projectId);
            List<SparesReportDetailsResponceDto> data = new List<SparesReportDetailsResponceDto>();
            int? totalChannels = 0;
            int totalUsedChannels = 0;
            decimal? totalSpareChannels = 0;
            decimal? totalpercentUsed = 0;
            decimal? totalpercentSpare = 0;
            foreach (var item in sparesData.ToLookup(a => a.PLCNumber).ToList())
            {
                totalChannels += item.ToList().Sum(a => a.TotalChanneles);
                totalUsedChannels += item.ToList().Sum(a => a.UsedChanneles);
                decimal? SpareChannels = 0;
                foreach (var test in item.ToList())
                {
                    if (test.TotalChanneles != null && test.TotalChanneles != 0)
                    {
                        int? channel = test.TotalChanneles > test.UsedChanneles ? test.TotalChanneles - test.UsedChanneles : 0;
                        SpareChannels += channel;
                    }
                }
                int? totalPLCChannels = item.ToList().Sum(a => a.TotalChanneles) ?? 0;
                int usedPLCChannels = item.ToList().Sum(a => a.UsedChanneles);
                totalSpareChannels += SpareChannels;
                totalpercentUsed += totalPLCChannels != null && totalPLCChannels != 0 ?
                    usedPLCChannels > totalPLCChannels ? 100 : (Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0)) : 0;
                totalpercentSpare += totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : 0;
                data.Add(new SparesReportDetailsResponceDto()
                {
                    PLCNumber = item.Key,
                    TotalChanneles = item.ToList().Sum(a => a.TotalChanneles) ?? 0,
                    UsedChanneles = item.ToList().Sum(a => a.UsedChanneles),
                    SpareChannels = SpareChannels,
                    PercentUsed = totalPLCChannels != null && totalPLCChannels != 0 ?
                        usedPLCChannels > totalPLCChannels ? 100 : (Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0)) : totalPLCChannels,
                    PercentSpare = totalPLCChannels != null && totalPLCChannels != 0 ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : totalPLCChannels
                });
                foreach (var rackItem in item.ToList().ToLookup(a => a.Rack).ToList())
                {
                    SpareChannels = 0;

                    foreach (var racktest in rackItem.ToList())
                    {
                        if (racktest.TotalChanneles != null && racktest.TotalChanneles != 0)
                        {
                            int? channel = racktest.TotalChanneles > racktest.UsedChanneles ? racktest.TotalChanneles - racktest.UsedChanneles : 0;
                            SpareChannels += channel;
                        }
                    }
                    int? totalRackChannels = rackItem.ToList().Sum(a => a.TotalChanneles) ?? 0;
                    int usedRackChannels = rackItem.ToList().Sum(a => a.UsedChanneles);
                    SparesReportDetailsResponceDto rack = data[data.Count - 1];
                    rack.ChildItems.Add(new SparesReportDetailsResponceDto()
                    {
                        Rack = rackItem.Key,
                        TotalChanneles = rackItem.ToList().Sum(a => a.TotalChanneles) ?? 0,
                        UsedChanneles = rackItem.ToList().Sum(a => a.UsedChanneles),
                        SpareChannels = SpareChannels,
                        PercentUsed = totalRackChannels != 0 && totalRackChannels != null ?
                        usedRackChannels > totalRackChannels ? 100 : Math.Round(((decimal)usedRackChannels / totalRackChannels * 100) ?? 0) : totalRackChannels,
                        PercentSpare = totalRackChannels != 0 && totalRackChannels != null ? Math.Round(((decimal)SpareChannels / totalRackChannels * 100) ?? 0) : totalRackChannels
                    });
                    foreach (var natureItem in rackItem.ToList().ToLookup(a => a.NatureOfSignal).ToList())
                    {
                        SparesReportDetailsResponceDto natureInfo = rack.ChildItems[rack.ChildItems.Count - 1];
                        SpareChannels = 0;

                        foreach (var natureTest in natureItem.ToList())
                        {
                            if (natureTest.TotalChanneles != null && natureTest.TotalChanneles != 0)
                            {
                                int? channel = natureTest.TotalChanneles > natureTest.UsedChanneles ? natureTest.TotalChanneles - natureTest.UsedChanneles : 0;
                                SpareChannels += channel;
                            }
                        }
                        int? totalNatureChannels = natureItem.ToList().Sum(a => a.TotalChanneles) ?? 0;
                        int usedNatureChannels = natureItem.ToList().Sum(a => a.UsedChanneles);
                        natureInfo.ChildItems.Add(new SparesReportDetailsResponceDto()
                        {
                            NatureOfSignal = natureItem.Key,
                            TotalChanneles = natureItem.ToList().Sum(a => a.TotalChanneles) ?? 0,
                            UsedChanneles = natureItem.ToList().Sum(a => a.UsedChanneles),
                            SpareChannels = SpareChannels,
                            PercentUsed = totalNatureChannels != 0 && totalNatureChannels != null ?
                            usedNatureChannels > totalNatureChannels ? 100 : Math.Round(((decimal)usedNatureChannels / totalNatureChannels * 100) ?? 0) : totalNatureChannels,
                            PercentSpare = totalNatureChannels != 0 && totalNatureChannels != null ? Math.Round(((decimal)SpareChannels / totalNatureChannels * 100) ?? 0) : totalNatureChannels,
                            ChildItems = (natureItem.ToList()).Select(a => new SparesReportDetailsResponceDto
                            {
                                SlotNumber = a.SlotNumber,
                                TotalChanneles = a.TotalChanneles,
                                UsedChanneles = a.UsedChanneles,
                                SpareChannels = a.TotalChanneles > a.UsedChanneles ? a.TotalChanneles - a.UsedChanneles : 0,
                                PercentUsed = a.TotalChanneles != 0 && a.TotalChanneles != null ?
                            (a.UsedChanneles > a.TotalChanneles ? 100 : Math.Round((a.TotalChanneles == null ? 0 : (decimal)a.UsedChanneles / a.TotalChanneles * 100) ?? 0)) : a.TotalChanneles,
                                PercentSpare = a.TotalChanneles != 0 && a.TotalChanneles != null ?
                            Math.Round((a.TotalChanneles == null ? 0 : (decimal)((decimal)a.TotalChanneles > a.UsedChanneles ? a.TotalChanneles - a.UsedChanneles : 0) / a.TotalChanneles * 100) ?? 0) : a.TotalChanneles,
                            }).ToList()
                        });
                    }
                }
            }
            return data;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<SparesReportPLCResponceDto>> GetSparesPLCData(Guid? projectId)
        {
            List<SparesReportPLCDto> sparesData = await _storedProcedureHelper.GetSparesReportPLCData(@"public.""spSparesReportPLCSummary""", projectId);
            List<SparesReportPLCResponceDto> data = new List<SparesReportPLCResponceDto>();
            int? totalChannels = 0;
            int totalUsedChannels = 0;
            decimal? totalSpareChannels = 0;
            decimal? totalpercentUsed = 0;
            decimal? totalpercentSpare = 0;
            foreach (var item in sparesData.ToLookup(a => a.PLCNumber).ToList())
            {
                totalChannels += item.ToList().Sum(a => a.TotalChanneles);
                totalUsedChannels += item.ToList().Sum(a => a.UsedChanneles);
                decimal? SpareChannels = 0;
                foreach (var test in item.ToList())
                {
                    if (test.TotalChanneles != null && test.TotalChanneles != 0)
                    {
                        int? channel = test.TotalChanneles > test.UsedChanneles ? test.TotalChanneles - test.UsedChanneles : 0;
                        SpareChannels += channel;
                    }
                }
                int? totalPLCChannels = item.ToList().Sum(a => a.TotalChanneles) ?? 0;
                int usedPLCChannels = item.ToList().Sum(a => a.UsedChanneles);
                totalSpareChannels += totalPLCChannels - usedPLCChannels;
                totalpercentUsed += totalPLCChannels != 0 && totalPLCChannels != null ? (usedPLCChannels > totalPLCChannels ? 100 : Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0)) : 0;
                totalpercentSpare += totalPLCChannels != 0 && totalPLCChannels != null ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : 0;
                data.Add(new SparesReportPLCResponceDto()
                {
                    PLCNumber = item.Key,
                    TotalChanneles = item.ToList().Sum(a => a.TotalChanneles) ?? 0,
                    UsedChanneles = item.ToList().Sum(a => a.UsedChanneles),
                    SpareChannels = SpareChannels,
                    PercentUsed = totalPLCChannels != 0 && totalPLCChannels != null ? (usedPLCChannels > totalPLCChannels ? 100 : Math.Round(((decimal)usedPLCChannels / totalPLCChannels * 100) ?? 0)) : 0,
                    PercentSpare = totalPLCChannels != 0 && totalPLCChannels != null ? Math.Round(((decimal)SpareChannels / totalPLCChannels * 100) ?? 0) : 0,
                });
                foreach (var natureItem in item.ToList().ToLookup(a => a.NatureOfSignal).ToList())
                {
                    SpareChannels = 0;
                    foreach (var racktest in natureItem.ToList())
                    {
                        if (racktest.TotalChanneles != null && racktest.TotalChanneles != 0)
                        {
                            int? channel = racktest.TotalChanneles > racktest.UsedChanneles ? racktest.TotalChanneles - racktest.UsedChanneles : 0;
                            SpareChannels += channel;
                        }
                    }
                    int? totalRackChannels = natureItem.ToList().Sum(a => a.TotalChanneles) ?? 0;
                    int usedRackChannels = natureItem.ToList().Sum(a => a.UsedChanneles);
                    SparesReportPLCResponceDto rack = data[data.Count - 1];
                    rack.ChildItems.Add(new SparesReportPLCResponceDto()
                    {
                        NatureOfSignal = natureItem.Key,
                        TotalChanneles = natureItem.ToList().Sum(a => a.TotalChanneles) ?? 0,
                        UsedChanneles = natureItem.ToList().Sum(a => a.UsedChanneles),
                        SpareChannels = SpareChannels,
                        PercentUsed = totalRackChannels != 0 && totalRackChannels != null ? (usedRackChannels > totalRackChannels ? 100 : Math.Round(((decimal)usedRackChannels / totalRackChannels * 100) ?? 0)) : totalRackChannels,
                        PercentSpare = totalRackChannels != 0 && totalRackChannels != null ? Math.Round(((decimal)SpareChannels / totalRackChannels * 100) ?? 0) : totalRackChannels,
                    });
                }
            }

            return data;
        }
        #endregion

        #region Unassociated
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<UnassociatedTagsDto>> GetUnassociatedTagsData(Guid projectId)
        {
            List<ViewUnassociatedTagsDto> unassociatedTags = await _viewUnassociatedTagsService.GetAll(a => a.ProjectId == projectId).ToListAsync();
            List<UnassociatedTagsDto> data = (from tg in unassociatedTags
                                              join p in _processService.GetAll() on tg.ProcessId equals p.Id into Process
                                              from p in Process.DefaultIfEmpty()
                                              join sp in _subProcessService.GetAll() on tg.SubProcessId equals sp.Id into SubProcess
                                              from sp in SubProcess.DefaultIfEmpty()
                                              join st in _streamService.GetAll() on tg.StreamId equals st.Id into Stream
                                              from st in Stream.DefaultIfEmpty()
                                              join ec in _equipmentCodeService.GetAll() on tg.EquipmentCodeId equals ec.Id into Code
                                              from ec in Code.DefaultIfEmpty()
                                              select new UnassociatedTagsDto
                                              {
                                                  Tag = tg.TagName,
                                                  Process = p?.ProcessName ?? "",
                                                  SubProcess = sp?.SubProcessName ?? "",
                                                  Stream = st?.StreamName ?? "",
                                                  EquipmentCode = ec?.Code ?? "",
                                                  SequenceNumber = tg.SequenceNumber,
                                                  EquipmentIdentifier = tg.EquipmentIdentifier,
                                                  DocumentNumber = tg.DocumentNumber,
                                                  Revision = tg.Revision,
                                                  Version = tg.Version
                                              }).OrderBy(a => a.Tag).ToList();
            return data;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewUnassociatedSkidsDto>> GetUnassociatedSkidsData(Guid projectId)
        {
            List<ViewUnassociatedSkidsDto> unassociatedSkids = await _viewUnassociatedSkidsService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return unassociatedSkids;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewUnassociatedStandsDto>> GetUnassociatedStandsData(Guid projectId)
        {
            List<ViewUnassociatedStandsDto> unassociatedStands = await _viewUnassociatedStandsService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return unassociatedStands;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewUnassociatedJunctionBoxesDto>> GetUnassociatedJunctionBoxesData(Guid projectId)
        {
            List<ViewUnassociatedJunctionBoxesDto> unassociatedBoxes = await _viewUnassociatedJunctionBoxesService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return unassociatedBoxes;
        }

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewUnassociatedPanelsDto>> GetUnassociatedPanelsData(Guid projectId)
        {
            List<ViewUnassociatedPanelsDto> unassociatedPanels = await _viewUnassociatedPanelsService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return unassociatedPanels;
        }
        #endregion

        #region NatureOfSignalValidation
        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewNatureOfSignalValidationFailuresDto>> GetNatureOfSignalValidationsData(Guid projectId)
        {
            List<ViewNatureOfSignalValidationFailuresDto> natureOfSignalsData = await _viewNatureOfSignalValidationFailuresService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return natureOfSignalsData;
        }
        #endregion

        #region PSSTags

        [HttpGet]
        [AuthorizePermission()]
        public async Task<List<ViewPSSTagsDto>> GetPSSTagsData(Guid projectId)
        {
            List<ViewPSSTagsDto> pssTagsData = await _viewPSSTagsService.GetAll(a => a.ProjectId == projectId).OrderBy(a => a.TagName).ToListAsync();
            return pssTagsData;
        }
        #endregion
    }
}
