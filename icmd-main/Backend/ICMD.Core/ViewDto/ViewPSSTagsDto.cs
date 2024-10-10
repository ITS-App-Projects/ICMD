using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewPSSTagsDto
    {
        public string? Kind { get; set; }
        public string? CBTagNumber { get; set; }
        public string? CBVariableType { get; set; }
        public string? PLCTag { get; set; }
        public string? PCS7VariableType { get; set; }
        public string? SignalExtension { get; set; }
        public string? ProcessName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StreamName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public string? TagName { get; set; }
        public string? PLCNumber { get; set; }
        public string? NatureOfSignalName { get; set; }
        public string? GSDType { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
