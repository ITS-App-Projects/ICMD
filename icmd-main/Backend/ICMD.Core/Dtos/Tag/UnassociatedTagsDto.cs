using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Tag
{
    public class UnassociatedTagsDto
    {
        public string? Tag { get; set; }
        public string? Process { get; set; }
        public string? SubProcess { get; set; }
        public string? Stream { get; set; }
        public string? EquipmentCode { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
    }
}
