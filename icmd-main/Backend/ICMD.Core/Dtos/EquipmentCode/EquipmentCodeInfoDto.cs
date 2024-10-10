using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.EquipmentCode
{
    public class EquipmentCodeInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Descriptor { get; set; }
    }
}
