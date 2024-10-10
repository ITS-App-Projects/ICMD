using ICMD.Core.AuditModels;
using ICMD.Core.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ProjectUser : FullEntityWithAudit<Guid>
    {
        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ICMDUser ICMDUser { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string? Authorization { get; set; }
    }
}
