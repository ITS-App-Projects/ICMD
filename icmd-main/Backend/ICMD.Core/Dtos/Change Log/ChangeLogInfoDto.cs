using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Change_Log
{
    public class ChangeLogInfoDto
    {
        public string? Context { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? EntityName { get; set; }
        public Guid ContextId { get; set; }
        public string? Status { get; set; }
        public string? OriginalValues { get; set; }
        public string? NewValues { get; set; }
        public string? CreatedBy { get; set; }
    }
}
