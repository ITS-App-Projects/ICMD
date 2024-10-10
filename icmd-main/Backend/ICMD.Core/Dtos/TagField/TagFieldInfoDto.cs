using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.TagField
{
    public class TagFieldInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Source { get; set; }
        public string? Separator { get; set; }
        public Guid ProjectId { get; set; }
        public bool IsEditable { get; set; }
    }
}
