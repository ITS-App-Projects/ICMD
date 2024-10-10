using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ReferenceDocumentDevice : FullEntityWithAudit<Guid>
    {
        public Guid DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

        public Guid ReferenceDocumentId { get; set; }
        [ForeignKey("ReferenceDocumentId")]
        public virtual ReferenceDocument ReferenceDocument { get; set; }
    }
}
