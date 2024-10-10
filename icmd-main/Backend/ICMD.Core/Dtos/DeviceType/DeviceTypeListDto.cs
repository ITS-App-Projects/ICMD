using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.DeviceType
{
    public class DeviceTypeListDto: ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? DeviceType { get; set; }
    }
}
