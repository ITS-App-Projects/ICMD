using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ICMD.Core.DBModels
{
    public class Tag : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string TagName { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? SequenceNumber { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? EquipmentIdentifier { get; set; }

        public Guid? ProcessId { get; set; }
        [ForeignKey("ProcessId")]
        public virtual Process? Process { get; set; }

        public Guid? SubProcessId { get; set; }
        [ForeignKey("SubProcessId")]
        public virtual SubProcess? SubProcess { get; set; }

        public Guid? StreamId { get; set; }
        [ForeignKey("StreamId")]
        public virtual Stream? Stream { get; set; }

        public Guid? EquipmentCodeId { get; set; }

        [ForeignKey("EquipmentCodeId")]
        public virtual EquipmentCode? EquipmentCode { get; set; }

        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public Guid? TagTypeId { get; set; }

        [ForeignKey("TagTypeId")]
        public virtual TagType? TagType { get; set; }

        public Guid? TagDescriptorId { get; set; }

        [ForeignKey("TagDescriptorId")]
        public virtual TagDescriptor? TagDescriptor { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? Field1String { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? Field2String { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? Field3String { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? Field4String { get; set; }
    }
}
