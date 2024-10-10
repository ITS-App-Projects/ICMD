using ICMD.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.DeviceModel
{
    public class DeviceModelListDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public string? Manufacturer { get; set; }
        public Guid? ManufacturerId { get; set; }
    }
}
