using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class System : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string Number { get; set; }
        [Column(TypeName = "character varying(500)")]
        [MaxLength(500)]
        public string Description { get; set; }

        public Guid WorkAreaPackId { get; set; }

        [ForeignKey("WorkAreaPackId")]
        public virtual WorkAreaPack WorkAreaPack { get; set; }
    }
}
