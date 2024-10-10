using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class AttributeValue : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string? Revision { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string? Unit { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Requirement { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Value { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string? ItemNumber { get; set; }

        public bool IncludeInDatasheet { get; set; }

        public Guid? DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device? Device { get; set; }

        public Guid? DeviceTypeId { get; set; }
        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }

        public Guid? DeviceModelId { get; set; }
        [ForeignKey("DeviceModelId")]
        public virtual DeviceModel? DeviceModel { get; set; }

        public Guid? NatureOfSignalId { get; set; }
        [ForeignKey("NatureOfSignalId")]
        public virtual NatureOfSignal? NatureOfSignal { get; set; }

        public Guid AttributeDefinitionId { get; set; }
        [ForeignKey("AttributeDefinitionId")]
        public virtual AttributeDefinition AttributeDefinition { get; set; }
    }
}
