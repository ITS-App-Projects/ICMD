using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Project
{
    public class ProjectTagFieldInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Source { get; set; }
        public string? Separator { get; set; }
        public bool IsUsed { get; set; }
        public List<SourceDataInfoDto>? FieldData { get; set; }
    }

    public class SourceDataInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
