using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Hierarchy
{
    public class HierarchyRequestDto
    {
        public Guid? ProjectId { get; set; }
        public string? HieararchyType { get; set; }
        public string? Option { get; set; }
        public string? TagName { get; set; }
    }
}
