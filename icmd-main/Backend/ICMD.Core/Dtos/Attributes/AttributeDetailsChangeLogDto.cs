using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Attributes
{
    public class AttributeDetailsChangeLogDto
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? OriginalValue { get; set; }
    }
}
