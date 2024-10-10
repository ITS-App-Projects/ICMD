using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class DeviceClassification : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string Classification { get; set; }
    }
}
