using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class EquipmentCode : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Code { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Descriptor { get; set; }
    }
}
