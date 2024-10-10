using ICMD.Core.Dtos.Instrument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Node_Address
{
    public class DuplicateRackSlotChannelDto
    {
        public string? RackNo { get; set; }
        public List<ViewInstrumentListLiveDto> Items { get; set; } = new List<ViewInstrumentListLiveDto>();
    }
}
