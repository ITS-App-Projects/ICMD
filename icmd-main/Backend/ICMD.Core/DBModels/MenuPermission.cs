using ICMD.Core.AuditModels;
using ICMD.Core.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.DBModels
{
    public class MenuPermission : FullEntityWithAudit<Guid>
    {
        public Guid MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual MenuItems MenuItems { get; set; }
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual ICMDRole ICMDRole { get; set; }
        public bool IsGranted { get; set; }
    }
}
