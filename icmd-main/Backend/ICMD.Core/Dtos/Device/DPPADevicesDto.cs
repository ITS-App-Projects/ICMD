using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Device
{
    public class DPPADevicesDto
    {
        public string? PLCNumber { get; set; }

        public string? PLCSlotNumber { get; set; }

        public string? DPPACoupler { get; set; }

        public string? AFDHubNumber { get; set; }

        public int? NoOfDPDevices { get; set; }

        public int? NoOfPADevices { get; set; }
        public List<DPPADevicesDto> ChildInfo { get; set; } = new List<DPPADevicesDto>();
    }
}
