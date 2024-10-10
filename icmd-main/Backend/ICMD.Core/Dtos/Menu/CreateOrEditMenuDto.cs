using ICMD.Core.AuditModels;
using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Menu
{
    public class CreateOrEditMenuDto : FullEntityWithAudit<Guid>
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string MenuName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(250, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string MenuDescription { get; set; }
        public string ControllerName { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public int SortOrder { get; set; }
        public Guid? ParentMenuId { get; set; }
        public bool IsPermission { get; set; }
    }
}
