using ICMD.Core.AuditModels;
using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.DBModels
{
    public class PermissionManagement : FullEntityWithAudit<Guid>
    {
        public Guid MenuPermissionId { get; set; }
        [ForeignKey("MenuPermissionId")]
        public virtual MenuPermission MenuPermission { get; set; }

        public Operations Operation { get; set; }
        public bool IsGranted { get; set; }
    }
}
