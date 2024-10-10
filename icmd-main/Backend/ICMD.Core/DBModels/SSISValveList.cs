using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SSISValveList : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PnPId { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProcessNumber { get; set; }

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
        public string? DWGTitle { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Rev { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Version { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Size { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? FluidCode { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PipeLinesTag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PipingClass { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ClassName { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? OnSkid { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Failure { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Switches { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? From { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? To { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Accessories { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DesignTemp { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? NominalPressure { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ValveSpecNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PNRating { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? TrackingNumber { get; set; }
        public Guid ProjectId { get; set; }
    }
}
