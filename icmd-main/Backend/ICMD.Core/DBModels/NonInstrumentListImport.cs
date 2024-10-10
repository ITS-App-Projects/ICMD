using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class NonInstrumentListImport : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProcessNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SubProcess { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Stream { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? EquipmentCode { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SequenceNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? EquipmentIdentifier { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Tag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DeviceType { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? NatureOfSignal { get; set; }

        public int? DPNodeAddress { get; set; }
        public int? NoSlotsChannels { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ConnectionParent { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PLCNumber { get; set; }
        public int? PLCSlotNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Location { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Manufacturer { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ModelNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ModelDescription { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ArchitecturalDrawing { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ArchitecturalDrawingSheet { get; set; }
        public int? Revision { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? RevisionChanges { get; set; }
    }
}
