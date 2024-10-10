using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class DeviceAttributeValue : FullEntityWithAudit<Guid>
    {
        public Guid DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

        public Guid AttributeValueId { get; set; }
        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue AttributeValue { get; set; }
    }
}
