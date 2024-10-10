using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Change_Log
{
    public class ChangeLogListDto
    {
        public string? Key { get; set; }
        public List<ChangeLogInfoDto>? Items { get; set; }
    }
}
