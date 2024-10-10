using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ReferenceDocument : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string DocumentNumber { get; set; }

        [Column(TypeName = "character varying(2000)")]
        [MaxLength(2000)]
        public string? URL { get; set; }

        [Column(TypeName = "character varying(500)")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Version { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Revision { get; set; }

        public DateTime? Date { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Sheet { get; set; }
        public bool IsVDPDocumentNumber { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public Guid ReferenceDocumentTypeId { get; set; }

        [ForeignKey("ReferenceDocumentTypeId")]
        public virtual ReferenceDocumentType ReferenceDocumentType { get; set; }
    }
}
