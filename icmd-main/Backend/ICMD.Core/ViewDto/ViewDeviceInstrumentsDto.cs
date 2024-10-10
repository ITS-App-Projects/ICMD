namespace ICMD.Core.ViewDto
{
    public class ViewDeviceInstrumentsDto
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? ModelDescription { get; set; }
        public string? TagName { get; set; }
        public string? Type { get; set; }
        public string? ProcessName { get; set; }
        public string? Zone { get; set; }
        public string? Bank { get; set; }
        public string? Service { get; set; }
        public string? NatureOfSignalName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StreamName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? ServiceDescription { get; set; }
        public string? LineVesselNumber { get; set; }
        public bool VendorSupply { get; set; }
        public string? FailStateName { get; set; }
        public int Revision { get; set; }
        public string? RevisionChanges { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? Manufacturer { get; set; }
        public string? Variable { get; set; }
        public string? Train { get; set; }
        public Guid?  DeviceModelId { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public Guid? PanelTagId { get; set; }
        public Guid? SkidTagId { get; set; }
        public Guid? StandTagId { get; set; }
        public Guid? JunctionBoxTagId { get; set; }
        public Guid? DeviceTypeId { get; set; }
        public string? IsInstrument { get; set; }
        public string? SubSystem { get; set; }
        public string? System { get; set; }
        public string? WorkAreaPack { get; set; }
        public bool? HistoricalLogging { get; set; }
        public double? HistoricalLoggingFrequency { get; set; }
        public double? HistoricalLoggingResolution { get; set; }
        public Guid ProjectId { get; set; }
    }
}
