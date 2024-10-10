using ICMD.API.Helpers;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.Shared.Interface;
using ICMD.Core.ViewDto;
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
    public class NonInstrumentController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly ViewNonInstrumentListService _viewNonInstrumentListService;
        public NonInstrumentController(IDeviceService deviceService, ViewNonInstrumentListService viewNonInstrumentListService)
        {
            _deviceService = deviceService;
            _viewNonInstrumentListService = viewNonInstrumentListService;
        }

        #region NonInstruments
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ViewNonInstrumentListDto>> GetAllNonInstruments(PagedAndSortedResultRequestDto input)
        {
            try
            {
                IQueryable<ViewNonInstrumentListDto> allNonInstruments = _viewNonInstrumentListService.GetAll(x => x.ProjectId == input.ProjectId && x.IsDeleted != true);

                if (!string.IsNullOrEmpty(input.Search))
                {
                    allNonInstruments = allNonInstruments.Where(s => (!string.IsNullOrEmpty(s.ProcessNo) && s.ProcessNo.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.StreamName) && s.StreamName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentCode) && s.EquipmentCode.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SequenceNumber) && s.SequenceNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.EquipmentIdentifier) && s.EquipmentIdentifier.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.TagName) && s.TagName.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DeviceType) && s.DeviceType.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ServiceDescription) && s.ServiceDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NatureOfSignal) && s.NatureOfSignal.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.DPNodeAddress) && s.DPNodeAddress.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.NoOfSlotsChannels) && s.NoOfSlotsChannels.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SlotNumber) && s.SlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ConnectionParent) && s.ConnectionParent.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCNumber) && s.PLCNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.PLCSlotNumber) && s.PLCSlotNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (s.Revision != null && s.Revision.ToString().ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Location) && s.Location.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.Manufacturer) && s.Manufacturer.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelDescription) && s.ModelDescription.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ModelNumber) && s.ModelNumber.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitectureDrawing) && s.ArchitectureDrawing.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.ArchitectureDrawingSheet) && s.ArchitectureDrawingSheet.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.RevisionChanges) && s.RevisionChanges.ToLower().Contains(input.Search.ToLower())) ||
                    (!string.IsNullOrEmpty(s.SubProcess) && s.SubProcess.ToLower().Contains(input.Search.ToLower())));
                }

                if (input.CustomSearchs != null && input.CustomSearchs.Count != 0 && !string.IsNullOrEmpty(input.SearchFieldQuery))
                {
                    allNonInstruments = allNonInstruments.Where(input.SearchFieldQuery);
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
                                allNonInstruments = allNonInstruments.Where(x => x.IsActive);
                            }
                            else if (value == (int)RecordType.Inactive)
                            {
                                allNonInstruments = allNonInstruments.Where(x => !x.IsActive);
                            }
                        }
                    }
                }

                if (input.CustomColumnSearch != null && input.CustomColumnSearch.Count != 0 && !string.IsNullOrEmpty(input.SearchColumnFilterQuery))
                    allNonInstruments = allNonInstruments.Where(input.SearchColumnFilterQuery);

                allNonInstruments = allNonInstruments.OrderBy(@$"{(string.IsNullOrEmpty(input.Sorting) ? "deviceId" : input.Sorting)} {(input.SortAcending ? "asc" : "desc")}");
                bool isExport = input.CustomSearchs != null && input.CustomSearchs.Any(s => s.FieldName == "isExport") ? Convert.ToBoolean(input.CustomSearchs.FirstOrDefault(s => s.FieldName == "isExport")?.FieldValue) : false;
                IQueryable<ViewNonInstrumentListDto> paginatedData = !isExport ? allNonInstruments.Skip((input.PageNumber - 1) * input.PageSize).Take(input.PageSize) : allNonInstruments;

                return new PagedResultDto<ViewNonInstrumentListDto>(
                   allNonInstruments.Count(),
                    await paginatedData.ToListAsync()
               );
            }
            catch (Exception ex)
            {
                return new PagedResultDto<ViewNonInstrumentListDto>(
                   0,
                    new List<ViewNonInstrumentListDto>()
               );
            }

        }
        #endregion
    }
}
