using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Reports
{
    public class ReportListDto
    {
        public string? Group { get; set; }
        public List<ReportInfoDto>? Items { get; set; }
    }
}
