using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ChangeLog : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Context { get; set; }

        public Guid ContextId { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string EntityName { get; set; }

        [Column(TypeName = "uuid")]
        public Guid EntityId { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string Status { get; set; }

        public string OriginalValues { get; set; }

        public string NewValues { get; set; }

        public Guid? ProjectId { get; set; }
    }
}
