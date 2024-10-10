using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class PropertyChangeLogDto
    {
        public string? Name { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
