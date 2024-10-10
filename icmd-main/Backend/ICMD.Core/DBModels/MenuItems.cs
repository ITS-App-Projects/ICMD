using ICMD.Core.AuditModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.DBModels
{
    public class MenuItems : FullEntityWithAudit<Guid>
    {
        public string? MenuName { get; set; }
        public string? ControllerName { get; set; }
        public string? MenuDescription { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public Guid? ParentMenuId { get; set; }
        [ForeignKey("ParentMenuId")]
        public virtual MenuItems? ParentMenu { get; set; }
        public bool IsPermission { get; set; }
    }
}
