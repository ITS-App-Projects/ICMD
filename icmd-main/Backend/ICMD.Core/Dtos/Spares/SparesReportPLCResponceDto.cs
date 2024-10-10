using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Spares
{
    public class SparesReportPLCResponceDto
    {
        public string? PLCNumber { get; set; }
        public string? NatureOfSignal { get; set; }
        public int? TotalChanneles { get; set; }
        public int UsedChanneles { get; set; }
        public decimal? SpareChannels { get; set; }
        public decimal? PercentUsed { get; set; }
        public decimal? PercentSpare { get; set; }
        public List<SparesReportPLCResponceDto> ChildItems { get; set; } = new List<SparesReportPLCResponceDto>();
    }
}
