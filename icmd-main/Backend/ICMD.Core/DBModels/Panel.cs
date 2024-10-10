using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class Panel : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Type { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public Guid? ReferenceDocumentId { get; set; }
        [ForeignKey("ReferenceDocumentId")]
        public virtual ReferenceDocument? ReferenceDocument { get; set; }
    }
}
