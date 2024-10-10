using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class ReferenceDocumentChangeLogDto
    {
        public string? Type { get; set; }
        public string? DocumentNo { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
        public string? Sheet { get; set; }
        public string? Status { get; set; }
    }
}
