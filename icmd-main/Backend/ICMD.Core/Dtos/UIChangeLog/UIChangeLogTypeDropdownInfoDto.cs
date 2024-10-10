using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class UIChangeLogTypeDropdownInfoDto
    {
        public List<string>? Types { get; set; }
        public List<DropdownInfoDto>? TagList { get; set; }
        public List<DropdownInfoDto>? PLCList { get; set; }
        public List<DropdownInfoDto>? UserList { get; set; }
    }
}
