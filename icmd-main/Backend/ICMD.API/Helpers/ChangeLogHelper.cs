using ICMD.Core.Account;
using ICMD.Core.Constants;
using ICMD.Core.DBModels;
using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Device;
using ICMD.Core.Dtos.JunctionBox;
using ICMD.Core.Dtos.Stand;
using ICMD.Core.Dtos.UIChangeLog;
using ICMD.Core.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ICMD.API.Helpers
{
    public class ChangeLogHelper
    {
        private readonly IUIChangeLogService _uiChangeLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITagService _tagService;
        private readonly IReferenceDocumentService _referenceDocumentService;
        private readonly Lazy<CommonMethods> _commonMethods;
        private readonly IDeviceModelService _deviceModelService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IFailStateService _failStateService;
        private readonly IJunctionBoxService _junctionBoxService;
        private readonly INatureOfSignalService _natureOfSignalService;
        private readonly IPanelService _panelService;
        private readonly IBankService _bankService;
        private readonly ITrainService _trainService;
        private readonly IZoneService _zoneService;
        private readonly ISkidService _skidService;
        private readonly IStandService _standService;
        private readonly ISubSystemService _subSystemService;
        private readonly IDeviceService _deviceService;
        private readonly IControlSystemHierarchyService _conntrolSystemHierarchyService;
        private readonly IAttributeDefinitionService _attributeDefinitionService;
        private StringBuilder changes = new StringBuilder();

        public ChangeLogHelper(IUIChangeLogService uiChangeLogService, IHttpContextAccessor httpContextAccessor, ITagService tagService,
            IReferenceDocumentService referenceDocumentService, Lazy<CommonMethods> commonMethods, IDeviceModelService deviceModelService,
            IDeviceTypeService deviceTypeService, IFailStateService failStateService, IJunctionBoxService junctionBoxService,
            INatureOfSignalService natureOfSignalService, IPanelService panelService, IBankService bankService, ITrainService trainService,
            IZoneService zoneService, ISkidService skidService, IStandService standService, ISubSystemService subSystemService,
            IDeviceService deviceService, IControlSystemHierarchyService conntrolSystemHierarchyService, IAttributeDefinitionService attributeDefinitionService)
        {
            _uiChangeLogService = uiChangeLogService;
            _httpContextAccessor = httpContextAccessor;
            _tagService = tagService;
            _referenceDocumentService = referenceDocumentService;
            _commonMethods = commonMethods;
            _deviceModelService = deviceModelService;
            _deviceTypeService = deviceTypeService;
            _failStateService = failStateService;
            _junctionBoxService = junctionBoxService;
            _natureOfSignalService = natureOfSignalService;
            _panelService = panelService;
            _bankService = bankService;
            _trainService = trainService;
            _zoneService = zoneService;
            _skidService = skidService;
            _standService = standService;
            _subSystemService = subSystemService;
            _deviceService = deviceService;
            _conntrolSystemHierarchyService = conntrolSystemHierarchyService;
            _attributeDefinitionService = attributeDefinitionService;
        }

        private Guid GetUserId()
        {
            // Assuming you're using ASP.NET Core Identity
            return new Guid(_httpContextAccessor.HttpContext.User.FindFirst(IdentityClaimNames.UserId).Value.ToString() ?? "");
        }

        private static string EncodeToXML(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace("&", "&amp;");
                text = text.Replace("\"", "&quot;");
                text = text.Replace("'", "&apos;");
                text = text.Replace("<", "&lt;");
                text = text.Replace(">", "&gt;");
            }

            return text;
        }

        private async Task<bool> CreateChangeLogItem(string changes, string tag, string type, Guid? parentTag = null)
        {
            UIChangeLog changeLog = new UIChangeLog();

            try
            {
                //if there are changes
                if (changes.Length > 0)
                {
                    //create change log item
                    changeLog = new UIChangeLog()
                    {
                        Changes = changes.ToString(),
                        Tag = tag,
                        Type = type
                    };
                    if (parentTag != null && parentTag != Guid.Empty)
                    {
                        var parent = await _deviceService.GetAll(d => d.TagId == parentTag && d.IsActive && !d.IsDeleted).FirstOrDefaultAsync();
                        var deviceType = await _deviceTypeService.GetAll(d => d.Type == "PLC" && d.IsActive && !d.IsDeleted).FirstOrDefaultAsync();

                        if (deviceType != null)
                        {
                            //search while parent is not null and it isn't of device type "PLC"
                            while (parent != null && parent.DeviceTypeId != deviceType.Id)
                            {
                                var controlSystemHierarchy = await _conntrolSystemHierarchyService.GetAll(a => a.ChildDeviceId == parent.Id && a.Instrument == false && !a.IsDeleted).FirstOrDefaultAsync();

                                parent = (controlSystemHierarchy != null) ? controlSystemHierarchy.ParentDevice : null;
                            }

                            if (parent != null)
                                changeLog.PLCNumber = parent.Tag.TagName ?? "";
                        }
                    }
                    await _uiChangeLogService.AddAsync(changeLog, GetUserId());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> CreateActivationChangeLog(bool activated, string tagName, string type, ChangeLogOptions option)
        {
            try
            {
                changes = new StringBuilder();
                changes.Append("<Changes>");
                changes.Append("<Type>").Append(EncodeToXML(type)).Append("</Type>");
                if (option == ChangeLogOptions.ActiveDeactive)
                {
                    if (activated)
                        changes.Append("<Activated>").Append(activated).Append("</Activated>");
                    else
                        changes.Append("<Activated>").Append(activated).Append("</Activated>");
                }
                else
                {
                    changes.Append("<Deleted>").Append(true).Append("</Deleted>");
                }

                changes.Append("</Changes>");

                await CreateChangeLogItem(changes.ToString(), tagName, type);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateJunctionBoxChangeLog(JunctionBox originalModel, CreateOrEditJunctionBoxDto newModel)
        {
            return await CreateChangeLog(originalModel, newModel, "Junction Box");
        }

        public async Task<bool> CreatePanelChangeLog(Panel originalModel, CreateOrEditJunctionBoxDto newModel)
        {
            return await CreateChangeLog(originalModel, newModel, "Panel");
        }

        public async Task<bool> CreateSkidChangeLog(Skid originalModel, CreateOrEditJunctionBoxDto newModel)
        {
            return await CreateChangeLog(originalModel, newModel, "Skid");
        }

        public async Task<bool> CreateStandChangeLog(Stand originalModel, CreateOrEditStandDto newModel)
        {
            try
            {
                var changes = new StringBuilder();
                Tag newTag = await _tagService.GetSingleAsync(a => a.Id == newModel.TagId);

                if (originalModel != null)
                {
                    if (originalModel.TagId != newModel.TagId)
                    {
                        string oldTag = originalModel.TagId != Guid.Empty ? _tagService.GetSingle(a => a.Id == originalModel.TagId)?.TagName ?? "" : "";
                        AppendStringChange(changes, "Tag", oldTag ?? "", newTag?.TagName ?? "");
                    }

                    if (originalModel.Type != newModel.Type)
                    {
                        AppendStringChange(changes, "Type", originalModel.Type ?? "", newModel.Type ?? "");
                    }

                    if (originalModel.Description != newModel.Description)
                    {
                        AppendStringChange(changes, "Description", originalModel.Description ?? "", newModel.Description ?? "");
                    }

                    if (originalModel.Area != newModel.Area)
                    {
                        AppendStringChange(changes, "Area", originalModel.Area ?? "", newModel.Area ?? "");
                    }

                    if (originalModel.ReferenceDocumentId != newModel.ReferenceDocumentId)
                    {
                        string oldValue = originalModel.ReferenceDocumentId != null
                            ? _commonMethods.Value.GenerateFullReportName(_referenceDocumentService.GetSingle(a => a.Id == originalModel.ReferenceDocumentId))
                            : "";

                        string newValue = newModel.ReferenceDocumentId != null
                            ? _commonMethods.Value.GenerateFullReportName(_referenceDocumentService.GetSingle(a => a.Id == newModel.ReferenceDocumentId))
                            : "";

                        AppendStringChange(changes, "ReferenceDocument", oldValue ?? "", newValue ?? "");
                    }

                    if (changes.Length > 0)
                    {
                        changes.Insert(0, $"<Changes><Type>Stand</Type><Properties>");
                        changes.Append("</Properties></Changes>");
                    }

                    await CreateChangeLogItem(changes.ToString(), newTag?.TagName ?? "", "Stand");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> CreateChangeLog<T>(T originalModel, CreateOrEditJunctionBoxDto newModel, string type)
        {
            try
            {
                var changes = new StringBuilder();
                Tag newTag = await _tagService.GetSingleAsync(a => a.Id == newModel.TagId);

                if (originalModel != null)
                {
                    string? tagId = originalModel.GetType().GetProperty("TagId")?.GetValue(originalModel)?.ToString();
                    string? referenceDocumentId = originalModel.GetType().GetProperty("ReferenceDocumentId")?.GetValue(originalModel)?.ToString();
                    if (tagId != newModel.TagId?.ToString())
                    {
                        string oldTag = !string.IsNullOrEmpty(tagId)
                            ? _tagService.GetSingle(a => a.Id.ToString() == tagId)?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "Tag", oldTag ?? "", newTag?.TagName ?? "");
                    }

                    if (originalModel.GetType().GetProperty("Type")?.GetValue(originalModel)?.ToString() != newModel.Type)
                    {
                        AppendStringChange(changes, "Type", originalModel.GetType().GetProperty("Type")?.GetValue(originalModel)?.ToString() ?? "", newModel.Type ?? "");
                    }

                    if (originalModel.GetType().GetProperty("Description")?.GetValue(originalModel)?.ToString() != newModel.Description)
                    {
                        AppendStringChange(changes, "Description", originalModel.GetType().GetProperty("Description")?.GetValue(originalModel)?.ToString() ?? "", newModel.Description ?? "");
                    }

                    if (referenceDocumentId != newModel.ReferenceDocumentId?.ToString())
                    {
                        string oldValue = !string.IsNullOrEmpty(referenceDocumentId)
                            ? _commonMethods.Value.GenerateFullReportName(_referenceDocumentService.GetSingle(a => a.Id.ToString() == referenceDocumentId))
                            : "";

                        string newValue = newModel.ReferenceDocumentId != null
                            ? _commonMethods.Value.GenerateFullReportName(_referenceDocumentService.GetSingle(a => a.Id == newModel.ReferenceDocumentId))
                            : "";

                        AppendStringChange(changes, "ReferenceDocument", oldValue ?? "", newValue ?? "");
                    }

                    if (changes.Length > 0)
                    {
                        changes.Insert(0, $"<Changes><Type>{type}</Type><Properties>");
                        changes.Append("</Properties></Changes>");
                    }

                    await CreateChangeLogItem(changes.ToString(), newTag?.TagName ?? "", type);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public async Task<bool> CreateDeviceChangeLog(Device originalModel, CreateOrEditDeviceDto newModel, Guid? originalConnectionParentTagId,
            Guid? originalInstrumentParentTagId, List<Guid>? newReferenceDocumentIds = null, List<Guid>? deletedReferenceDocumentIds = null, AddUpdateDeleteAttributeInfoDto attributeData = null)
        {
            try
            {
                var changes = new StringBuilder();
                Tag newTag = await _tagService.GetSingleAsync(a => a.Id == newModel.TagId);

                if (originalModel != null)
                {
                    if (originalModel.Id != Guid.Empty)
                    {
                        ControlSystemHierarchy? connectionParent = await _conntrolSystemHierarchyService.GetAll(s => s.ChildDeviceId == originalModel.Id && s.Instrument == false && !s.IsDeleted).FirstOrDefaultAsync();
                        if (connectionParent != null)
                        {
                            originalConnectionParentTagId = connectionParent.ParentDeviceId;
                        }

                        ControlSystemHierarchy? instrumentParent = await _conntrolSystemHierarchyService.GetAll(s => s.ChildDeviceId == originalModel.Id && s.Instrument == true && !s.IsDeleted).FirstOrDefaultAsync();
                        if (instrumentParent != null)
                        {
                            originalInstrumentParentTagId = instrumentParent.ParentDeviceId;
                        }
                    }

                    #region Device
                    if (originalModel.IsInstrument != newModel.IsInstrument)
                    {
                        AppendStringChange(changes, "IsInstrument", originalModel.IsInstrument, newModel.IsInstrument);
                    }

                    if (!string.IsNullOrEmpty(originalModel.LineVesselNumber) && !string.IsNullOrEmpty(newModel.LineVesselNumber) && originalModel.LineVesselNumber != newModel.LineVesselNumber)
                    {
                        AppendStringChange(changes, "LineVesselNumber", originalModel.LineVesselNumber ?? "", newModel.LineVesselNumber ?? "");
                    }

                    if (!string.IsNullOrEmpty(originalModel.RevisionChanges) && !string.IsNullOrEmpty(newModel.RevisionChanges) && originalModel.RevisionChanges != newModel.RevisionChanges)
                    {
                        AppendStringChange(changes, "RevisionChanges", originalModel.RevisionChanges ?? "", newModel.RevisionChanges ?? "");
                    }

                    if (!string.IsNullOrEmpty(originalModel.SerialNumber) && !string.IsNullOrEmpty(newModel.SerialNumber) && originalModel.SerialNumber != newModel.SerialNumber)
                    {
                        AppendStringChange(changes, "SerialNumber", originalModel.SerialNumber ?? "", newModel.SerialNumber ?? "");
                    }

                    if (!string.IsNullOrEmpty(originalModel.Service) && !string.IsNullOrEmpty(newModel.Service) && originalModel.Service != newModel.Service)
                    {
                        AppendStringChange(changes, "Service", originalModel.Service ?? "", newModel.Service ?? "");
                    }

                    if (!string.IsNullOrEmpty(originalModel.Variable) && !string.IsNullOrEmpty(newModel.Variable) && originalModel.Variable != newModel.Variable)
                    {
                        AppendStringChange(changes, "Variable", originalModel.Variable ?? "", newModel.Variable ?? "");
                    }

                    if (!string.IsNullOrEmpty(originalModel.ServiceDescription) && !string.IsNullOrEmpty(newModel.ServiceDescription) && originalModel.ServiceDescription != newModel.ServiceDescription)
                    {
                        AppendStringChange(changes, "ServiceDescription", originalModel.ServiceDescription ?? "", newModel.ServiceDescription ?? "");
                    }

                    if (originalModel.TagId != newModel.TagId)
                    {
                        string oldTag = originalModel.TagId != Guid.Empty ? _tagService.GetSingle(a => a.Id == originalModel.TagId)?.TagName ?? "" : "";
                        AppendStringChange(changes, "Tag", oldTag ?? "", newTag?.TagName ?? "");
                    }

                    if (originalModel.VendorSupply != newModel.VendorSupply)
                    {
                        AppendStringChange(changes, "VendorSupply", originalModel.VendorSupply?.ToString() ?? "", newModel.VendorSupply?.ToString() ?? "");
                    }

                    if (originalModel.HistoricalLogging != newModel.HistoricalLogging)
                    {
                        AppendStringChange(changes, "HistoricalLogging", originalModel.HistoricalLogging?.ToString() ?? "", newModel.HistoricalLogging?.ToString() ?? "");
                    }

                    if (originalModel.HistoricalLoggingFrequency != newModel.HistoricalLoggingFrequency)
                    {
                        AppendStringChange(changes, "HistoricalLoggingFrequency", originalModel.HistoricalLoggingFrequency?.ToString() ?? "", newModel.HistoricalLoggingFrequency?.ToString() ?? "");
                    }

                    if (originalModel.HistoricalLoggingResolution != newModel.HistoricalLoggingResolution)
                    {
                        AppendStringChange(changes, "HistoricalLoggingResolution", originalModel.HistoricalLoggingResolution?.ToString() ?? "", newModel.HistoricalLoggingResolution?.ToString() ?? "");
                    }

                    if (originalModel.DeviceModelId != newModel.DeviceModelId)
                    {
                        string oldValue = originalModel.DeviceModelId != null
                           ? _deviceModelService.GetSingle(a => a.Id == originalModel.DeviceModelId)?.Model ?? ""
                           : "";

                        string newValue = newModel.DeviceModelId != null
                            ? _deviceModelService.GetSingle(a => a.Id == newModel.DeviceModelId)?.Model ?? ""
                            : "";

                        AppendStringChange(changes, "DeviceModel", oldValue, newValue);
                    }

                    if (originalModel.DeviceTypeId != newModel.DeviceTypeId)
                    {
                        string oldValue = originalModel.DeviceTypeId != Guid.Empty
                           ? _deviceTypeService.GetSingle(a => a.Id == originalModel.DeviceTypeId)?.Type ?? ""
                           : "";

                        string newValue = newModel.DeviceTypeId != Guid.Empty
                            ? _deviceTypeService.GetSingle(a => a.Id == newModel.DeviceTypeId)?.Type ?? ""
                            : "";

                        AppendStringChange(changes, "DeviceType", oldValue, newValue);
                    }

                    if (originalModel.FailStateId != newModel.FailStateId)
                    {
                        string oldValue = originalModel.FailStateId != null
                           ? _failStateService.GetSingle(a => a.Id == originalModel.FailStateId)?.FailStateName ?? ""
                           : "";

                        string newValue = newModel.FailStateId != null
                            ? _failStateService.GetSingle(a => a.Id == newModel.FailStateId)?.FailStateName ?? ""
                            : "";

                        AppendStringChange(changes, "FailState", oldValue, newValue);
                    }


                    if (originalModel.JunctionBoxTagId != newModel.JunctionBoxTagId)
                    {
                        string oldValue = originalModel.JunctionBoxTagId != null
                           ? _junctionBoxService.GetSingle(a => a.Id == originalModel.JunctionBoxTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.JunctionBoxTagId != null
                            ? _junctionBoxService.GetSingle(a => a.Id == newModel.JunctionBoxTagId)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "JunctionBox", oldValue, newValue);
                    }

                    if (originalModel.NatureOfSignalId != newModel.NatureOfSignalId)
                    {
                        string oldValue = originalModel.NatureOfSignalId != null
                           ? _natureOfSignalService.GetSingle(a => a.Id == originalModel.NatureOfSignalId)?.NatureOfSignalName ?? ""
                           : "";

                        string newValue = newModel.NatureOfSignalId != null
                            ? _natureOfSignalService.GetSingle(a => a.Id == newModel.NatureOfSignalId)?.NatureOfSignalName ?? ""
                            : "";

                        AppendStringChange(changes, "NatureOfSignal", oldValue, newValue);
                    }

                    if (originalModel.PanelTagId != newModel.PanelTagId)
                    {
                        string oldValue = originalModel.PanelTagId != null
                           ? _panelService.GetSingle(a => a.Id == originalModel.PanelTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.PanelTagId != null
                            ? _panelService.GetSingle(a => a.Id == newModel.PanelTagId)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "Panel", oldValue, newValue);
                    }

                    if (originalModel.ServiceBankId != newModel.ServiceBankId)
                    {
                        string oldValue = originalModel.ServiceBankId != null
                           ? _bankService.GetSingle(a => a.Id == originalModel.ServiceBankId)?.Bank ?? ""
                           : "";

                        string newValue = newModel.ServiceBankId != null
                            ? _bankService.GetSingle(a => a.Id == newModel.ServiceBankId)?.Bank ?? ""
                            : "";

                        AppendStringChange(changes, "Bank", oldValue, newValue);
                    }

                    if (originalModel.ServiceTrainId != newModel.ServiceTrainId)
                    {
                        string oldValue = originalModel.ServiceTrainId != null
                           ? _trainService.GetSingle(a => a.Id == originalModel.ServiceTrainId)?.Train ?? ""
                           : "";

                        string newValue = newModel.ServiceTrainId != null
                            ? _trainService.GetSingle(a => a.Id == newModel.ServiceTrainId)?.Train ?? ""
                            : "";

                        AppendStringChange(changes, "Train", oldValue, newValue);
                    }

                    if (originalModel.ServiceZoneId != newModel.ServiceZoneId)
                    {
                        string oldValue = originalModel.ServiceZoneId != null
                           ? _zoneService.GetSingle(a => a.Id == originalModel.ServiceZoneId)?.Zone ?? ""
                           : "";

                        string newValue = newModel.ServiceZoneId != null
                            ? _zoneService.GetSingle(a => a.Id == newModel.ServiceZoneId)?.Zone ?? ""
                            : "";

                        AppendStringChange(changes, "Zone", oldValue, newValue);
                    }

                    if (originalModel.SkidTagId != newModel.SkidTagId)
                    {
                        string oldValue = originalModel.SkidTagId != null
                           ? _skidService.GetSingle(a => a.Id == originalModel.SkidTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.SkidTagId != null
                            ? _skidService.GetSingle(a => a.Id == newModel.SkidTagId)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "Skid", oldValue, newValue);
                    }

                    if (originalModel.StandTagId != newModel.StandTagId)
                    {
                        string oldValue = originalModel.StandTagId != null
                           ? _standService.GetSingle(a => a.Id == originalModel.StandTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.StandTagId != null
                            ? _standService.GetSingle(a => a.Id == newModel.StandTagId)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "Stand", oldValue, newValue);
                    }

                    if (originalModel.SubSystemId != newModel.SubSystemId)
                    {
                        string oldValue = originalModel.SubSystemId != null
                           ? _subSystemService.GetSingle(a => a.Id == originalModel.SubSystemId)?.Number ?? ""
                           : "";

                        string newValue = newModel.SubSystemId != null
                            ? _subSystemService.GetSingle(a => a.Id == newModel.SubSystemId)?.Number ?? ""
                            : "";

                        AppendStringChange(changes, "SubSystem", oldValue, newValue);
                    }

                    if (originalConnectionParentTagId != newModel.ConnectionParentTagId)
                    {
                        string oldValue = originalConnectionParentTagId != null
                           ? _deviceService.GetSingle(a => a.TagId == originalConnectionParentTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.ConnectionParentTagId != null
                            ? _deviceService.GetFirstOrDefault(a => a.TagId == newModel.ConnectionParentTagId)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "ConnectionParent", oldValue, newValue);
                    }

                    if (originalInstrumentParentTagId != newModel.InstrumentParentTagId)
                    {
                        string oldValue = originalInstrumentParentTagId != null
                           ? _deviceService.GetSingle(a => a.TagId == originalInstrumentParentTagId)?.Tag?.TagName ?? ""
                           : "";

                        string newValue = newModel.InstrumentParentTagId != null
                            ? _deviceService.GetSingle(a => a.TagId == newModel.InstrumentParentTagId && !a.IsDeleted)?.Tag?.TagName ?? ""
                            : "";

                        AppendStringChange(changes, "InstrumentParent", oldValue, newValue);
                    }

                    if (changes.Length > 0)
                    {
                        var temp = changes.ToString();
                        changes.Clear();
                        changes.Append("<Properties>").Append(temp).Append("</Properties>");
                    }
                    #endregion

                    #region Attributes
                    if (attributeData.NewAttributes.Count() > 0 || attributeData.DeletedAttributes.Count() > 0 || attributeData.ModifiedAttributes.Count() > 0)
                    {
                        changes.Append("<Attributes>");

                        if (attributeData.NewAttributes != null)
                        {
                            foreach (var attribute in attributeData.NewAttributes)
                            {
                                if (!string.IsNullOrEmpty(attribute.Value))
                                {
                                    changes.Append("<Attribute>");
                                    changes.Append("<Name>").Append(EncodeToXML(attribute?.Name ?? "")).Append("</Name>");
                                    changes.Append("<OriginalValue>").Append("").Append("</OriginalValue>");
                                    changes.Append("<NewValue>").Append(EncodeToXML(attribute?.Value ?? "")).Append("</NewValue>");
                                    changes.Append("<Status>").Append("Added").Append("</Status>");
                                    changes.Append("</Attribute>");
                                }
                            }

                        }

                        if (attributeData.ModifiedAttributes != null)
                        {
                            foreach (var attribute in attributeData.ModifiedAttributes)
                            {
                                if (attribute.OriginalValue != attribute.Value)
                                {
                                    changes.Append("<Attribute>");
                                    changes.Append("<Name>").Append(EncodeToXML(attribute?.Name ?? "")).Append("</Name>");
                                    changes.Append("<OriginalValue>").Append(EncodeToXML(attribute?.OriginalValue ?? "")).Append("</OriginalValue>");
                                    changes.Append("<NewValue>").Append(EncodeToXML(attribute?.Value ?? "")).Append("</NewValue>");
                                    changes.Append("<Status>").Append("Updated").Append("</Status>");
                                    changes.Append("</Attribute>");
                                }
                            }
                        }

                        if (attributeData.DeletedAttributes != null)
                        {
                            foreach (var attribute in attributeData.DeletedAttributes)
                            {
                                if (!string.IsNullOrEmpty(attribute.Value))
                                {
                                    changes.Append("<Attribute>");
                                    changes.Append("<Name>").Append(EncodeToXML(attribute?.Name ?? "")).Append("</Name>");
                                    changes.Append("<OriginalValue>").Append(EncodeToXML(attribute?.Value ?? "")).Append("</OriginalValue>");
                                    changes.Append("<NewValue>").Append("").Append("</NewValue>");
                                    changes.Append("<Status>").Append("Removed").Append("</Status>");
                                    changes.Append("</Attribute>");
                                }
                            }
                        }


                        changes.Append("</Attributes>");
                    }
                    #endregion

                    #region ReferenceDocument   
                    if ((newReferenceDocumentIds != null && newReferenceDocumentIds.Any()) || (deletedReferenceDocumentIds != null && deletedReferenceDocumentIds.Any()))
                    {
                        changes.Append("<ReferenceDocuments>");

                        if (newReferenceDocumentIds != null && newReferenceDocumentIds.Any())
                        {
                            List<ReferenceDocument> newReferenceDocuments = await _referenceDocumentService.GetAll(a => newReferenceDocumentIds.Contains(a.Id)).ToListAsync();
                            foreach (var document in newReferenceDocuments)
                            {
                                changes.Append("<Document>");
                                changes.Append("<Type>").Append(EncodeToXML(document.ReferenceDocumentType?.Type ?? "")).Append("</Type>");
                                changes.Append("<DocumentNumber>").Append(EncodeToXML(document.DocumentNumber ?? "")).Append("</DocumentNumber>");
                                changes.Append("<Revision>").Append(EncodeToXML(document.Revision ?? "")).Append("</Revision>");
                                changes.Append("<Version>").Append(EncodeToXML(document.Version ?? "")).Append("</Version>");
                                changes.Append("<Sheet>").Append(EncodeToXML(document.Sheet ?? "")).Append("</Sheet>");
                                changes.Append("<Status>").Append("Added").Append("</Status>");
                                changes.Append("</Document>");
                            }
                        }

                        if (deletedReferenceDocumentIds != null && deletedReferenceDocumentIds.Any())
                        {
                            List<ReferenceDocument> deletedReferenceDocuments = await _referenceDocumentService.GetAll(a => deletedReferenceDocumentIds.Contains(a.Id)).ToListAsync();
                            foreach (var document in deletedReferenceDocuments)
                            {
                                changes.Append("<Document>");
                                changes.Append("<Type>").Append(EncodeToXML(document.ReferenceDocumentType?.Type ?? "")).Append("</Type>");
                                changes.Append("<DocumentNumber>").Append(EncodeToXML(document.DocumentNumber ?? "")).Append("</DocumentNumber>");
                                changes.Append("<Revision>").Append(EncodeToXML(document.Revision ?? "")).Append("</Revision>");
                                changes.Append("<Version>").Append(EncodeToXML(document.Version ?? "")).Append("</Version>");
                                changes.Append("<Sheet>").Append(EncodeToXML(document.Sheet ?? "")).Append("</Sheet>");
                                changes.Append("<Status>").Append("Removed").Append("</Status>");
                                changes.Append("</Document>");
                            }
                        }

                        changes.Append("</ReferenceDocuments>");
                    }
                    #endregion
                    //if there are any changes, create changes xml string
                    if (changes.Length > 0)
                    {
                        var temp = changes.ToString();
                        changes.Clear();
                        changes.Append("<Changes>");
                        changes.Append("<Type>Device</Type>");
                        changes.Append(temp);
                        changes.Append("</Changes>");
                    }

                    //Attributes

                    await CreateChangeLogItem(changes.ToString(), newTag?.TagName ?? "", "Device", newModel.ConnectionParentTagId);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private void AppendStringChange(StringBuilder changes, string propertyName, string originalValue, string newValue)
        {
            if (originalValue != newValue)
            {
                changes.Append("<Property>")
                       .Append("<Name>").Append(propertyName).Append("</Name>")
                       .Append("<OriginalValue>").Append(EncodeToXML(originalValue ?? "")).Append("</OriginalValue>")
                       .Append("<NewValue>").Append(EncodeToXML(newValue)).Append("</NewValue>")
                       .Append("</Property>");
            }
        }

        #region Change Log - Bulk Edit
        public async Task<bool> CreateBulkDeleteLog(string module, List<BulkDeleteLogDto> bulkLogs)
        {
            try
            {
                changes = new StringBuilder();
                changes.Append("<Changes>");
                changes.Append("<Type>").Append(EncodeToXML("Bulk Delete")).Append("</Type>");
                changes.Append("<Records>");
                foreach (BulkDeleteLogDto log in bulkLogs)
                {
                    changes.Append("<Record>");
                    changes.Append("<Name>").Append(EncodeToXML(log.Name ?? "")).Append("</Name>");
                    changes.Append("<Status>").Append(log.Status ?? false).Append("</Status>");
                    changes.Append("<Message>").Append(EncodeToXML(log.Message ?? "")).Append("</Message>");
                    changes.Append("</Record>");
                }
                changes.Append("</Records>");
                changes.Append("</Changes>");

                await CreateChangeLogItem(changes.ToString(), module, "Bulk Delete");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Import Logs
        public async Task<bool> CreateImportLogs(string module, List<ImportLogDto> importLogs)
        {
            try
            {
                if (importLogs.Count == 0) 
                    return false;

                changes = new StringBuilder();
                changes.Append("<Changes>");
                changes.Append("<Type>").Append(EncodeToXML("Import")).Append("</Type>");
                changes.Append("<Records>");
                foreach (ImportLogDto log in importLogs)
                {
                    changes.Append("<Record>");
                    changes.Append("<Name>").Append(EncodeToXML(log.Name ?? "")).Append("</Name>");
                    changes.Append("<Status>").Append(log.Status).Append("</Status>");
                    changes.Append("<Operation>").Append(log.Operation ?? "").Append("</Operation>");
                    changes.Append("<Message>").Append(EncodeToXML(log.Message ?? "")).Append("</Message>");

                    foreach (var item in log.Items)
                    {
                        changes.Append("<Items>");
                        changes.Append("<ItemColumnName>").Append(EncodeToXML(item.ItemColumnName ?? "")).Append("</ItemColumnName>");
                        changes.Append("<PreviousValue>").Append(EncodeToXML(item.PreviousValue ?? "")).Append("</PreviousValue>");
                        changes.Append("<NewValue>").Append(EncodeToXML(item.NewValue ?? "")).Append("</NewValue>");
                        changes.Append("</Items>");
                    }
                    changes.Append("</Record>");
                }
                changes.Append("</Records>");
                changes.Append("</Changes>");

                await CreateChangeLogItem(changes.ToString(), module, "Import");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
