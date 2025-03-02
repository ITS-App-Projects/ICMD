using ICMD.Core.Dtos.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.Dtos.Device
{
    public class CreateOrEditDeviceDto
    {
        public Guid Id { get; set; }
        public Guid DeviceTypeId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }
        public string IsInstrument { get; set; }
        public Guid? ManufacturerId { get; set; }
        public Guid? DeviceModelId { get; set; }
        public Guid? ConnectionParentTagId { get; set; }
        public Guid? InstrumentParentTagId { get; set; }

        [MaxLength(255, ErrorMessage = "Line Vessel Number must be less than 255 characters in length.")]
        public string? LineVesselNumber { get; set; }
        public bool? VendorSupply { get; set; }
        public Guid? FailStateId { get; set; }
        public Guid? ServiceZoneId { get; set; }
        public Guid? ServiceBankId { get; set; }

        [MaxLength(12, ErrorMessage = "Service must be less than 12 characters in length.")]
        public string? Service { get; set; }
        public Guid? ServiceTrainId { get; set; }

        [MaxLength(50, ErrorMessage = "Variable must be less than 50 characters in length.")]
        public string Variable { get; set; }
        public Guid? NatureOfSignalId { get; set; }

        [MaxLength(32, ErrorMessage = "Service Description must be less than 32 characters in length.")]
        public string? ServiceDescription { get; set; }
        public Guid? SkidTagId { get; set; }
        public Guid? PanelTagId { get; set; }
        public Guid? JunctionBoxTagId { get; set; }
        public Guid? StandTagId { get; set; }

        [MaxLength(50, ErrorMessage = "Serial Number must be less than 50 characters in length.")]
        public string? SerialNumber { get; set; }
        public bool? HistoricalLogging { get; set; }
        public double? HistoricalLoggingFrequency { get; set; }
        public double? HistoricalLoggingResolution { get; set; }

        [MaxLength(1000, ErrorMessage = "Revision Changes must be less than 1000 characters in length.")]
        public string? RevisionChanges { get; set; }
        public Guid? WorkAreaPackId { get; set; }
        public Guid? SystemId { get; set; }
        public Guid? SubSystemId { get; set; }
        public List<Guid> ReferenceDocumentIds { get; set; } = [];

        public List<AttributeValueDto>? Attributes { get; set; }

        public Guid? ConnectionCableTagId { get; set; }
        public Guid? InstrumentCableTagId { get; set; }
    }
}
