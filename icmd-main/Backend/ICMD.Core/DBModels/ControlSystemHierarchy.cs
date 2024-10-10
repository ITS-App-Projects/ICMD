using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ControlSystemHierarchy : FullEntityWithAudit<Guid>
    {
        public bool Instrument { get; set; }

        public Guid ParentDeviceId { get; set; }
        [ForeignKey("ParentDeviceId")]
        public virtual Device ParentDevice { get; set; }

        public Guid ChildDeviceId { get; set; }
        [ForeignKey("ChildDeviceId")]
        public virtual Device ChildDevice { get; set; }
    }
}
