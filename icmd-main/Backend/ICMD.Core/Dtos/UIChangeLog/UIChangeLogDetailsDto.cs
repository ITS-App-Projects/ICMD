using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class UIChangeLogDetailsDto
    {
        public Guid Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string? PLCNumber { get; set; }
        public string Changes { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string UserName { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
