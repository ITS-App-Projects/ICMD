
using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class OLMDPTDPR : FullEntityWithAudit<Guid>
    {
        public double? No { get; set; }
        public Guid OLMDPTDPRDeviceTag { get; set; }
        public double? PLCSlotNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DeviceType { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DevicePhysicalLocation { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PLCNo { get; set; }
    }
}
