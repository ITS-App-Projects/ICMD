using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class NatureOfSignalSignalExtension : FullEntityWithAudit<Guid>
    {
        public Guid NatureOfSignalId { get; set; }

        [ForeignKey("NatureOfSignalId")]
        public virtual NatureOfSignal NatureOfSignal { get; set; }
        public Guid SignalExtensionId { get; set; }

        [ForeignKey("SignalExtensionId")]
        public virtual SignalExtension SignalExtension { get; set; }
    }
}
