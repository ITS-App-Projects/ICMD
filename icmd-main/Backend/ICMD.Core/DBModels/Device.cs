using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;

namespace ICMD.Core.DBModels
{
    public class Device : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(32)")]
        [MaxLength(32)]
        public string? ServiceDescription { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? LineVesselNumber { get; set; }
        public bool? VendorSupply { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? SerialNumber { get; set; }
        public bool? HistoricalLogging { get; set; }
        public double? HistoricalLoggingFrequency { get; set; }
        public double? HistoricalLoggingResolution { get; set; }
        public int Revision { get; set; }

        [Column(TypeName = "character varying(1000)")]
        [MaxLength(1000)]
        public string? RevisionChanges { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Service { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Variable { get; set; }

        [Column(TypeName = "character varying(1)")]
        [MaxLength(1)]
        public string IsInstrument { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public Guid DeviceTypeId { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType DeviceType { get; set; }

        public Guid? DeviceModelId { get; set; }
        [ForeignKey("DeviceModelId")]
        public virtual DeviceModel? DeviceModel { get; set; }

        public Guid? ProcessLevelId { get; set; }
        [ForeignKey("ProcessLevelId")]
        public virtual ProcessLevel? ProcessLevel { get; set; }

        public Guid? ServiceZoneId { get; set; }
        [ForeignKey("ServiceZoneId")]
        public virtual ServiceZone? ServiceZone { get; set; }

        public Guid? ServiceBankId { get; set; }
        [ForeignKey("ServiceBankId")]
        public virtual ServiceBank? ServiceBank { get; set; }

        public Guid? ServiceTrainId { get; set; }
        [ForeignKey("ServiceTrainId")]
        public virtual ServiceTrain? ServiceTrain { get; set; }

        public Guid? NatureOfSignalId { get; set; }
        [ForeignKey("NatureOfSignalId")]
        public virtual NatureOfSignal? NatureOfSignal { get; set; }

        public Guid? PanelTagId { get; set; }
        [ForeignKey("PanelTagId")]
        public virtual Tag? PanelTag { get; set; }

        public Guid? SkidTagId { get; set; }
        [ForeignKey("SkidTagId")]
        public virtual Tag? SkidTag { get; set; }

        public Guid? StandTagId { get; set; }
        [ForeignKey("StandTagId")]
        public virtual Tag? StandTag { get; set; }

        public Guid? JunctionBoxTagId { get; set; }
        [ForeignKey("JunctionBoxTagId")]
        public virtual Tag? JunctionBoxTag { get; set; }

        public Guid? FailStateId { get; set; }
        [ForeignKey("FailStateId")]
        public virtual FailState? FailState { get; set; }

        public Guid? SubSystemId { get; set; }
        [ForeignKey("SubSystemId")]
        public virtual SubSystem? SubSystem { get; set; }
    }
}
