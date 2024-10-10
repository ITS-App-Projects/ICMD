using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewCountDPPADevicesDto
    {
        public Guid? ProjectId { get; set; }

        [Column("PLC_Number")]
        public string? PLCNumber { get; set; }

        [Column("PLC_Slot_Number")]
        public string? PLCSlotNumber { get; set; }

        [Column("DP_or_PA_Coupler")]
        public string? DPPACoupler { get; set; }

        [Column("AFD___Hub_Number")]
        public string? AFDHubNumber { get; set; }

        [Column("No__of_DP_Devices")]
        public int? NoOfDPDevices { get; set; }

        [Column("No__of_PA_Devices")]
        public int? NoOfPADevices { get; set; }
    }
}
