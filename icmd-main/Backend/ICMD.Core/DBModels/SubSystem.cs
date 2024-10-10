using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SubSystem : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string Number { get; set; }

        [Column(TypeName = "character varying(500)")]
        [MaxLength(500)]
        public string Description { get; set; }
        public Guid SystemId { get; set; }

        [ForeignKey("SystemId")]
        public virtual System System { get; set; }
    }
}
