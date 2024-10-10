using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos
{
    public class DropdownInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Authorization { get; set; }
    }
}
