using ICMD.API.Helpers;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.Hierarchy;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class HierarchyController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly IControlSystemHierarchyService _controlSystemHierarchyService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly ISystemService _systemService;
        private readonly ISubSystemService _subSystemService;
        public HierarchyController(IDeviceService deviceService, IControlSystemHierarchyService controlSystemHierarchyService, IWorkAreaPackService workAreaPackService,
            ISystemService systemService, ISubSystemService subSystemService)
        {
            _deviceService = deviceService;
            _controlSystemHierarchyService = controlSystemHierarchyService;
            _workAreaPackService = workAreaPackService;
            _systemService = systemService;
            _subSystemService = subSystemService;
        }

        [HttpPost]
        [AuthorizePermission()]
        public async Task<HierarchyResponceDto> GetHierarchyData(HierarchyRequestDto info)
        {
            HierarchyResponceDto hierarchyData = new HierarchyResponceDto();
            if (ModelState.IsValid)
            {
                bool? optionStatus = (Options.All.ToString() == info.Option) ? null : (Options.Active.ToString() == info.Option ? true : false);
                var tagData = await _deviceService.GetAll(s => !s.IsDeleted && s.Tag.ProjectId == info.ProjectId).Select(s => new
                {
                    Id = s.Id,
                    Name = s.Tag.TagName,
                    IsInstrument = s.IsInstrument
                }).ToListAsync();
                HierachyType sourceEnum;
                if (Enum.TryParse(info.HieararchyType, out sourceEnum))
                {
                    switch (sourceEnum)
                    {
                        case HierachyType.Control:
                            hierarchyData = await GetControlHierarchy(info.ProjectId, optionStatus);
                            break;
                        case HierachyType.CCMD:
                            hierarchyData = await GetCCMDHierarchy(info.ProjectId, optionStatus);
                            break;
                        default:
                            break;
                    }
                    hierarchyData.TagList = sourceEnum != HierachyType.Control ? tagData.Where(x => !x.IsInstrument.Equals(IsInstrumentOption.None)).Select(s => new DropdownInfoDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList() : tagData.Select(s => new DropdownInfoDto
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();
                }

            }
            return hierarchyData;
        }

        private async Task<HierarchyResponceDto> GetControlHierarchy(Guid? projectId, bool? status)
        {
            HierarchyResponceDto info = new HierarchyResponceDto();

            // Fetch data asynchronously
            List<Device> projectDevices = await _deviceService.GetAll(s => !s.IsDeleted && s.Tag.ProjectId == projectId).ToListAsync();
            List<ControlSystemHierarchy> controls = await _controlSystemHierarchyService.GetAll(s => !s.ChildDevice.IsDeleted && !s.IsDeleted).ToListAsync();

            List<Device> devices = projectDevices
                .Where(pd => !controls.Any(cs => cs.ChildDeviceId == pd.Id))
                .ToList();

            List<HierarchyDeviceInfoDto> deviceData = new List<HierarchyDeviceInfoDto>();
            List<HierarchyDeviceInfoDto> notAttachedData = new List<HierarchyDeviceInfoDto>();

            foreach (var item in devices)
            {
                List<ControlSystemHierarchy> childDevices = controls
                    .Where(s => s.ParentDeviceId == item.Id && !s.IsDeleted)
                    .ToList();
                List<ControlSystemHierarchy> parentDevices = controls
                    .Where(s => s.ChildDeviceId == item.Id && !s.IsDeleted)
                    .ToList();

                List<HierarchyDeviceInfoDto> childData = childDevices
                    .Select(s => new HierarchyDeviceInfoDto
                    {
                        Id = s.ChildDevice.Id,
                        Name = s.ChildDevice.Tag.TagName,
                        IsFolder = false,
                        IsActive = s.IsActive,
                        ChildrenList = new List<HierarchyDeviceInfoDto>()
                    })
                    .ToList();

                ProcessHierarchyInfo(controls, deviceData, notAttachedData, item, childDevices, parentDevices, childData, status);
            }

            if (notAttachedData.Any() && !(status != null && !status.Value))
            {
                deviceData.Add(new HierarchyDeviceInfoDto
                {
                    Id = Guid.Empty,
                    Name = "Not Attached",
                    IsFolder = true,
                    IsActive = true,
                    ChildrenList = notAttachedData
                });
            }

            string text = JsonSerializer.Serialize(deviceData);

            if (status != null && !status.Value)
            {
                info.DeviceList = FindRecordsWithInactiveParentsOrChildren(deviceData);
                List<HierarchyDeviceInfoDto> notAttachDeletedData = FindRecordsWithInactiveParentsOrChildren(notAttachedData);
                if (notAttachDeletedData.Any())
                {
                    info.DeviceList.Add(new HierarchyDeviceInfoDto
                    {
                        Id = Guid.Empty,
                        Name = "Not Attached",
                        IsFolder = true,
                        IsActive = true,
                        ChildrenList = notAttachDeletedData
                    });
                }
            }
            else if (status != null && status.Value)
                info.DeviceList = FindRecordsWithActiveParentsOrChildren(deviceData);
            else
                info.DeviceList = deviceData;

            return info;
        }

        private void ProcessHierarchyInfo(
            List<ControlSystemHierarchy> controls,
            List<HierarchyDeviceInfoDto> deviceData,
            List<HierarchyDeviceInfoDto> notAttachedData,
            Device item,
            List<ControlSystemHierarchy> childDevices,
            List<ControlSystemHierarchy> parentDevices,
            List<HierarchyDeviceInfoDto> childData,
            bool? status)
        {
            if (parentDevices.Count == 0 && !childDevices.Any(s => !s.Instrument))
            {
                notAttachedData.Add(new HierarchyDeviceInfoDto
                {
                    Id = item.Id,
                    Name = item.Tag.TagName,
                    Instrument = false,
                    IsFolder = false,
                    IsActive = item.IsActive,
                    ChildrenList = childData
                });

                SetChildDataForControlHierarchy(childData, controls, childDevices, status);
            }
            else
            {
                deviceData.Add(new HierarchyDeviceInfoDto
                {
                    Id = item.Id,
                    Name = item.Tag.TagName,
                    IsFolder = false,
                    Instrument = parentDevices.Any(x => x.Instrument && x.ParentDevice.Id == item.Id),
                    IsActive = item.IsActive,
                    ChildrenList = childData
                });

                // Recursively process child records
                SetChildDataForControlHierarchy(childData, controls, childDevices, status);
            }
        }

        private void SubProcessHierarchyInfo(List<ControlSystemHierarchy> controls, HierarchyDeviceInfoDto childItem, Device? item, List<ControlSystemHierarchy> childDevices, List<ControlSystemHierarchy> parentDevices,
            List<HierarchyDeviceInfoDto> childData, bool? status)
        {
            if (parentDevices.Count == 0 && !childDevices.Any(s => !s.Instrument))
            {
                childItem.ChildrenList.AddRange(childData);
                SetChildDataForControlHierarchy(childData, controls, childDevices, status);
            }
            else
            {
                childItem.ChildrenList.AddRange(childData);

                // Recursively process child records
                SetChildDataForControlHierarchy(childData, controls, childDevices, status);
            }
        }


        private void SetChildDataForControlHierarchy(List<HierarchyDeviceInfoDto> childData, List<ControlSystemHierarchy> controls, List<ControlSystemHierarchy> childDevices, bool? status)
        {
            foreach (var childItem in childData)
            {
                childItem.ChildrenList = childItem.ChildrenList == null ? new List<HierarchyDeviceInfoDto>() : childItem.ChildrenList;
                List<HierarchyDeviceInfoDto> newChildData = controls.Where(s => s.ParentDeviceId == childItem.Id && !s.IsDeleted).ToList()
                .Select(s => new HierarchyDeviceInfoDto
                {
                    Id = s.ChildDevice.Id,
                    Name = s.ChildDevice.Tag.TagName,
                    IsFolder = false,
                    IsActive = s.IsActive,
                    ChildrenList = new List<HierarchyDeviceInfoDto>()
                })
                .ToList();

                SubProcessHierarchyInfo(
                    controls,
                    childItem,
                    childDevices.FirstOrDefault(a => a.ChildDeviceId == childItem.Id)?.ChildDevice,
                    controls.Where(s => s.ParentDeviceId == childItem.Id && !s.IsDeleted).ToList(),
                    controls.Where(s => s.ChildDeviceId == childItem.Id && !s.IsDeleted).ToList(),
                    newChildData,
                    status
                    );
            }
        }

        private async Task<HierarchyResponceDto> GetCCMDHierarchy(Guid? projectId, bool? status)
        {
            HierarchyResponceDto info = new HierarchyResponceDto();
            List<Device> projectDevices = await _deviceService.GetAll(s => !s.IsDeleted && s.Tag.ProjectId == projectId).ToListAsync();
            List<WorkAreaPack> workAreaPacks = await _workAreaPackService.GetAll(s => s.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            List<ICMD.Core.DBModels.System> allSystems = await _systemService.GetAll(s => s.WorkAreaPack.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            List<SubSystem> allSubSystems = await _subSystemService.GetAll(s => s.System.WorkAreaPack.ProjectId == projectId && s.IsActive && !s.IsDeleted).ToListAsync();
            List<HierarchyDeviceInfoDto> deviceData = new List<HierarchyDeviceInfoDto>();
            foreach (var workItem in workAreaPacks)
            {
                List<HierarchyDeviceInfoDto> systems = allSystems.Where(s => s.WorkAreaPackId == workItem.Id).Select(a => new HierarchyDeviceInfoDto
                {
                    Id = a.Id,
                    Name = a.Number,
                    IsFolder = true
                }).ToList();
                foreach (var item in systems)
                {
                    List<HierarchyDeviceInfoDto> subSystemData = allSubSystems.Where(s => s.SystemId == item.Id).Select(a => new HierarchyDeviceInfoDto
                    {
                        Id = a.Id,
                        Name = a.Number,
                        IsFolder = true
                    }).ToList();
                    foreach (var subSystemItem in subSystemData)
                    {
                        List<HierarchyDeviceInfoDto> subSystemDevices = projectDevices.Where(a => !a.IsInstrument.Equals("-") && a.SubSystemId == subSystemItem.Id).Select(a => new HierarchyDeviceInfoDto
                        {
                            Id = a.Id,
                            Name = a.Tag.TagName,
                            Instrument = false,
                            IsFolder = false,
                            IsActive = a.IsActive
                        })
                            .Where(a => status == null || a.IsActive == status)
                            .ToList();
                        subSystemItem.ChildrenList = subSystemDevices;
                    }
                    item.ChildrenList = subSystemData.Where(s => s.ChildrenList != null && s.ChildrenList.Count != 0).ToList();
                }
                deviceData.Add(new HierarchyDeviceInfoDto
                {
                    Id = workItem.Id,
                    Name = workItem.Number,
                    IsFolder = true,
                    ChildrenList = systems.Where(s => s.ChildrenList != null && s.ChildrenList.Count != 0).ToList()
                });
            }
            info.DeviceList = deviceData.Where(s => s.ChildrenList != null && s.ChildrenList.Count != 0).ToList();
            return info;
        }

        private List<HierarchyDeviceInfoDto> FindRecordsWithInactiveParentsOrChildren(List<HierarchyDeviceInfoDto> data)
        {
            var recordsWithInactiveParentsOrChildren = data
                .Where(record => !record.IsActive || HasInactiveNestedChild(record))
                .ToList();

            return recordsWithInactiveParentsOrChildren;
        }

        private bool HasInactiveNestedChild(HierarchyDeviceInfoDto child)
        {
            if (child.ChildrenList == null || !child.ChildrenList.Any())
            {
                return false;
            }

            return child.ChildrenList.Any(nestedChild => !nestedChild.IsActive || HasInactiveNestedChild(nestedChild));
        }


        private List<HierarchyDeviceInfoDto> FindRecordsWithActiveParentsOrChildren(List<HierarchyDeviceInfoDto> data)
        {
            foreach (var item in data)
            {
                RemoveInactiveChildren(item);
            }
            return data;
        }

        private void RemoveInactiveChildren(HierarchyDeviceInfoDto item)
        {
            item.ChildrenList.RemoveAll(child => !child.IsActive);

            foreach (var child in item.ChildrenList)
            {
                RemoveInactiveChildren(child);
            }
        }

    }
}
