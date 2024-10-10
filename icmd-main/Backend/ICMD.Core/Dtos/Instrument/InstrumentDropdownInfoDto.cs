using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Instrument
{
    public class InstrumentDropdownInfoDto
    {
        public List<DropdownInfoDto>? EquipmentCodeList { get; set; }
        public List<DropdownInfoDto>? ManufacturerList { get; set; }
        public List<DropdownInfoDto>? ProcessList { get; set; }
        public List<DropdownInfoDto>? ZoneList { get; set; }
        public List<DropdownInfoDto>? TagList { get; set; }
        public List<DropdownInfoDto>? NatureOfSignalList { get; set; }
        public List<DropdownInfoDto>? PLCNumberList { get; set; }
    }
}
