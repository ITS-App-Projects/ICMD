using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Shared.Extension;
using ICMD.Core.Shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Net;

namespace ICMD.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImportController : BaseController
    {
        private readonly IOMItemService _omItemService;
        private readonly CSVImport _csvImport;
        private readonly IOMDescriptionImportService _omDescriptionImportService;
        private readonly StoredProcedureHelper _storedProcedureHelper;
        private readonly ISSISEquipmentListService _ssisEquipmentService;
        private readonly ISSISInstrumentsService _ssisInstrumentService;
        private readonly ISSISValveListService _ssisValueService;
        private readonly IWorkAreaPackService _workAreaPackService;
        private readonly ISystemService _systemService;
        private readonly ISubSystemService _subSystemService;
        private readonly IDeviceService _deviceService;

        public ImportController(CSVImport csvImport, IOMItemService omItemService, IOMDescriptionImportService omDescriptionImportService,
            StoredProcedureHelper storedProcedureHelper, ISSISEquipmentListService ssisEquipmentService, ISSISInstrumentsService ssisInstrumentService,
            ISSISValveListService ssisValueService, IWorkAreaPackService workAreaPackService, ISystemService systemService,
            ISubSystemService subSystemService, IDeviceService deviceService)
        {
            _csvImport = csvImport;
            _omItemService = omItemService;
            _omDescriptionImportService = omDescriptionImportService;
            _storedProcedureHelper = storedProcedureHelper;
            _ssisEquipmentService = ssisEquipmentService;
            _ssisInstrumentService = ssisInstrumentService;
            _ssisValueService = ssisValueService;
            _workAreaPackService = workAreaPackService;
            _systemService = systemService;
            _subSystemService = subSystemService;
            _deviceService = deviceService;
        }

        #region OMImport
        [HttpPost]
        [AuthorizePermission()]
        public async Task<BaseResponse> UploadOMItems([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                FileType fileType;
                var typeHeaders = _csvImport.ReadFile(info.File, out fileType);
                try
                {
                    if (fileType == FileType.OMItems)
                    {
                        await ImportOMItems(typeHeaders, info.ProjectId);
                    }
                    else if (fileType == FileType.OMServiceDescriptions)
                    {
                        await ImportOMServiceDescriptions(typeHeaders, info.ProjectId);
                    }
                    else
                    {
                        return new BaseResponse(false, ResponseMessages.FailedOMImport, HttpStatusCode.BadRequest);
                    }
                    return new BaseResponse(true, ResponseMessages.ImportFile, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return new BaseResponse(false, ex.Message, HttpStatusCode.OK);
                }
            }
            return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<bool> ImportOMItems(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            try
            {
                List<OMItem> removeOMItems = await _omItemService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId).ToListAsync();
                if (removeOMItems.Any())
                {
                    foreach (var item in removeOMItems)
                    {
                        item.IsDeleted = true;
                        _omItemService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                var requiredKeys = new List<string> { "ItemId", "ItemDesc", "ParentItemId", "AssetTypeId" };
                foreach (var dictionary in headerItems)
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains))
                    {
                        var omInfo = new OMItem()
                        {
                            ItemId = dictionary["ItemId"],
                            ItemDescription = dictionary["ItemDesc"],
                            ParentItemId = dictionary["ParentItemId"],
                            AssetTypeId = dictionary["AssetTypeId"],
                            ProjectId = projectId
                        };
                        await _omItemService.AddAsync(omInfo, User.GetUserId());
                    }

                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        private async Task<bool> ImportOMServiceDescriptions(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            try
            {
                Guid userId = User.GetUserId();
                var requiredKeys = new List<string> {"Tag", "Service Description", "Area","Stream", "Bank", "Service", "Variable",
                                                                                        "Train" };
                foreach (var dictionary in headerItems)
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains))
                    {
                        var serviceDescription = new OMServiceDescriptionImport()
                        {
                            Tag = dictionary["Tag"] ?? "",
                            ServiceDescription = dictionary["Service Description"] ?? "",
                            Area = dictionary["Area"] ?? null,
                            Stream = dictionary["Stream"] ?? null,
                            Bank = dictionary["Bank"] ?? null,
                            Service = dictionary["Service"] ?? null,
                            Variable = dictionary["Variable"] ?? null,
                            Train = dictionary["Train"] ?? null,
                            ProjectId = projectId
                        };

                        await _omDescriptionImportService.AddAsync(serviceDescription, userId);
                    }


                }

                //call function
                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("_CreatedBy", NpgsqlDbType.Uuid) { Value = userId },
                };
                await _storedProcedureHelper.ImportOMServiceDescriptions(parameters);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region PnIdImport
        [AuthorizePermission()]
        [HttpPost]
        public async Task<BaseResponse> UploadPnIDs([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                FileType fileType;
                var typeHeaders = _csvImport.ReadFile(info.File, out fileType);
                if (fileType == FileType.EquipmentList)
                {
                    await ImportEquipmentList(typeHeaders, info.ProjectId);
                }
                else if (fileType == FileType.InstrumentList)
                {
                    await ImportInstrumentList(typeHeaders, info.ProjectId);
                }
                else if (fileType == FileType.ValveList)
                {
                    await ImportValveList(typeHeaders, info.ProjectId);
                }
                else
                {
                    return new BaseResponse(false, ResponseMessages.FailedPnIdImport, HttpStatusCode.BadRequest);
                }
                var parameters = new NpgsqlParameter[]
                 {
                    new NpgsqlParameter("_ProjectId", NpgsqlDbType.Uuid) { Value = info.ProjectId },
                    new NpgsqlParameter("_CreatedBy", NpgsqlDbType.Uuid) { Value = User.GetUserId() },
                 };
                await _storedProcedureHelper.ImportPNIDTags(parameters);
                return new BaseResponse(true, ResponseMessages.ImportFile, HttpStatusCode.OK);
            }
            return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<bool> ImportEquipmentList(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            try
            {
                List<SSISEquipmentList> removeItems = await _ssisEquipmentService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId).ToListAsync();
                if (removeItems.Any())
                {
                    foreach (var item in removeItems)
                    {
                        item.IsDeleted = true;
                        _ssisEquipmentService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                var requiredKeys = new List<string> { "PnPId", "Process Number", "Sub Process", "Stream",
                                                                               "Equipment Code", "Sequence Number", "Equipment Identifier",
                                                                               "Tag", "DWG Title", "REV", "VERSION", "Description",
                                                                               "Piping Class", "On Skid", "Function",   "Tracking Number" };
                foreach (var dictionary in headerItems)
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains))
                    {
                        var info = new SSISEquipmentList()
                        {
                            PnPId = dictionary["PnPId"],
                            ProcessNumber = dictionary["Process Number"],
                            SubProcess = dictionary["Sub Process"],
                            Stream = dictionary["Stream"],
                            EquipmentCode = dictionary["Equipment Code"],
                            SequenceNumber = dictionary["Sequence Number"],
                            EquipmentIdentifier = dictionary["Equipment Identifier"],
                            Tag = dictionary["Tag"],
                            DWGTitle = dictionary["DWG Title"],
                            Rev = dictionary["REV"],
                            Version = dictionary["VERSION"],
                            Description = dictionary["Description"],
                            PipingClass = dictionary["Piping Class"],
                            OnSkid = dictionary["On Skid"],
                            Function = dictionary["Function"],
                            TrackingNumber = dictionary["Tracking Number"],
                            ProjectId = projectId
                        };
                        await _ssisEquipmentService.AddAsync(info, User.GetUserId());
                    }

                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        private async Task<bool> ImportInstrumentList(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            try
            {
                List<SSISInstruments> removeItems = await _ssisInstrumentService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId).ToListAsync();
                if (removeItems.Any())
                {
                    foreach (var item in removeItems)
                    {
                        item.IsDeleted = true;
                        _ssisInstrumentService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                var requiredKeys = new List<string> { "PnPId", "Process Number", "Sub Process", "Stream",
                                                                                 "Equipment Code", "Sequence Number", "Equipment Identifier",
                                                                                 "Tag", "On Equipment", "On Skid", "Description", "FluidCode",
                                                                                 "PipeLines.Tag", "Size", "DWG Title", "REV", "VERSION", "To",
                                                                                 "From", "Tracking Number"  };
                foreach (var dictionary in headerItems)
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains))
                    {
                        var info = new SSISInstruments()
                        {
                            PnPId = dictionary["PnPId"],
                            ProcessNumber = dictionary["Process Number"],
                            SubProcess = dictionary["Sub Process"],
                            Stream = dictionary["Stream"],
                            EquipmentCode = dictionary["Equipment Code"],
                            SequenceNumber = dictionary["Sequence Number"],
                            EquipmentIdentifier = dictionary["Equipment Identifier"],
                            Tag = dictionary["Tag"],
                            OnEquipment = dictionary["On Equipment"],
                            OnSkid = dictionary["On Skid"],
                            Description = dictionary["Description"],
                            FluidCode = dictionary["FluidCode"],
                            PipeLinesTag = dictionary["PipeLines.Tag"],
                            Size = dictionary["Size"],
                            DWGTitle = dictionary["DWG Title"],
                            Rev = dictionary["REV"],
                            Version = dictionary["VERSION"],
                            To = dictionary["To"],
                            From = dictionary["From"],
                            TrackingNumber = dictionary["Tracking Number"],
                            ProjectId = projectId
                        };
                        await _ssisInstrumentService.AddAsync(info, User.GetUserId());
                    }

                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        private async Task<bool> ImportValveList(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            try
            {
                List<SSISValveList> removeItems = await _ssisValueService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId).ToListAsync();
                if (removeItems.Any())
                {
                    foreach (var item in removeItems)
                    {
                        item.IsDeleted = true;
                        _ssisValueService.Update(item, item, User.GetUserId(), true, true);
                    }
                }

                var requiredKeys = new List<string> { "PnPId", "Process Number", "Sub Process", "Stream",
                                                                            "Equipment Code", "Sequence Number", "Equipment Identifier", "Tag",
                                                                            "DWG Title", "REV", "VERSION", "Description", "Size", "FluidCode",
                                                                            "PipeLines.Tag", "Piping Class", "Class Name", "On Skid", "Failure",
                                                                            "Switches", "From", "To", "Accessories", "Design Temp", "Nominal Pressure",
                                                                            "Valve Spec Number", "PN Rating", "Tracking Number"  };
                foreach (var dictionary in headerItems)
                {
                    var keys = dictionary.Keys.ToList();
                    if (requiredKeys.All(keys.Contains))
                    {
                        var info = new SSISValveList()
                        {
                            PnPId = dictionary["PnPId"],
                            ProcessNumber = dictionary["Process Number"],
                            SubProcess = dictionary["Sub Process"],
                            Stream = dictionary["Stream"],
                            EquipmentCode = dictionary["Equipment Code"],
                            SequenceNumber = dictionary["Sequence Number"],
                            EquipmentIdentifier = dictionary["Equipment Identifier"],
                            Tag = dictionary["Tag"],
                            DWGTitle = dictionary["DWG Title"],
                            Rev = dictionary["REV"],
                            Version = dictionary["VERSION"],
                            Description = dictionary["Description"],
                            Size = dictionary["Size"],
                            FluidCode = dictionary["FluidCode"],
                            PipeLinesTag = dictionary["PipeLines.Tag"],
                            PipingClass = dictionary["Piping Class"],
                            ClassName = dictionary["Class Name"],
                            OnSkid = dictionary["On Skid"],
                            Failure = dictionary["Failure"],
                            Switches = dictionary["Switches"],
                            From = dictionary["From"],
                            To = dictionary["To"],
                            Accessories = dictionary["Accessories"],
                            DesignTemp = dictionary["Design Temp"],
                            NominalPressure = dictionary["Nominal Pressure"],
                            ValveSpecNumber = dictionary["Valve Spec Number"],
                            PNRating = dictionary["PN Rating"],
                            TrackingNumber = dictionary["Tracking Number"],
                            ProjectId = projectId
                        };
                        await _ssisValueService.AddAsync(info, User.GetUserId());
                    }

                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion

        #region CCMDImport
        [AuthorizePermission()]
        [HttpPost]
        public async Task<BaseResponse> UploadCCMD([FromForm] FileUploadModel info)
        {
            if (info.File != null && info.File.Length > 0)
            {
                FileType fileType;
                var typeHeaders = _csvImport.ReadFile(info.File, out fileType);
                if (fileType == FileType.CCMD)
                {
                    try
                    {
                        List<string> missinTags = await ImportCCMD(typeHeaders, info.ProjectId);
                        if (missinTags.Any())
                        {
                            return new BaseResponse(false, ResponseMessages.MissingTagImport.ToString().Replace("{{missingTags}}",
                                string.Join(",", missinTags)), HttpStatusCode.BadRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new BaseResponse(false, ex.Message, HttpStatusCode.BadRequest);
                    }

                }
                else
                {
                    return new BaseResponse(false, ResponseMessages.FailedOMImport, HttpStatusCode.BadRequest);
                }
                return new BaseResponse(true, ResponseMessages.ImportFile, HttpStatusCode.OK);
            }
            return new BaseResponse(false, ResponseMessages.GlobalModelValidationMessage, HttpStatusCode.BadRequest);
        }

        private async Task<List<string>> ImportCCMD(List<Dictionary<string, string>> headerItems, Guid projectId)
        {
            List<string> workAreas = headerItems.Where(a => a.Keys.Contains("WAP")).Select(d => d["WAP"]).Distinct().ToList();
            Guid userId = User.GetUserId();
            foreach (var item in workAreas.Where(a => !string.IsNullOrEmpty(a?.Trim())).ToList())
            {
                var chkExist = await _workAreaPackService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId && a.Number == item).FirstOrDefaultAsync();
                if (chkExist == null)
                {
                    WorkAreaPack workInfo = new WorkAreaPack()
                    {
                        Number = item,
                        Description = "",
                        ProjectId = projectId
                    };
                    await _workAreaPackService.AddAsync(workInfo, userId);
                }
            }

            ///System
            var systemRequiredKeys = new List<string> { "WAP", "SystemCode" };
            var systemDictionaries = headerItems.Where(a => systemRequiredKeys.All(a.Keys.Contains)).GroupBy(d => new { WAP = d["WAP"], SystemCode = d["SystemCode"] })
                                                       .Select(g => g.FirstOrDefault());
            foreach (var dictionary in systemDictionaries)
            {
                if (dictionary != null)
                {
                    var keys = dictionary != null ? dictionary.Keys.ToList() : null;
                    if (keys != null && systemRequiredKeys.All(keys.Contains))
                    {
                        string wapNo = dictionary != null ? dictionary["WAP"] : "";
                        string systemCode = dictionary != null ? dictionary["SystemCode"] : "";
                        var workAreaChkExist = await _workAreaPackService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId && a.Number == wapNo).FirstOrDefaultAsync();

                        if (workAreaChkExist != null)
                        {
                            var systemChkExist = await _systemService.GetAll(a => a.IsActive && !a.IsDeleted && a.Number == systemCode &&
                            a.WorkAreaPackId == workAreaChkExist.Id).FirstOrDefaultAsync();

                            if (systemChkExist == null)
                            {
                                Core.DBModels.System systemInfo = new Core.DBModels.System()
                                {
                                    Number = systemCode,
                                    Description = "",
                                    WorkAreaPackId = workAreaChkExist.Id,
                                };
                                await _systemService.AddAsync(systemInfo);
                            }
                        }

                    }
                }
            }

            //SubSystem
            var subSystemRequiredKeys = new List<string> { "WAP", "SystemCode", "SubsystemCode" };
            var subSystemGroups = headerItems.Where(a => subSystemRequiredKeys.All(a.Keys.Contains)).GroupBy(d => new { WAP = d["WAP"], SystemCode = d["SystemCode"], SubSystemCode = d["SubsystemCode"] })
                                                    .Select(g => g.FirstOrDefault());

            foreach (var dictionary in subSystemGroups)
            {
                if (dictionary != null)
                {
                    var keys = dictionary != null ? dictionary.Keys.ToList() : null;
                    if (keys != null && subSystemRequiredKeys.All(keys.Contains))
                    {
                        string wapNo = dictionary != null ? dictionary["WAP"] : "";
                        string systemCode = dictionary != null ? dictionary["SystemCode"] : "";
                        string subSystemCode = dictionary != null ? dictionary["SubsystemCode"] : "";

                        var workAreaChkExist = await _workAreaPackService.GetAll(a => a.IsActive && !a.IsDeleted && a.ProjectId == projectId && a.Number == wapNo).FirstOrDefaultAsync();

                        if (workAreaChkExist != null)
                        {
                            var systemChkExist = await _systemService.GetAll(a => a.IsActive && !a.IsDeleted && a.Number == systemCode &&
                            a.WorkAreaPackId == workAreaChkExist.Id).FirstOrDefaultAsync();

                            if (systemChkExist != null)
                            {
                                var subSystemChkExist = await _subSystemService.GetAll(a => a.IsActive && !a.IsDeleted && a.Number == subSystemCode &&
                            a.SystemId == systemChkExist.Id).FirstOrDefaultAsync();

                                if (subSystemChkExist == null)
                                {
                                    SubSystem subSystemInfo = new SubSystem()
                                    {
                                        Number = subSystemCode,
                                        Description = "",
                                        SystemId = systemChkExist.Id
                                    };
                                    await _subSystemService.AddAsync(subSystemInfo);
                                }
                            }
                        }
                    }
                }
            }

            //Missing Tag
            List<string> missingTags = new List<string>();
            var headerRequiredKeys = new List<string> { "TagNo", "WAP", "SystemCode", "SubsystemCode" };
            foreach (var dictionary in headerItems)
            {
                if (dictionary != null)
                {
                    var keys = dictionary != null ? dictionary.Keys.ToList() : null;
                    if (keys != null && headerRequiredKeys.All(keys.Contains))
                    {
                        string tagNo = dictionary != null ? dictionary["TagNo"] : "";
                        string wapNo = dictionary != null ? dictionary["WAP"] : "";
                        string systemCode = dictionary != null ? dictionary["SystemCode"] : "";
                        string subSystemCode = dictionary != null ? dictionary["SubsystemCode"] : "";

                        var chkExistDevice = await _deviceService.GetAll(a => a.IsActive && !a.IsDeleted &&
                        a.Tag.ProjectId == projectId && a.Tag.TagName == tagNo).FirstOrDefaultAsync();

                        if (chkExistDevice != null)
                        {
                            var getSubSyetmInfo = await _subSystemService.GetAll(a => a.IsActive && !a.IsDeleted &&
                            a.Number == subSystemCode && a.System.Number == systemCode &&
                            a.System.WorkAreaPack.Number == wapNo && a.System.WorkAreaPack.ProjectId == projectId).FirstOrDefaultAsync();

                            if (getSubSyetmInfo != null)
                            {
                                chkExistDevice.SubSystemId = getSubSyetmInfo.Id;
                                _deviceService.Update(chkExistDevice, chkExistDevice, userId);
                            }

                        }
                        else if (missingTags.Count < 20)
                        {
                            missingTags.Add(tagNo);
                        }
                    }
                }
            }

            return missingTags;



        }
        #endregion        
    }
}
