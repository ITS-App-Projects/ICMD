using AutoMapper;
using ICMD.API.Helpers;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.Dtos.Instrument;
using ICMD.Core.Shared.Interface;
using ICMD.Repository.ViewService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class InstrumentController : BaseController
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IDeviceService _deviceService;
        private readonly IControlSystemHierarchyService _controlSystemHierarchyService;
        private readonly IMapper _mapper;
        private readonly ViewDeviceInstrumentService _viewDeviceInstrumentService;
        private readonly ViewInstrumentListLiveService _viewInstrumentListLiveService;
        private static string ModuleName = "Device";

        public InstrumentController(IMapper mapper, IInstrumentService instrumentService, ViewDeviceInstrumentService viewDeviceInstrumentService, ViewInstrumentListLiveService viewInstrumentListLiveService,
            IDeviceService deviceService, IControlSystemHierarchyService controlSystemHierarchyService)
        {
            _mapper = mapper;
            _instrumentService = instrumentService;
            _viewDeviceInstrumentService = viewDeviceInstrumentService;
            _viewInstrumentListLiveService = viewInstrumentListLiveService;
            _deviceService = deviceService;
            _controlSystemHierarchyService = controlSystemHierarchyService;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ViewInstrumentListLiveDto>> GetAllInstruments(PagedAndSortedResultRequestDto input)
        {
            try
            {
                IQueryable<ViewInstrumentListLiveDto> allInstruments = _viewInstrumentListLiveService.GetAll(x => x.ProjectId == input.ProjectId && x.IsDeleted != true);

                if (!string.IsNullOrEmpty(input.Search))
                {
                    allInstruments = allInstruments.Where(s => (!string.IsNullOrEmpty(s.ProcessNo) && s.ProcessNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StreamName) && s.StreamName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentCode) && s.EquipmentCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentIdentifier) && s.EquipmentIdentifier.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TagName) && s.TagName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.InstrumentParentTag) && s.InstrumentParentTag.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ServiceDescription) && s.ServiceDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.LineVesselNumber) && s.LineVesselNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Plant != null && s.Plant.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (s.Area != null && s.Area.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (s.VendorSupply != null && s.VendorSupply.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SkidNumber) && s.SkidNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StandNumber) && s.StandNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Manufacturer) && s.Manufacturer.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelNumber) && s.ModelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CalibratedRangeMin) && s.CalibratedRangeMin.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CalibratedRangeMax) && s.CalibratedRangeMax.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ProcessRangeMin) && s.ProcessRangeMin.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ProcessRangeMax) && s.ProcessRangeMax.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.CRUnits) && s.CRUnits.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PRUnits) && s.PRUnits.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RLPosition) && s.RLPosition.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DatasheetNumber) && s.DatasheetNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SheetNumber) && s.SheetNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.HookUpDrawing) && s.HookUpDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TerminationDiagram) && s.TerminationDiagram.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PIDNumber) && s.PIDNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.LayoutDrawing) && s.LayoutDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitecturalDrawing) && s.ArchitecturalDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.JunctionBoxNumber) && s.JunctionBoxNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NatureOfSignal) && s.NatureOfSignal.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.FailState) && s.FailState.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.GSDType) && s.GSDType.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ControlPanelNumber) && s.ControlPanelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCNumber) && s.PLCNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCSlotNumber) && s.PLCSlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SlotNo) && s.SlotNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ChannelNo) && s.ChannelNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPNodeAddress) && s.DPNodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PANodeAddress) && s.PANodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.FieldPanelNumber) && s.FieldPanelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPDPCoupler) && s.DPDPCoupler.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPPACoupler) && s.DPPACoupler.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.AFDHubNumber) && s.AFDHubNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RackNo) && s.RackNo.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Revision != null && s.Revision.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RevisionChangesOutstandingComments) && s.RevisionChangesOutstandingComments.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Zone) && s.Zone.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Bank) && s.Bank.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Service) && s.Service.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Variable) && s.Variable.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Train) && s.Train.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.WorkAreaPack) && s.WorkAreaPack.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SystemCode) && s.SystemCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubsystemCode) && s.SubsystemCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubProcess) && s.SubProcess.ToLower().Contains(input.Search.ToLower())));

                }

                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0 && !string.IsNullOrEmpty(input.SearchFieldQuery))
                {
                    allInstruments = allInstruments.Where(input.SearchFieldQuery);
                }
                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0)
                {
                    foreach (var item in input.CustomSearchs)
                    {
                        if (item.FieldName.ToLower() == "type".ToLower() && !string.IsNullOrEmpty(item.FieldValue))
                        {
                            int value = Convert.ToInt16(item.FieldValue);
                            if (value == (int)RecordType.Active)
                            {
                                allInstruments = allInstruments.Where(x => x.IsActive);
                            }
                            else if (value == (int)RecordType.Inactive)
                            {
                                allInstruments = allInstruments.Where(x => !x.IsActive);
                            }
                        }
                    }
                }

                if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                    allInstruments = allInstruments.Where(input.SearchColumnFilterQuery);

                allInstruments = allInstruments.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "deviceId" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
                bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
                IQueryable<ViewInstrumentListLiveDto> paginatedData = !isExport ? allInstruments.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allInstruments;

                return new PagedResultDto<ViewInstrumentListLiveDto>(
                   allInstruments.Count(),
                    await paginatedData.ToListAsync()
               );
            }
            catch (Exception ex)
            {
                return new PagedResultDto<ViewInstrumentListLiveDto>(
                   0,
                    new List<ViewInstrumentListLiveDto>()
               );
            }

        }
    }
}
