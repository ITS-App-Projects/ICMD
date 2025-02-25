using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class UIChangeLogRequestDto
    {
        public Guid ProjectId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Tag { get; set; }
        public string? PLCNo { get; set; }
        public string? UserName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
