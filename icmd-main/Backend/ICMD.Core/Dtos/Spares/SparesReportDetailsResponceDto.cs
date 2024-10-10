using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Spares
{
    public class SparesReportDetailsResponceDto
    {

        public string? PLCNumber { get; set; }
        public string? Rack { get; set; }
        public string? NatureOfSignal { get; set; }
        public int? SlotNumber { get; set; }
        public int? TotalChanneles { get; set; }
        public int UsedChanneles { get; set; }
        public decimal? SpareChannels { get; set; }
        public decimal? PercentUsed { get; set; }
        public decimal? PercentSpare { get; set; }
        public List<SparesReportDetailsResponceDto> ChildItems { get; set; } = new List<SparesReportDetailsResponceDto>();
    }
}
