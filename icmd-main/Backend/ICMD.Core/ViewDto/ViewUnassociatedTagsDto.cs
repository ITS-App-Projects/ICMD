using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewUnassociatedTagsDto
    {
        public Guid Id { get; set; }
        public string? TagName { get; set; }
        public Guid? ProcessId { get; set; }
        public Guid? SubProcessId { get; set; }
        public Guid? StreamId { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public Guid? EquipmentCodeId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
