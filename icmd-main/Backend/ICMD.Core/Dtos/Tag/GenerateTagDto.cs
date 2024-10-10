using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Tag
{
    public class GenerateTagDto
    {
        public String? Field1Id { get; set; }
        public String? Field2Id { get; set; }
        public String? Field3Id { get; set; }
        public String? Field4Id { get; set; }
        public String? Field5Id { get; set; }
        public String? Field6Id { get; set; }
        public Guid ProjectId { get; set; }
    }
}
