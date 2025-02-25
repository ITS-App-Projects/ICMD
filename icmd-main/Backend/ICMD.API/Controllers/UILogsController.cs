using System.Xml;
using System.Xml.Linq;

using ICMD.API.Helpers;
using ICMD.Core.Authorization;
using ICMD.Core.Common;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Interface;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UILogsController : BaseController
    {
        private readonly IUIChangeLogService _uiChangeLogService;
        private readonly ITagService _tagService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly UserManager<ICMDUser> _userManager;
        private readonly IDeviceService _deviceService;

        public UILogsController(IUIChangeLogService uiChangeLogService, ITagService tagService, UserManager<ICMDUser> userManager, IDeviceTypeService deviceTypeService,
            IDeviceService deviceService)
        {
            _uiChangeLogService = uiChangeLogService;
            _tagService = tagService;
            _userManager = userManager;
            _deviceTypeService = deviceTypeService;
            _deviceService = deviceService;
        }

        #region UIChangeLogs
        [HttpPost]
        [AuthorizePermission()]
        public async Task<PagedResultDto<ChangeLogResponceDto>> GetTypeWiseChangeLogs(UIChangeLogRequestDto info)
        {
            List<string> projectTags = await _tagService
                .GetAll(t => t.ProjectId == info.ProjectId)
                .Select(t => t.TagName)
                .ToListAsync();

            List<string> manualTypes = ["Bulk Delete", "Import"];

            List<UIChangeLogDetailsDto> changeLogItems = manualTypes.Contains(info.Type) ?
                await (from uc in _uiChangeLogService.GetAll(c => c.Type == info.Type)
                       join um in _userManager.Users on uc.CreatedBy equals um.Id
                       select new UIChangeLogDetailsDto
                        {
                            Id = uc.Id,
                            Tag = uc.Tag ?? "", // Module
                            PLCNumber = uc.PLCNumber,
                            Changes = uc.Changes,
                            Type = uc.Type,
                            UserName = um.FullName,
                            CreatedBy = uc.CreatedBy,
                            CreatedDate = uc.CreatedDate
                        }).OrderByDescending(a => a.CreatedDate).ToListAsync() :
                await (from uc in _uiChangeLogService.GetAll(c => projectTags.Contains(c.Tag))
                       join um in _userManager.Users on uc.CreatedBy equals um.Id
                       select new UIChangeLogDetailsDto
                       {
                           Id = uc.Id,
                           Tag = uc.Tag ?? "",
                           PLCNumber = uc.PLCNumber,
                           Changes = uc.Changes,
                           Type = uc.Type,
                           UserName = um.FullName,
                           CreatedBy = uc.CreatedBy,
                           CreatedDate = uc.CreatedDate
                       }).OrderByDescending(a => a.CreatedDate).ToListAsync();

            if (!string.IsNullOrEmpty(info.Type))
                changeLogItems = changeLogItems.Where(a => a.Type == info.Type).ToList();

            if (!string.IsNullOrEmpty(info.Tag))
                changeLogItems = changeLogItems.Where(i => i.Tag.Contains(info.Tag.Trim())).ToList();

            if (!string.IsNullOrEmpty(info.PLCNo))
                changeLogItems = changeLogItems.Where(i => !string.IsNullOrEmpty(i.PLCNumber) && i.PLCNumber.Contains(info.PLCNo.Trim())).ToList();

            if (!string.IsNullOrEmpty(info.UserName))
                changeLogItems = changeLogItems.Where(i => i.UserName.Contains(info.UserName.Trim())).ToList();

            if (!string.IsNullOrEmpty(info.StartDate))
            {
                DateTime startDate = Convert.ToDateTime(info.StartDate);
                changeLogItems = changeLogItems.Where(i => i.CreatedDate.Date >= startDate.Date).ToList();
            }

            if (!string.IsNullOrEmpty(info.EndDate))
            {
                DateTime endDate = Convert.ToDateTime(info.EndDate);
                changeLogItems = changeLogItems.Where(i => i.CreatedDate.Date <= endDate.Date).ToList();
            }

            List<ChangeLogItemDto> logs = ReadChangeLog(changeLogItems);
            List<ChangeLogResponceDto> typeLogsData = logs.ToLookup(a => a.Tag).Select(a => new ChangeLogResponceDto
            {
                Key = a.Key,
                Items = a.ToList()
            }).ToList();

            var paginatedData = typeLogsData.Skip((info.PageNumber - 1) * info.PageSize).Take(info.PageSize);
            return new PagedResultDto<ChangeLogResponceDto>(typeLogsData.Count, paginatedData.ToList());
        }

        [HttpGet]
        public async Task<UIChangeLogTypeDropdownInfoDto> GetChangeLogTypes(Guid projectId)
        {
            UIChangeLogTypeDropdownInfoDto changeLogInfo = new UIChangeLogTypeDropdownInfoDto();
            List<Tag> projectTags = await _tagService.GetAll(t => t.ProjectId == projectId && t.IsActive && !t.IsDeleted).ToListAsync();
            List<string> tagName = projectTags.Select(a => a.TagName).ToList();
            List<UIChangeLog> changeLogItems = await _uiChangeLogService.GetAll(c => tagName.Contains(c.Tag)).OrderByDescending(a => a.CreatedDate).ToListAsync();

            //Types List
            changeLogInfo.Types = changeLogItems.Select(a => a.Type).Distinct().ToList();
            changeLogInfo.Types.Add("Bulk Delete");
            changeLogInfo.Types.Add("Import");

            //TagList
            changeLogInfo.TagList = projectTags.Select(a => new DropdownInfoDto
            {
                Id = a.Id,
                Name = a.TagName
            }).ToList();

            //UserList
            var userData = await (from uc in _uiChangeLogService.GetAll(c => tagName.Contains(c.Tag))
                                  join um in _userManager.Users on uc.CreatedBy equals um.Id
                                  select new DropdownInfoDto
                                  {
                                      Id = um.Id,
                                      Name = um.FullName
                                  }).ToListAsync();
            changeLogInfo.UserList = userData.DistinctBy(a => a.Name).ToList();

            //PLCList
            DeviceType deviceTypeInfo = await _deviceTypeService.GetSingleAsync(a => a.Type == "PLC" && a.IsActive && !a.IsDeleted);
            changeLogInfo.PLCList = deviceTypeInfo != null ? await _deviceService.GetAll(a => a.Tag.ProjectId == projectId && a.DeviceTypeId == deviceTypeInfo.Id && !a.IsDeleted).Select(a => new DropdownInfoDto
            {
                Id = a.TagId,
                Name = a.Tag.TagName
            }).ToListAsync() : new List<DropdownInfoDto>();

            return changeLogInfo;
        }

        private List<ChangeLogItemDto> ReadChangeLog(List<UIChangeLogDetailsDto> changeLogs)
        {
            List<ChangeLogItemDto> changeLogItems = new List<ChangeLogItemDto>();
            foreach (var item in changeLogs)
            {
                var reader = XmlReader.Create(new StringReader(item.Changes));
                var root = XElement.Load(reader);

                ChangeLogItemDto changeLog = new ChangeLogItemDto()
                {
                    Tag = item.Tag,
                    Date = item.CreatedDate,
                    Type = root.Element("Type")?.Value ?? "",
                    UserName = item.UserName
                };

                var properties = root.Element("Properties");

                if (properties != null)
                {
                    foreach (var property in properties.Elements())
                    {
                        var changeLogProperty = new PropertyChangeLogDto
                        {
                            Name = property.Element("Name")?.Value ?? "",
                            OldValue = property.Element("OriginalValue")?.Value ?? "",
                            NewValue = property.Element("NewValue")?.Value ?? ""
                        };

                        changeLog.Properties.Add(changeLogProperty);
                    }
                }

                var attributes = root.Element("Attributes");

                if (attributes != null)
                {
                    foreach (var attribute in attributes.Elements())
                    {
                        var changeLogAttribute = new PropertyChangeLogDto
                        {
                            Name = attribute.Element("Name")?.Value ?? "",
                            OldValue = attribute.Element("OriginalValue")?.Value ?? "",
                            NewValue = attribute.Element("NewValue")?.Value ?? ""
                        };

                        changeLog.Attributes.Add(changeLogAttribute);
                    }
                }

                var documents = root.Element("ReferenceDocuments");

                if (documents != null)
                {
                    foreach (var document in documents.Elements())
                    {
                        var changeLogDocument = new ReferenceDocumentChangeLogDto()
                        {
                            Type = document.Element("Type")?.Value ?? "",
                            DocumentNo = document.Element("DocumentNumber")?.Value ?? "",
                            Revision = document.Element("Revision")?.Value ?? "",
                            Version = document.Element("Version")?.Value ?? "",
                            Sheet = document.Element("Sheet")?.Value ?? "",
                            Status = document.Element("Status")?.Value ?? ""
                        };

                        changeLog.ReferenceDocuments.Add(changeLogDocument);
                    }
                }

                var records = root.Element("Records");

                if (records != null)
                {
                    var type = root.Element("Type")?.Value;

                    if (type == "Import")
                    {
                        foreach (var record in records.Elements())
                        {
                            var importLog = new ImportLogDto()
                            {
                                Name = record.Element("Name")?.Value ?? "",
                                Status = record.Element("Status")?.Value ?? "",
                                Message = record.Element("Message")?.Value ?? "",
                                Operation = record.Element("Operation")?.Value ?? ""
                            };

                            foreach(var change in record.Elements("Items"))
                            {
                                importLog.Items.Add(new Core.Dtos.ImportValidation.ChangesDto
                                {
                                    ItemColumnName = change.Element("ItemColumnName")?.Value ?? string.Empty,
                                    PreviousValue = change.Element("PreviousValue")?.Value ?? string.Empty,
                                    NewValue = change.Element("NewValue")?.Value ?? string.Empty
                                });
                            }

                            changeLog.ImportRecords.Add(importLog);
                        }
                    }
                    else
                    {

                        foreach (var record in records.Elements())
                        {
                            var bulkDeleteLog = new BulkDeleteLogDto()
                            {
                                Name = record.Element("Name")?.Value ?? "",
                                Status = Convert.ToBoolean(record.Element("Status")?.Value ?? ""),
                                Message = record.Element("Message")?.Value ?? ""
                            };

                            changeLog.BulkDeleteRecords.Add(bulkDeleteLog);
                        }
                    }
                }

                var activated = root.Element("Activated");

                if (activated != null)
                {
                    var isActive = bool.Parse(activated.Value);
                    var changeLogAttribute = new PropertyChangeLogDto
                    {
                        Name = (isActive) ? "Activated" : "Deactivated"
                    };
                    changeLog.Statuses.Add(changeLogAttribute);

                    //changeLog.Status = (isActive) ? "Activated" : "Deactivated";
                }

                var deleted = root.Element("Deleted");

                if (deleted != null)
                {
                    var changeLogAttribute = new PropertyChangeLogDto
                    {
                        Name = "Deleted"
                    };
                    changeLog.Statuses.Add(changeLogAttribute);
                }

                changeLogItems.Add(changeLog);

            }
            return changeLogItems;
        }
        #endregion
    }
}
