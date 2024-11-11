using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ICMD.Core.AuditModels;

namespace ICMD.Core.DBModels
{
    public class TagType : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(30)")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Column(TypeName = "character varying(80)")]
        public string? Description { get; set; }
    }
}
