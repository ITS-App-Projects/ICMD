using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Spares
{
    public class SparesReportDetailsDto
    {
        [Column("Total Channels")]
        public int? TotalChanneles { get; set; }

        [Column("Used Channels")]
        public int UsedChanneles { get; set; } = 0;

        public string? Rack { get; set; }

        [Column("PLC Number")]
        public string? PLCNumber { get; set; }

        [Column("Nature of Signal")]
        public string? NatureOfSignal { get; set; }

        [Column("Slot Number")]
        public int? SlotNumber { get; set; }

        public decimal? SpareChannels { get; set; }
        public decimal? PercentUsed { get; set; }
        public decimal? PercentSpare { get; set; }
    }
}
