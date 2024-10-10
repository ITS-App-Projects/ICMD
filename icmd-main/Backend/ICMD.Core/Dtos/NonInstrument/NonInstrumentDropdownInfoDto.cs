using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.NonInstrument
{
    public class NonInstrumentDropdownInfoDto
    {
        public List<DropdownInfoDto>? TagList { get; set; }
        public List<DropdownInfoDto>? PLCNumberList { get; set; }
        public List<DropdownInfoDto>? EquipmentCodeList { get; set; }
        public List<DropdownInfoDto>? LocationList { get; set; }
    }
}
