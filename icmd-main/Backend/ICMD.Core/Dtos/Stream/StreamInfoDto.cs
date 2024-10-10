using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Stream
{
    public class StreamInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? StreamName { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
    }
}
