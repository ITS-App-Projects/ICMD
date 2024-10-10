using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class GSDType_SignalExtension : FullEntityWithAudit<Guid>
    {
        public Guid GSDTypeId { get; set; }

        [ForeignKey("GSDTypeId")]
        public virtual GSDType GSDType { get; set; }
        
        public Guid SignalExtensionId { get; set; }

        [ForeignKey("SignalExtensionId")]
        public virtual SignalExtension SignalExtension { get; set; }
    }
}
