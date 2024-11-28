using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Hierarchy
{
    public class HierarchyDeviceInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool Instrument { get; set; }
        public bool IsFolder { get; set; }
        public bool IsActive { get; set; }
        public bool HasChildren => ChildrenList != null && ChildrenList.Any();
        public List<HierarchyDeviceInfoDto> ChildrenList { get; set; } = new();
    }
}
