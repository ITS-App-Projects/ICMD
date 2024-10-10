using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class DeviceType : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Type { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
