using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Reference_Document;

namespace ICMD.Core.Dtos.Device
{
    public class DeviceInfoDto
    {
        public Guid Id { get; set; }
        public Guid DeviceTypeId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }
        public string? IsInstrument { get; set; }
        public Guid? ManufacturerId { get; set; }
        public Guid? DeviceModelId { get; set; }
        public Guid? ConnectionParentTagId { get; set; }
        public Guid? InstrumentParentTagId { get; set; }
        public string? LineVesselNumber { get; set; }
        public bool? VendorSupply { get; set; }
        public Guid? FailStateId { get; set; }
        public Guid? ServiceZoneId { get; set; }
        public Guid? ServiceBankId { get; set; }
        public string? Service { get; set; }
        public Guid? ServiceTrainId { get; set; }
        public string? Variable { get; set; }
        public Guid? NatureOfSignalId { get; set; }
        public string? ServiceDescription { get; set; }
        public Guid? SkidTagId { get; set; }
        public Guid? PanelTagId { get; set; }
        public Guid? JunctionBoxTagId { get; set; }
        public Guid? StandTagId { get; set; }
        public string? SerialNumber { get; set; }
        public bool? HistoricalLogging { get; set; }
        public double? HistoricalLoggingFrequency { get; set; }
        public double? HistoricalLoggingResolution { get; set; }
        public string? RevisionChanges { get; set; }
        public Guid? WorkAreaPackId { get; set; }
        public Guid? SystemId { get; set; }
        public Guid? SubSystemId { get; set; }
        public List<Guid>? ReferenceDocumentIds { get; set; }
        public List<ReferenceDocumentInfoDto>? ReferenceDocumentInfo { get; set; }
        public List<AttributeValueDto>? Attributes { get; set; }
        public Guid? ConnectionCableTagId { get; set; }
        public Guid? InstrumentCableTagId { get; set; }
    }
}
