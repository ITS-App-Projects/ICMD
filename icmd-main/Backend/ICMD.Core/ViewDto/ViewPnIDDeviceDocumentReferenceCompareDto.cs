using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewPnIDDeviceDocumentReferenceCompareDto
    {
        public Guid ProjectId { get; set; }
        public Guid DeviceId { get; set; } 
        public string? Tag { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
        public string? PnIdDocumentNumber { get; set; }
        public string? PnIdRevision { get; set; }
        public string? PnIdVersion { get; set; }
    }
}
