using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.PnIdTags
{
    public class PnIdDeviceDocumentReferenceRequestDto
    {
        public Guid ProjectId { get; set; }
        public PnIdDeviceMisMatchDocumentReference Type { get; set; }
    }
}
