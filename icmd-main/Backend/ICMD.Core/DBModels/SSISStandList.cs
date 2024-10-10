using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SSISStandList : FullEntityWithAudit<Guid>
    {
        public double? Item { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Rev { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandTag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandType { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Qrea { get; set; }
        public double? QTY { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DrawingReference { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Figure { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? AFD1 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? AFD2 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? AFD3 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? AFDPlates { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DPH1 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DPH2 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DPHPlates { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Instrument1 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Instrument2 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Instrument3 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Instrument4 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ReasonsForChange { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ChangeBy { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? F23 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? F24 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? F25 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? F26 { get; set; }
    }
}
