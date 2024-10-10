using ICMD.Core.Dtos.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.DeviceModel
{
    public class DeviceModelInfoDto
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
        public Guid? ManufacturerId { get; set; }
        public List<AttributesDto>? Attributes { get; set; }
    }
}
