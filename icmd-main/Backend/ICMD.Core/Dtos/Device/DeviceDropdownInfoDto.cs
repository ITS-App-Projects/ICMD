using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Device
{
    public class DeviceDropdownInfoDto
    {
        public List<DropdownInfoDto>? DeviceTypes { get; set; }
        public List<DropdownInfoDto>? TagList { get; set; }
        public List<DropdownInfoDto>? ManufacturerList { get; set; }
        public List<DropdownInfoDto>? FailStateList { get; set; }
        public List<DropdownInfoDto>? ZoneList { get; set; }
        public List<DropdownInfoDto>? BankList { get; set; }
        public List<DropdownInfoDto>? TrainList { get; set; }
        public List<DropdownInfoDto>? NatureOfSignalList { get; set; }
        public List<DropdownInfoDto>? FieldPanelList { get; set; }
        public List<DropdownInfoDto>? SkidList { get; set; }
        public List<DropdownInfoDto>? StandList { get; set; }
        public List<DropdownInfoDto>? JunctionBoxList { get; set; }
        public List<KeyValueInfoDto>? IsInstrumentOptionsList { get; set; }
        public List<DropdownInfoDto>? ReferenceDocTypeList { get; set; }
        public List<DropdownInfoDto>? WorkAreaPackList { get; set; }
        public List<DropdownInfoDto>? ConnectionTagList { get; set; }
        public List<DropdownInfoDto>? InstrumentTagList { get; set; }
        public List<DropdownInfoDto>? CableDeviceTagList { get; set; }

    }

    public class KeyValueInfoDto
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
