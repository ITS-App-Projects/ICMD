using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.TagType
{
    public class TagTypeInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public string? TagTypeName { get; set; }
        public string? TagDescriptorName { get; set; }
    }
}
