using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ProcessHierarchy : FullEntityWithAudit<Guid>
    {
        public Guid ChildProcessLevelId { get; set; }

        [ForeignKey("ChildProcessLevelId")]
        public virtual ProcessLevel ChildProcessLevel { get; set; }
        
        public Guid ParentProcessLevelId { get; set; }

        [ForeignKey("ParentProcessLevelId")]
        public virtual ProcessLevel ParentProcessLevel { get; set; }
    }
}
