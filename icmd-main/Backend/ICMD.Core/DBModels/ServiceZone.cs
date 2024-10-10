using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ServiceZone : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string Zone { get; set; }

        [Column(TypeName = "character varying(500)")]
        [MaxLength(500)]
        public string? Description { get; set; }

        public int? Area { get; set; }

        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
