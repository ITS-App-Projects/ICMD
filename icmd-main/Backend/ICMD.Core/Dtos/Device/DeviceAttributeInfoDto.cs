using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Device
{
    public class DeviceAttributeInfoDto
    {
        public Guid? DeviceTypeId { get; set; }
        public Guid? DeviceModelId { get; set; }
        public Guid? NatureOfSignalId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? DeviceId { get; set; }
        public Guid? ConnectionParentTagId { get; set; }
    }
}
