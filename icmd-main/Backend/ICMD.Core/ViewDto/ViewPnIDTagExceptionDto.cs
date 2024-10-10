using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewPnIDTagExceptionDto
    {
        public string? TagName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? ProcessName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StreamName { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public string? ServiceDescription { get; set; }
        public string? SkidTag { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
