using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.SubSystem
{
    public class SubSystemInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? WorkAreaPack { get; set; }
        public string? System { get; set; }
        public string? Number { get; set; }
        public string? Description { get; set; }
        public Guid? WorkAreaPackId { get; set; }
        public Guid SystemId { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
