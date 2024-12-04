using ICMD.Core.Dtos.Attributes;
using ICMD.Core.Dtos.Reference_Document;

namespace ICMD.Core.Dtos.Device
{
    public class ViewDeviceInfoDto
    {
        public Guid Id { get; set; }
        public string? DeviceType { get; set; }
        public string? Tag { get; set; }
        public string? IsInstrument { get; set; }
        public string? Manufacturer { get; set; }
        public string? DeviceModel { get; set; }
        public string? LineVesselNumber { get; set; }
        public bool? VendorSupply { get; set; }
        public string? FailState { get; set; }
        public int? Area { get; set; }
        public string? ServiceZone { get; set; }
        public string? ServiceBank { get; set; }
        public string? Service { get; set; }
        public string? ServiceTrain { get; set; }
        public string? Variable { get; set; }
        public string? NatureOfSignal { get; set; }
        public string? ServiceDescription { get; set; }
        public string? SkidTag { get; set; }
        public string? PanelTag { get; set; }
        public string? JunctionBoxTag { get; set; }
        public string? StandTag { get; set; }
        public string? SerialNumber { get; set; }
        public bool? HistoricalLogging { get; set; }
        public double? HistoricalLoggingFrequency { get; set; }
        public double? HistoricalLoggingResolution { get; set; }
        public string? RevisionChanges { get; set; }
        public string? WorkAreaPack { get; set; }
        public string? System { get; set; }
        public string? SubSystem { get; set; }
        public List<ReferenceDocumentInfoDto>? ReferenceDocumentInfo { get; set; }
        public List<AttributeValueDto>? Attributes { get; set; }
        public string? Status { get; set; }
        public string? ProcessName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StreamName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? SequenceNumber { get; set; }
        public int? Revision { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public string? ConnectionParentTag { get; set; }
        public string? InstrumentParentTag { get; set; }
        public string? OriginCableTag { get; set; }
        public string? DestinationCableTag { get; set; }
    }
}
