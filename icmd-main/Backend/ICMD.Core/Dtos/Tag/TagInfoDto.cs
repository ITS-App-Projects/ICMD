using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Tag
{
    public class TagInfoDto
    {
        public Guid Id { get; set; }
        public string? TagName { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public Guid ProcessId { get; set; }
        public Guid SubProcessId { get; set; }
        public Guid StreamId { get; set; }
        public Guid EquipmentCodeId { get; set; }
    }
}
