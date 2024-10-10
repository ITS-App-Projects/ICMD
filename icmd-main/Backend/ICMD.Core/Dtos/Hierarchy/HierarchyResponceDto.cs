using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Hierarchy
{
    public class HierarchyResponceDto
    {
        public List<HierarchyDeviceInfoDto> DeviceList { get; set; }
        public List<DropdownInfoDto> TagList { get; set; }
    }
}
