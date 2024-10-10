using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class AttributeDefinition : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Category { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(255)]
        public string? ValueType { get; set; }
        public bool Inherit { get; set; }
        public bool Private { get; set; }
        public bool Required { get; set; }
        public Guid? DeviceTypeId { get; set; }

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }

        public Guid? DeviceModelId { get; set; }
        [ForeignKey("DeviceModelId")]
        public virtual DeviceModel? DeviceModel { get; set; }

        public Guid? NatureOfSignalId { get; set; }
        [ForeignKey("NatureOfSignalId")]
        public virtual NatureOfSignal? NatureOfSignal { get; set; }
    }
}
