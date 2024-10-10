using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ReferenceDocumentType : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Type { get; set; }
    }
}
