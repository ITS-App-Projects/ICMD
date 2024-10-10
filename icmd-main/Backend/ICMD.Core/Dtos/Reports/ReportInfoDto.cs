using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Reports
{
    public class ReportInfoDto
    {
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Order { get; set; }
    }
}
