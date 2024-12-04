using System.ComponentModel.DataAnnotations.Schema;

using ICMD.Core.AuditModels;

namespace ICMD.Core.DBModels
{
    public class CableHierarchy : FullEntityWithAudit<Guid>
    {
        public bool Instrument { get; set; }

        public Guid OriginDeviceId { get; set; }
        [ForeignKey("OriginDeviceId")]
        public virtual Device OriginDevice { get; set; }

        public Guid DestinationDeviceId { get; set; }
        [ForeignKey("DestinationDeviceId")]
        public virtual Device DestinationDevice { get; set; }
    }
}
