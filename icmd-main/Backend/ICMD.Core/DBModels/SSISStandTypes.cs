using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SSISStandTypes : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Item { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Rev { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandCode { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandType { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? VendorReferenceDrawing { get; set; }
        public double? QTY { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProjectDrawingReference { get; set; }
    }
}
